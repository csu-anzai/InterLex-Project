using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Services
{
    using System.Security.Claims;
    using Common;
    using Data;
    using Interlex.Exceptions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models.RequestModels;
    using Models.ResponseModels;

    public class UsersService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly CurrentUserService currentUserService;
        private readonly AppDbContext context;

        public UsersService(UserManager<ApplicationUser> userManager, CurrentUserService currentUserService, AppDbContext context)
        {
            this.userManager = userManager;
            this.currentUserService = currentUserService;
            this.context = context;
        }

        public async Task<IEnumerable<UserListModel>> GetUsers()
        {
            var isSuperAdmin = this.currentUserService.IsSuperAdmin();
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            IQueryable<ApplicationUser> users;
            if (isSuperAdmin)
            {
                users = this.context.Users.AsQueryable();
            }
            else
            {
                var org = this.currentUserService.GetCurrentUserOrgClaim();
                users = this.context.Users.Include(u => u.Claims)
                    .Where(u => u.Organization.ShortName == org)
                    .Where(u => u.Claims.FirstOrDefault(c => c.ClaimType == Constants.Privileges).ClaimValue != Constants.SuperAdmin);
            }
            var result = users.Select(u => new UserListModel
            {
                IsActive = u.IsActive.Value,
                Username = u.UserName,
                Organization = u.Organization.ShortName,
                Privileges = u.Claims.FirstOrDefault(c => c.ClaimType == Constants.Privileges).ClaimValue
            });
            return await result.ToListAsync();
        }

        internal async Task ResetUser(UserResetModel model)
        {
            var user = await this.CheckAuthorityAndGetUser(model.Username);
            foreach (var validator in this.userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(this.userManager, user, model.Password);
                if (!result.Succeeded)
                {
                    throw new PasswordChangeException(result.Errors.First().Description);
                }
            }

            var hash = this.userManager.PasswordHasher.HashPassword(user, model.Password);
            user.PasswordHash = hash;
            await this.userManager.UpdateAsync(user);
        }

        internal async Task EditUser(UserEditModel model)
        {
            var user = await this.GetCurrentApplicationUser();
            var result = await this.userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            if (!result.Succeeded)
            {
                throw new PasswordChangeException(result.Errors.First().Description);
            }
        }

        public async Task<ApplicationUser> GetCurrentApplicationUser()
        {
            var user = await this.userManager.GetUserAsync(this.currentUserService.User);
            return user;
        }

        public async Task RegisterUser(UserRegisterModel model)
        {
            var isSuperAdmin = this.currentUserService.IsSuperAdmin();
            var user = new ApplicationUser
            {
                Email = model.Username,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var privilegeClaim = new Claim(Constants.Privileges, nameof(UserPrivileges.User));
            var orgClaim = new Claim(Constants.Organization, this.currentUserService.GetCurrentUserOrgClaim());
            if (isSuperAdmin)
            {
                if (model.Organization?.ShortName.IsNotNull() == true)
                {
                    var org = this.context.Organizations.FirstOrDefault(x => x.ShortName == model.Organization.ShortName);
                    user.Organization = org ?? throw new NotFoundException("Organization not found");
                    orgClaim = new Claim(Constants.Organization, org.ShortName);
                }

                if (model.Privileges != UserPrivileges.User)
                {
                    privilegeClaim = new Claim(Constants.Privileges, model.Privileges.ToString());
                }
            }
            else if ((model.Organization?.ShortName.IsNotNull() == true) || model.Privileges != UserPrivileges.User)
            {
                throw new NotAuthorizedException("You're not authorized to do this.");
            }

            if (user.Organization == null)
            {
                var org = this.context.Organizations.FirstOrDefault(x => x.ShortName == orgClaim.Value);
                user.Organization = org ?? throw new NotFoundException("Organization not found");
            }

            var created = await this.userManager.CreateAsync(user, model.Password);
            if (!created.Succeeded)
            {
                throw new UserCreationFailedException(created.Errors.First().Description);
            }

            await this.userManager.AddClaimsAsync(user, new[] { privilegeClaim, orgClaim });
        }

        internal async Task ChangeActiveStateOfUser(string username, bool changeTo)
        {

            var user = await this.CheckAuthorityAndGetUser(username);
            user.IsActive = changeTo;
            await this.userManager.UpdateAsync(user);
        }

        private async Task<ApplicationUser> CheckAuthorityAndGetUser(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new NotFoundException("Username not found");
            }
            var hasAuthorityToModify = false;
            if (this.currentUserService.IsSuperAdmin())
            {
                hasAuthorityToModify = true;
            }
            else
            {
                var currentUserOrgClaim = this.currentUserService.GetCurrentUserOrgClaim();
                var reqUserClaims = await this.userManager.GetClaimsAsync(user);
                var reqUserPrivClaim = reqUserClaims.First(x => x.Type == Constants.Privileges).Value;
                var reqUserOrgClaim = reqUserClaims.First(x => x.Type == Constants.Organization).Value;
                var reqUserIsUser = reqUserPrivClaim != Constants.SuperAdmin;   // this allows disabling other admins - might change

                var isSameOrg = currentUserOrgClaim == reqUserOrgClaim;
                hasAuthorityToModify = reqUserIsUser && isSameOrg;
            }

            if (!hasAuthorityToModify)
            {
                throw new NotAuthorizedException("Not authorized to do this");
            }
            return user;
        }
    }
}

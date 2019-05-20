namespace Interlex.Services
{
    using System.Linq;
    using System.Security.Claims;
    using Common;
    using Microsoft.AspNetCore.Http;

    public class CurrentUserService
    {
        private readonly IHttpContextAccessor context;

        public CurrentUserService(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public ClaimsPrincipal User => this.context.HttpContext.User;

        public bool IsAdmin()
        {
            var priviledgeClaim = this.context.HttpContext.User.Claims.First(x => x.Type == Constants.Privileges);
            return priviledgeClaim.Value == Constants.Admin || priviledgeClaim.Value == Constants.SuperAdmin;
        }

        public bool IsSuperAdmin()
        {
            return this.context.HttpContext.User.HasClaim(Constants.Privileges, Constants.SuperAdmin);
        }

        public string GetCurrentUserIdClaim()
        {
            var claim = this.context.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            return claim.Value;
        }

        public string GetCurrentUserOrgClaim()
        {
            var claim = this.context.HttpContext.User.Claims.First(x => x.Type == Constants.Organization);
            return claim.Value;
        }
        // add useful properties for claims username etc.
    }
}

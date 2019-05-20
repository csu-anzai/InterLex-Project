namespace Interlex.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Data;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Models.RequestModels;
    using Models.ResponseModels;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password) && user.IsActive.Value)
            {
                var responseUser = new UserLoggedModel
                {
                    Username = user.UserName
                };
                var userClaims = await userManager.GetClaimsAsync(user);
                var orgClaim = userClaims.First(x => x.Type == Constants.Organization);
                var privileges = userClaims.First(c => c.Type == Constants.Privileges);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    orgClaim,
                    privileges
                };
                responseUser.Privileges = Enum.Parse<UserPrivileges>(privileges.Value, true);
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config.GetValue<string>("SigningKey")));
                var token = new JwtSecurityToken(
                    issuer: "https://www.apis.bg",
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(3),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );
                responseUser.Expiration = token.ValidTo;
                responseUser.Token = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(responseUser);
            }

            return Unauthorized();
        }
    }
}
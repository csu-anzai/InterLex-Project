using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Interlex.Controllers
{
    using Data;
    using Exceptions;
    using Interlex.Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.RequestModels;
    using Services;

    [Route("api/[controller]")]
    [Authorize(Policy = Constants.Admin)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService service;

        public UsersController(UsersService service)
        {
            this.service = service;
        }

        [HttpGet("GetUsers")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.service.GetUsers();
            return Ok(users);
        }

        [HttpGet("Hello")]
        [ResponseCache(NoStore = true)]
        [AllowAnonymous]
        public IActionResult Hello()
        {

            return Ok("HelloFromApi");
        }

        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser([FromBody]UserEditModel model)
        {
            try
            {
                await this.service.EditUser(model);
            }
            catch (PasswordChangeException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost("ResetUser")]
        public async Task<IActionResult> ResetUser([FromBody]UserResetModel model)
        {
            try
            {
                await this.service.ResetUser(model);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized();
            }
            catch (PasswordChangeException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }


        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegisterModel model)
        {
            try
            {
                await this.service.RegisterUser(model);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized();
            }
            catch (UserCreationFailedException e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpPost("DeactivateUser")]
        public async Task<IActionResult> DeactivateUser([FromBody] UserChangeStateModel model)
        {
            try
            {
                await this.service.ChangeActiveStateOfUser(model.Username, false);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized();
            }
            return Ok();
        }


        [HttpPost("ActivateUser")]
        public async Task<IActionResult> ActivateUser([FromBody] UserChangeStateModel model)
        {
            try
            {
                await this.service.ChangeActiveStateOfUser(model.Username, true);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotAuthorizedException)
            {
                return Unauthorized();
            }
            return Ok();
        }


    }
}
namespace Interlex.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Repo;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class ValuesController : ControllerBase
    {
        private readonly Repository repo;
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ValuesController(Repository repo, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.repo = repo;
            this.context = context;
            this.userManager = userManager;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            var user = this.userManager.GetUserAsync(User).Result.Organization.ShortName;
            return user;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<string> Get(int id)
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }


        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
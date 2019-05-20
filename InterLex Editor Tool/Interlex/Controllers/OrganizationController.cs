using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interlex.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Services;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.SuperAdmin)]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService service;

        public OrganizationController(OrganizationService service)
        {
            this.service = service;
        }

        [HttpGet("GetOrganizationNames")]
        public async Task<IActionResult> GetOrganizationNames()
        {
            var names = await this.service.GetOrganizationNames();
            return Ok(names);
        }
    }
}
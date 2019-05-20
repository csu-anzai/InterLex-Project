using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Interlex.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Interlex.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReferenceEditorController : ControllerBase
    {
        private readonly ReferenceEditorRepository repository;

        public ReferenceEditorController(ReferenceEditorRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("actInfo")]
        public async Task<IActionResult> GetActInfoByCelexAsync([FromQuery, Required]String celex)
        {
            var info = await this.repository.GetEuActInfoAsync(celex);
            if (info is null)
            {
                return this.NotFound(new { celex = celex });
            }
            else
            {
                return this.Ok(info);
            }
        }
    }
}
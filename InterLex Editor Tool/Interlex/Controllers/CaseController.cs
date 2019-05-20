using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interlex.Controllers
{
    using Data;
    using Exceptions;
    using Interlex.Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Models.RequestModels;
    using Services;

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly CaseService service;

        public CaseController(CaseService service)
        {
            this.service = service;
        }

        [HttpGet("GetTreeData/{id:guid?}")]
        public IActionResult GetTreeData([FromRoute] Guid? id)
        {
            var data = this.service.GetTreeModel(id);
            return Ok(data);
        }

        [HttpGet("GetWholeTree")]
        public async Task<IActionResult> GetWholeTree()
        {
            var tree = this.service.GetWholeTree();
            return Ok(tree);
        }

        [HttpGet("GetSuggestions/{code}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetSuggestions(string code)
        {
            var suggestions = await this.service.GetSuggestions(code);
            return Ok(suggestions);
        }

        [HttpGet("GetAllSuggestions")]
        [ResponseCache(NoStore = true)]
        [Authorize(Policy = Constants.SuperAdmin)]
        public IActionResult GetAllSuggestions()
        {
            var suggestions = this.service.GetAllSuggestions();
            return Ok(suggestions);
        }

        [HttpPost("DeleteSuggestion")]
        [Authorize(Policy = Constants.SuperAdmin)]
        public async Task<IActionResult> DeleteSuggestion([FromBody] SuggestionReqModel model)
        {
            try
            {
                await this.service.DeleteSuggestion(model);
            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost("SaveCase")]
        public async Task<IActionResult> SaveCase([FromBody] CaseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await this.service.SaveCase(model);
            return Ok(id);
        }

        [HttpPost("EditCase/{id:int}")]
        public async Task<IActionResult> EditCase([FromBody]CaseModel model, [FromRoute]int id)
        {
            try
            {
                await this.service.EditCase(model, id);

            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost("DeleteCase/{id:int}")]
        public async Task<IActionResult> DeleteCase([FromRoute]int id)
        {
            try
            {
                await this.service.DeleteCase(id);
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

        [HttpPost("GetCasesList")]
        public async Task<IActionResult> GetCasesListAsync([FromBody] CaseListRequestModel request)
        {
            var data = await this.service.GetCaseList(request);
            return Ok(data);
        }

        [HttpGet("GetCaseContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetCaseContent(int id)
        {
            var data = await this.service.GetCaseContent(id);

            return Ok(data);
        }
    }
}
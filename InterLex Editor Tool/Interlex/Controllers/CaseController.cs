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
    using Microsoft.Net.Http.Headers;
    using Models.RequestModels;
    using Services;

    [Route("api/[controller]")]
    // [Authorize]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly CaseService service;
        private readonly AknConvertService aknConvertService;

        public CaseController(CaseService service, AknConvertService aknConvertService)
        {
            this.service = service;
            this.aknConvertService = aknConvertService;
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

        [HttpPost("SaveMetadata")]
        public async Task<IActionResult> SaveMetadata([FromBody] MetadataModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await this.service.SaveMetadata(model);
            return Ok(id);
        }

        [HttpPost("EditCase/{id:int}")]
        public async Task<IActionResult> EditCase([FromBody] CaseModel model, [FromRoute] int id)
        {
            try
            {
                await this.service.EditCase(model, id);
            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return Ok();
        }

        [HttpPost("EditMetadata/{id:int}")]
        public async Task<IActionResult> EditMetadata([FromBody] MetadataModel model, [FromRoute] int id)
        {
            try
            {
                await this.service.EditMetadata(model, id);
            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotAuthorizedException)
            {
                return this.Unauthorized();
            }

            return Ok();
        }

        [HttpGet("GetMetaFile/{id:int}")]
        //        [AllowAnonymous]
        public async Task<IActionResult> GetMetaFile([FromRoute] int id)
        {
            var data = await this.service.GetMetaFile(id);
            this.Response.Headers.Add("File-name", data.Name);
            this.Response.Headers.Add("access-control-expose-headers",
                "File-name"); // needed to expose headers to Angular
            return this.File(data.Content, data.MimeType, data.Name);
        }

        [HttpGet("GetMetaTranslatedFile/{id:int}")]
        //        [AllowAnonymous]
        public async Task<IActionResult> GetMetaTranslatedFile([FromRoute] int id)
        {
            var data = await this.service.GetMetaTranslatedFile(id);
            this.Response.Headers.Add("File-name", data.Name);
            this.Response.Headers.Add("access-control-expose-headers",
                "File-name"); // needed to expose headers to Angular
            return this.File(data.Content, data.MimeType, data.Name);
        }

        [HttpPost("DeleteMeta/{id:int}")]
        public async Task<IActionResult> DeleteMeta([FromRoute] int id)
        {
            try
            {
                await this.service.DeleteMeta(id);
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

        [HttpPost("DeleteCase/{id:int}")]
        public async Task<IActionResult> DeleteCase([FromRoute] int id)
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
            return this.Ok(data);
        }

        [HttpPost("GetMetaList")]
        public async Task<IActionResult> GetMetaListAsync([FromBody] CaseListRequestModel request)
        {
            var data = await this.service.GetMetadataList(request);
            return this.Ok(data);
        }

        [HttpGet("GetCaseContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetCaseContent(int id)
        {
            var data = await this.service.GetCaseContent(id);

            return this.Ok(data);
        }

        [HttpGet("GetMetaContent/{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetMetaContent(int id)
        {
            var data = await this.service.GetMetaContent(id);
            return this.Ok(data);
        }

        [HttpGet("GetCaseHtmlContent/{id:int}")]
        public async Task<IActionResult> GetCaseHtmlContent(int id)
        {
            var jsonContent = (await this.service.GetCaseContent(id)).Content;
            var documentType = 1;

            var html = await this.aknConvertService.ConvertToHtmlAsync(documentType, jsonContent);

            return this.Ok(html);
        }

        [HttpGet("GetMetaHtmlContent/{id:int}")]
        public async Task<IActionResult> GetMetaHtmlContent(int id)
        {
            var jsonContent = (await this.service.GetMetaContent(id)).Content;
            var documentType = 2;

            var html = await this.aknConvertService.ConvertToHtmlAsync(documentType, jsonContent);

            return this.Ok(html);
        }
    }
}
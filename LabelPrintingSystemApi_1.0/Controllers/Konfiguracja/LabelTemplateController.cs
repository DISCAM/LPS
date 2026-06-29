using Data;
using Data.Dtos.LabelTemplate;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.Konfiguracja
{
    [ApiController]
    [Authorize]
    public class LabelTemplateController : ControllerBase
    {
        private readonly ILabelTemplateService labelTemplateService;

        public LabelTemplateController(
            ILabelTemplateService labelTemplateService)
        {
            this.labelTemplateService = labelTemplateService;
        }

        [HttpGet]
        [Route(Urls.LABEL_TEMPLATES)]
        public async Task<IActionResult> GetAllLabelTemplatesAsync()
        {
            IEnumerable<LabelTemplateDto> labelTemplates =
                await labelTemplateService.GetAllLabelTemplatesAsync();

            return Ok(labelTemplates);
        }

        [HttpGet]
        [Route(Urls.LABEL_TEMPLATE_ID)]
        public async Task<IActionResult> GetLabelTemplateByIdAsync(int id)
        {
            LabelTemplateDto labelTemplate =
                await labelTemplateService.GetLabelTemplateByIdAsync(id);

            return Ok(labelTemplate);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpPost]
        [Route(Urls.LABEL_TEMPLATE_ADD)]
        public async Task<IActionResult> CreateLabelTemplateAsync(
            [FromBody] LabelTemplateCreateDto dto)
        {
            await labelTemplateService.CreateLabelTemplateAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpPut]
        [Route(Urls.LABEL_TEMPLATE_EDIT)]
        public async Task<IActionResult> EditLabelTemplateAsync(
            int id,
            [FromBody] LabelTemplateEditDto dto)
        {
            if (id != dto.LabelTemplateId)
            {
                return BadRequest("ID mismatch");
            }

            await labelTemplateService.EditLabelTemplateAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin,Manager")]
        [HttpDelete]
        [Route(Urls.LABEL_TEMPLATE_DELETE)]
        public async Task<IActionResult> DeleteLabelTemplateAsync(int id)
        {
            await labelTemplateService.DeleteLabelTemplateAsync(id);

            return Ok();
        }
    }
}
using Data;
using Data.Dtos.LogisticUnits;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabelPrintingSystemApi_1._0.Controllers.LogisticUnits
{
    [ApiController]
    [Authorize]
    public class LogisticUnitsController : ControllerBase
    {
        private readonly ILogisticUnitsService logisticUnitsService;

        public LogisticUnitsController(ILogisticUnitsService logisticUnitsService)
        {
            this.logisticUnitsService = logisticUnitsService;
        }

        [HttpGet]
        [Route(Urls.LOGISTIC_UNITS)]
        public async Task<ActionResult<List<LogisticUnitDto>>> GetAll()
        {
            List<LogisticUnitDto> logisticUnits = await this.logisticUnitsService.GetAllAsync();

            return this.Ok(logisticUnits);
        }
    }
}
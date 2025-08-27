using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;
using LimanTakipSistemi.API.Services.ShipVisitService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipVisitController : ControllerBase
    {
        private readonly IShipVisitService shipVisitService;

        public ShipVisitController(IShipVisitService shipVisitService)
        {
            this.shipVisitService = shipVisitService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? visitId = null,
            [FromQuery] int? shipId = null,
            [FromQuery] int? portId = null,
            [FromQuery] DateTime? arrivalDate = null,
            [FromQuery] DateTime? departureDate = null,
            [FromQuery] string? purpose = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var visits = await shipVisitService.GetAllAsync(visitId, shipId, portId, arrivalDate, departureDate, purpose, pageNumber, pageSize);
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving ship visits", error = ex.Message });
            }
        }

       
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var visit = await shipVisitService.GetByIdAsync(id);
                if (visit == null)
                {
                    return NotFound(new { message = "Ship visit not found" });
                }
                return Ok(visit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the ship visit", error = ex.Message });
            }
        }

        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddShipVisitRequestDto addShipVisitRequestDto)
        {
            try
            {
                var visitDto = await shipVisitService.CreateAsync(addShipVisitRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = visitDto.VisitId }, visitDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the ship visit", error = ex.Message });
            }
        }

       
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateShipVisitRequestDto updateShipVisitRequestDto)
        {
            try
            {
                var visitDto = await shipVisitService.UpdateAsync(id, updateShipVisitRequestDto);
                if (visitDto == null)
                {
                    return NotFound(new { message = "Ship visit not found" });
                }
                return Ok(visitDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the ship visit", error = ex.Message });
            }
        }

       
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await shipVisitService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Ship visit not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the ship visit", error = ex.Message });
            }
        }

       
        [HttpGet]
        [Route("by-ship/{shipId:int}")]
        public async Task<IActionResult> GetVisitsByShip([FromRoute] int shipId)
        {
            try
            {
                var visits = await shipVisitService.GetVisitsByShipAsync(shipId);
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving visits by ship", error = ex.Message });
            }
        }

       
        [HttpGet]
        [Route("by-port/{portId:int}")]
        public async Task<IActionResult> GetVisitsByPort([FromRoute] int portId)
        {
            try
            {
                var visits = await shipVisitService.GetVisitsByPortAsync(portId);
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving visits by port", error = ex.Message });
            }
        }

       
        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveVisits()
        {
            try
            {
                var visits = await shipVisitService.GetActiveVisitsAsync();
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active visits", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("upcoming")]
        public async Task<IActionResult> GetUpcomingVisits()
        {
            try
            {
                var visits = await shipVisitService.GetUpcomingVisitsAsync();
                return Ok(visits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving upcoming visits", error = ex.Message });
            }
        }

      
        [HttpGet]
        [Route("check-availability")]
        public async Task<IActionResult> CheckShipAvailability(
            [FromQuery] int shipId,
            [FromQuery] DateTime arrivalDate,
            [FromQuery] DateTime departureDate,
            [FromQuery] int? excludeVisitId = null)
        {
            try
            {
                var isAvailable = await shipVisitService.IsShipAvailableForVisitAsync(shipId, arrivalDate, departureDate, excludeVisitId);
                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking ship availability", error = ex.Message });
            }
        }
    }
}

using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Services.ShipService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipController : ControllerBase
    {
        private readonly IShipService shipService;

        public ShipController(IShipService shipService)
        {
            this.shipService = shipService;
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? shipId = null,
            [FromQuery] string? name = null,
            [FromQuery] string? IMO = null,
            [FromQuery] string? type = null,
            [FromQuery] string? flag = null,
            [FromQuery] int? yearBuilt = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var ships = await shipService.GetAllAsync(shipId, name, IMO, type, flag, yearBuilt, pageNumber, pageSize);
                return Ok(ships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving ships", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var ship = await shipService.GetByIdAsync(id);
                if (ship == null)
                {
                    return NotFound(new { message = "Ship not found" });
                }
                return Ok(ship);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the ship", error = ex.Message });
            }
        }

      
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddShipRequestDto addShipRequestDto)
        {
            try
            {
                var shipDto = await shipService.CreateAsync(addShipRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = shipDto.ShipId }, shipDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the ship", error = ex.Message });
            }
        }

        
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateShipRequestDto updateShipRequestDto)
        {
            try
            {
                var shipDto = await shipService.UpdateAsync(id, updateShipRequestDto);
                if (shipDto == null)
                {
                    return NotFound(new { message = "Ship not found" });
                }
                return Ok(shipDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the ship", error = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await shipService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Ship not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the ship", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("check-imo")]
        public async Task<IActionResult> CheckIMOUnique([FromQuery] string imo, [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await shipService.IsIMOUniqueAsync(imo, excludeId);
                return Ok(new { isUnique });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking IMO uniqueness", error = ex.Message });
            }
        }
    }
}

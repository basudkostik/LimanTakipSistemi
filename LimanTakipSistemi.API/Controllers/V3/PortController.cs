using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.Port;
using LimanTakipSistemi.API.Services.PortService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PortController : ControllerBase
    {
        private readonly IPortService portService;

        public PortController(IPortService portService)
        {
            this.portService = portService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? portId = null,
            [FromQuery] string? name = null,
            [FromQuery] string? country = null,
            [FromQuery] string? city = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var ports = await portService.GetAllAsync(portId, name, country, city, pageNumber, pageSize);
                return Ok(ports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving ports", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var port = await portService.GetByIdAsync(id);
                if (port == null)
                {
                    return NotFound(new { message = "Port not found" });
                }
                return Ok(port);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the port", error = ex.Message });
            }
        }

        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddPortRequestDto addPortRequestDto)
        {
            try
            {
                var portDto = await portService.CreateAsync(addPortRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = portDto.PortId }, portDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the port", error = ex.Message });
            }
        }

        
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePortRequestDto updatePortRequestDto)
        {
            try
            {
                var portDto = await portService.UpdateAsync(id, updatePortRequestDto);
                if (portDto == null)
                {
                    return NotFound(new { message = "Port not found" });
                }
                return Ok(portDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the port", error = ex.Message });
            }
        }

        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await portService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Port not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the port", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("check-unique")]
        public async Task<IActionResult> CheckPortUnique(
            [FromQuery] string name, 
            [FromQuery] string country, 
            [FromQuery] string city, 
            [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await portService.IsPortUniqueAsync(name, country, city, excludeId);
                return Ok(new { isUnique });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking port uniqueness", error = ex.Message });
            }
        }
    }
}

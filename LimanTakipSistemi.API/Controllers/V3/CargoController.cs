using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using LimanTakipSistemi.API.Services.CargoService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CargoController : ControllerBase
    {
        private readonly ICargoService cargoService;

        public CargoController(ICargoService cargoService)
        {
            this.cargoService = cargoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? cargoType = null,
            [FromQuery] decimal? minWeight = null,
            [FromQuery] decimal? maxWeight = null,
            [FromQuery] string? description = null,
            [FromQuery] int? shipId = null,
            [FromQuery] int? cargoId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var cargos = await cargoService.GetAllAsync(cargoType, minWeight, maxWeight, description, shipId, cargoId, pageNumber, pageSize);
                return Ok(cargos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cargos", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var cargo = await cargoService.GetByIdAsync(id);
                if (cargo == null)
                {
                    return NotFound(new { message = "Cargo not found" });
                }
                return Ok(cargo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the cargo", error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddCargoRequestDto addCargoRequestDto)
        {
            try
            {
                var cargoDto = await cargoService.CreateAsync(addCargoRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = cargoDto.CargoId }, cargoDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the cargo", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCargoRequestDto updateCargoRequestDto)
        {
            try
            {
                var cargoDto = await cargoService.UpdateAsync(id, updateCargoRequestDto);
                if (cargoDto == null)
                {
                    return NotFound(new { message = "Cargo not found" });
                }
                return Ok(cargoDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the cargo", error = ex.Message });
            }
        }

       
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await cargoService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Cargo not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the cargo", error = ex.Message });
            }
        }
    }
}

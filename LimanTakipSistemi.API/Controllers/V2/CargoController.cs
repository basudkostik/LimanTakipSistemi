using AutoMapper;
using LimanTakipSistemi.API.Mapping;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LimanTakipSistemi.API.Repositories.CargoRepository;
using LimanTakipSistemi.API.CustomActionFilter;
using Asp.Versioning;

namespace LimanTakipSistemi.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class CargoController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICargoRepository cargoRepository;

        public CargoController(ICargoRepository cargoRepository, IMapper mapper)
        {
            this.cargoRepository = cargoRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? cargoType, [FromQuery] Decimal? minWeight, [FromQuery] Decimal? maxWeight,
                [FromQuery] string? description, [FromQuery] int? shipId, [FromQuery] int? cargoId,
                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var cargoes = await cargoRepository.GetAllAsync(cargoType, minWeight, maxWeight, description, shipId, cargoId, pageNumber, pageSize);
            var cargoesDto = mapper.Map<List<CargoDto>>(cargoes);
            return Ok(cargoesDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var cargo = await cargoRepository.GetByIdAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                var cargoDto = mapper.Map<CargoDto>(cargo);
                return Ok(cargoDto);
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddCargoRequestDto addCargoRequestDto)
        {
            var cargo = mapper.Map<Cargo>(addCargoRequestDto);
            cargo = await cargoRepository.CreateAsync(cargo);
            var cargoDto = mapper.Map<CargoDto>(cargo);
            return CreatedAtAction(nameof(GetById), new { id = cargo.CargoId }, cargoDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCargoRequestDto updateCargoRequestDto)
        {
            var cargo = mapper.Map<Cargo>(updateCargoRequestDto);
            cargo = await cargoRepository.UpdateAsync(id, cargo);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                var cargoDto = mapper.Map<CargoDto>(cargo);
                return Ok(cargoDto);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var cargo = await cargoRepository.DeleteAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                var cargoDto = mapper.Map<CargoDto>(cargo);
                return Ok(cargoDto);
            }
        }
    }
}

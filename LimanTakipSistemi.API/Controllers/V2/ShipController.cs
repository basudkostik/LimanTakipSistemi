using AutoMapper;
using LimanTakipSistemi.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Models.Domain;
using Asp.Versioning;
using LimanTakipSistemi.API.Repositories.ShipRepository.cs;

namespace LimanTakipSistemi.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipController : ControllerBase
    {
        private readonly IShipRepository shipRepository;

        private readonly IMapper mapper;

        public ShipController(IShipRepository shipRepository, IMapper mapper)
        {
            this.shipRepository = shipRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? shipId, [FromQuery] string? name, [FromQuery] string? IMO,
        [FromQuery] string? type, [FromQuery] string? flag, [FromQuery] int? yearbuilt,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var ships = await shipRepository.GetAllAsync(shipId, name, IMO, type, flag, yearbuilt, pageNumber = 1, pageSize = 100);
            var shipsDto = mapper.Map<List<ShipDto>>(ships);
            return Ok(shipsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var ship = await shipRepository.GetByIdAsync(id);
            if (ship == null)
            {
                return NotFound();
            }
            var shipDto = mapper.Map<ShipDto>(ship);
            return Ok(shipDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddShipRequestDto addShipRequestDto)
        {

            var ship = mapper.Map<Ship>(addShipRequestDto);
            ship = await shipRepository.CreateAsync(ship);
            var shipDto = mapper.Map<ShipDto>(ship);
            return CreatedAtAction(nameof(GetById), new { id = ship.ShipId }, shipDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateShipRequestDto updateShipRequestDto)
        {
            if (updateShipRequestDto == null)
            {
                return BadRequest("Ship data is required.");
            }


            var ship = mapper.Map<Ship>(updateShipRequestDto);

            ship = await shipRepository.UpdateAsync(id, ship);
            if (ship == null)
            {
                return NotFound();
            }
            else
            {
                var shipDto = mapper.Map<ShipDto>(ship);
                return Ok(shipDto);
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var ship = await shipRepository.DeleteAsync(id);

            if (ship == null)
            {
                return NotFound();
            }
            var shipDto = mapper.Map<ShipDto>(ship);
            return Ok(shipDto);


        }

    }
}
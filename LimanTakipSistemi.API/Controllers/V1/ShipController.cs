using AutoMapper;
using LimanTakipSistemi.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public ShipController(LimanTakipDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var ships = dbContext.Ships.ToList();
            var shipsDto = mapper.Map<List<ShipDto>>(ships);
            return Ok(shipsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var ship = dbContext.Ships.FirstOrDefault(s => s.ShipId == id);
            if (ship == null)
            {
                return NotFound();
            }
            var shipDto = mapper.Map<ShipDto>(ship);
            return Ok(shipDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddShipRequestDto addShipRequestDto)
        {
            if (addShipRequestDto == null)
            {
                return BadRequest("Ship data is required.");
            }
            var ship = mapper.Map<Ship>(addShipRequestDto);
            dbContext.Ships.Add(ship);
            dbContext.SaveChanges();
            var shipDto = mapper.Map<ShipDto>(ship);
            return CreatedAtAction(nameof(GetById), new { id = ship.ShipId }, shipDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateShipRequestDto updateShipRequestDto)
        {
            if (updateShipRequestDto == null)
            {
                return BadRequest("Ship data is required.");
            }
            var ship = dbContext.Ships.FirstOrDefault(s => s.ShipId == id);
            if (ship == null)
            {
                return NotFound();
            }
            else
            {
                ship.Name = updateShipRequestDto.Name;
                ship.IMO = updateShipRequestDto.IMO;
                ship.Type = updateShipRequestDto.Type;
                ship.Flag = updateShipRequestDto.Flag;
                ship.YearBuilt = updateShipRequestDto.YearBuilt;

                dbContext.SaveChanges();
                var shipDto = mapper.Map<ShipDto>(ship);
                return Ok(shipDto);
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var ship = dbContext.Ships.FirstOrDefault(s => s.ShipId == id);
            if (ship == null)
            {
                return NotFound();
            }
            dbContext.Ships.Remove(ship);
            dbContext.SaveChanges();

            var shipDto = mapper.Map<ShipDto>(ship);
            return Ok(shipDto);


        }

    }
}

using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipVisitController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public ShipVisitController(LimanTakipDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetShipVisits()
        {
            var shipVisits = dbContext.ShipVisits.ToList();
            if (shipVisits == null || !shipVisits.Any())
            {
                return NotFound("No ship visits found.");
            }
            var shipVisitDtos = mapper.Map<List<ShipVisitDto>>(shipVisits);
            return Ok(shipVisitDtos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetShipVisitById([FromRoute]int id)
        {
            var shipVisit = dbContext.ShipVisits.FirstOrDefault(sv => sv.VisitId == id);
            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
            return Ok(shipVisitDto);
        }
        [HttpPost]
        public IActionResult CreateShipVisit([FromBody] AddShipVisitRequestDto addShipVisitRequestDto)
        {
            if (addShipVisitRequestDto == null)
            {
                return BadRequest("Ship visit data is required.");
            }
            var shipVisit = mapper.Map<ShipVisit>(addShipVisitRequestDto);
            dbContext.ShipVisits.Add(shipVisit);
            dbContext.SaveChanges();
            var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
            return CreatedAtAction(nameof(GetShipVisitById), new { id = shipVisit.VisitId }, shipVisitDto);
        }
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateShipVisit([FromRoute]int id, [FromBody] AddShipVisitRequestDto updateShipVisitRequestDto)
        {
            if (updateShipVisitRequestDto == null)
            {
                return BadRequest("Ship visit data is required.");
            }
            var shipVisit = dbContext.ShipVisits.FirstOrDefault(sv => sv.VisitId == id);
            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            else
            {
                shipVisit.ShipId = updateShipVisitRequestDto.ShipId;
                shipVisit.PortId = updateShipVisitRequestDto.PortId;
                shipVisit.ArrivalDate = updateShipVisitRequestDto.ArrivalDate;
                shipVisit.DepartureDate = updateShipVisitRequestDto.DepartureDate;
                shipVisit.Purpose = updateShipVisitRequestDto.Purpose;

                dbContext.SaveChanges();
                var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
                return Ok(shipVisitDto);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteShipVisit([FromRoute]int id)
        {
            var shipVisit = dbContext.ShipVisits.FirstOrDefault(sv => sv.VisitId == id);
            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            else
            {
                dbContext.ShipVisits.Remove(shipVisit);
                dbContext.SaveChanges();

                var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
                return Ok(shipVisitDto);
            }
        }
    }
}


using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;
using LimanTakipSistemi.API.Repositories.ShipVisitRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipVisitController : ControllerBase
    {
        private readonly IShipVisitRepository visitRepository;

        private readonly IMapper mapper;

        public ShipVisitController(IShipVisitRepository visitRepository, IMapper mapper)
        {
            this.visitRepository = visitRepository;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? visitId, [FromQuery] int? shipId,
        [FromQuery] int? portId, [FromQuery] DateTime? arrivalDate, [FromQuery] DateTime? departureDate,
        [FromQuery] string? purpose,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var shipVisits = await visitRepository.GetAllAsync(visitId, shipId, portId, arrivalDate, departureDate, purpose, pageNumber = 1, pageSize = 100);
            var shipVisitDtos = mapper.Map<List<ShipVisitDto>>(shipVisits);
            return Ok(shipVisitDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var shipVisit = await visitRepository.GetByIdAsync(id);
            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
            return Ok(shipVisitDto);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddShipVisitRequestDto addShipVisitRequestDto)
        {
            if (addShipVisitRequestDto == null)
            {
                return BadRequest("Ship visit data is required.");
            }
            var shipVisit = mapper.Map<ShipVisit>(addShipVisitRequestDto);
            shipVisit = await visitRepository.CreateAsync(shipVisit);
            var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
            return CreatedAtAction(nameof(GetById), new { id = shipVisit.VisitId }, shipVisitDto);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AddShipVisitRequestDto updateShipVisitRequestDto)
        {
            if (updateShipVisitRequestDto == null)
            {
                return BadRequest("Ship visit data is required.");
            }

            var shipVisit = mapper.Map<ShipVisit>(updateShipVisitRequestDto);
            shipVisit = await visitRepository.UpdateAsync(id, shipVisit);

            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            else
            {
                var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
                return Ok(shipVisitDto);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var shipVisit = await visitRepository.DeleteAsync(id);
            if (shipVisit == null)
            {
                return NotFound($"Ship visit with ID {id} not found.");
            }
            else
            {
                var shipVisitDto = mapper.Map<ShipVisitDto>(shipVisit);
                return Ok(shipVisitDto);
            }
        }
    }
}
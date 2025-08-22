using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository;



namespace LimanTakipSistemi.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipCrewAssignmentController : ControllerBase
    {
        private readonly IShipCrewAssignmentRepository assignmentRepository;
        private readonly IMapper mapper;

        public ShipCrewAssignmentController(IShipCrewAssignmentRepository assignmentRepository, IMapper mapper)
        {
            this.assignmentRepository = assignmentRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? assignmentId, [FromQuery] int? shipId,
        [FromQuery] int? crewId, [FromQuery] DateTime? assignmentDate,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var assignments = await assignmentRepository.GetAllAsync(assignmentId, shipId, crewId, assignmentDate, pageNumber = 1, pageSize = 100);
            var assignmentsDto = mapper.Map<List<ShipCrewAssignmentDto>>(assignments);
            return Ok(assignmentsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var assignment = await assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
            return Ok(assignmentDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddShipCrewAssignmentRequestDto addShipCrewAssignmentRequestDto)
        {
            if (addShipCrewAssignmentRequestDto == null)
            {
                return BadRequest("Assignment data is required.");
            }
            var assignment = mapper.Map<ShipCrewAssignment>(addShipCrewAssignmentRequestDto);
            assignment = await assignmentRepository.CreateAsync(assignment);
            var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
            return CreatedAtAction(nameof(GetById), new { id = assignment.AssignmentId }, assignmentDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateShipCrewAssignmentRequestDto updateShipCrewAssignmentRequestDto)
        {
            if (updateShipCrewAssignmentRequestDto == null)
            {
                return BadRequest("Assignment data is required.");
            }


            var assignment = mapper.Map<ShipCrewAssignment>(updateShipCrewAssignmentRequestDto);
            assignment = await assignmentRepository.UpdateAsync(id, assignment);
            if (assignment == null)
            {
                return NotFound();
            }
            else
            {
                var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
                return Ok(assignmentDto);
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var assignment = await assignmentRepository.DeleteAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            else
            {
                var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
                return Ok(assignmentDto);
            }

        }

    }
}
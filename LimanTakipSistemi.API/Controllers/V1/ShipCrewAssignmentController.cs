using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShipCrewAssignmentController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public ShipCrewAssignmentController(LimanTakipDbContext dbContext , IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var assignments = dbContext.ShipCrewAssignments.ToList();
            var assignmentsDto = mapper.Map<List<ShipCrewAssignmentDto>>(assignments);
            return Ok(assignmentsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var assignment = dbContext.ShipCrewAssignments.FirstOrDefault(a => a.AssignmentId == id);
            if (assignment == null)
            {
                return NotFound();
            }
            var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
            return Ok(assignmentDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddShipCrewAssignmentRequestDto addShipCrewAssignmentRequestDto)
        {
            if (addShipCrewAssignmentRequestDto == null)
            {
                return BadRequest("Assignment data is required.");
            }
            var assignment = mapper.Map<ShipCrewAssignment>(addShipCrewAssignmentRequestDto);
            dbContext.ShipCrewAssignments.Add(assignment);
            dbContext.SaveChanges();
            var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
            return CreatedAtAction(nameof(GetById), new { id = assignment.AssignmentId }, assignmentDto);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateShipCrewAssignmentRequestDto updateShipCrewAssignmentRequestDto)
        {
            if (updateShipCrewAssignmentRequestDto == null)
            {
                return BadRequest("Assignment data is required.");
            }
            var assignment = dbContext.ShipCrewAssignments.FirstOrDefault(a => a.AssignmentId == id);
            if (assignment == null)
            {
                return NotFound();
            }
            else
            {
                assignment.ShipId = updateShipCrewAssignmentRequestDto.ShipId;
                assignment.CrewId = updateShipCrewAssignmentRequestDto.CrewId;
                assignment.AssignmentDate = updateShipCrewAssignmentRequestDto.AssignmentDate;
                dbContext.SaveChanges();
                var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);
                return Ok(assignmentDto);
            }
           
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var assignment = dbContext.ShipCrewAssignments.FirstOrDefault(a => a.AssignmentId == id);
            if (assignment == null)
            {
                return NotFound();
            }
            else
            {
                dbContext.ShipCrewAssignments.Remove(assignment);
                dbContext.SaveChanges();
                var assignmentDto = mapper.Map<ShipCrewAssignmentDto>(assignment);  
                return Ok(assignmentDto);
            }
               
        }

    }
}

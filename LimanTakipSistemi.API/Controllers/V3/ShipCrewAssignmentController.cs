using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using LimanTakipSistemi.API.Services.ShipCrewAssignmentService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipCrewAssignmentController : ControllerBase
    {
        private readonly IShipCrewAssignmentService shipCrewAssignmentService;

        public ShipCrewAssignmentController(IShipCrewAssignmentService shipCrewAssignmentService)
        {
            this.shipCrewAssignmentService = shipCrewAssignmentService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? assignmentId = null,
            [FromQuery] int? shipId = null,
            [FromQuery] int? crewId = null,
            [FromQuery] DateTime? assignmentDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var assignments = await shipCrewAssignmentService.GetAllAsync(assignmentId, shipId, crewId, assignmentDate, pageNumber, pageSize);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving ship crew assignments", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var assignment = await shipCrewAssignmentService.GetByIdAsync(id);
                if (assignment == null)
                {
                    return NotFound(new { message = "Ship crew assignment not found" });
                }
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the ship crew assignment", error = ex.Message });
            }
        }

        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddShipCrewAssignmentRequestDto addShipCrewAssignmentRequestDto)
        {
            try
            {
                var assignmentDto = await shipCrewAssignmentService.CreateAsync(addShipCrewAssignmentRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = assignmentDto.AssignmentId }, assignmentDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the ship crew assignment", error = ex.Message });
            }
        }

       
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateShipCrewAssignmentRequestDto updateShipCrewAssignmentRequestDto)
        {
            try
            {
                var assignmentDto = await shipCrewAssignmentService.UpdateAsync(id, updateShipCrewAssignmentRequestDto);
                if (assignmentDto == null)
                {
                    return NotFound(new { message = "Ship crew assignment not found" });
                }
                return Ok(assignmentDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the ship crew assignment", error = ex.Message });
            }
        }

        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await shipCrewAssignmentService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Ship crew assignment not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the ship crew assignment", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("by-ship/{shipId:int}")]
        public async Task<IActionResult> GetAssignmentsByShip([FromRoute] int shipId)
        {
            try
            {
                var assignments = await shipCrewAssignmentService.GetAssignmentsByShipAsync(shipId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving assignments by ship", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("by-crew/{crewId:int}")]
        public async Task<IActionResult> GetAssignmentsByCrewMember([FromRoute] int crewId)
        {
            try
            {
                var assignments = await shipCrewAssignmentService.GetAssignmentsByCrewMemberAsync(crewId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving assignments by crew member", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("check-availability")]
        public async Task<IActionResult> CheckCrewMemberAvailability(
            [FromQuery] int crewId,
            [FromQuery] DateTime assignmentDate,
            [FromQuery] int? excludeAssignmentId = null)
        {
            try
            {
                var isAvailable = await shipCrewAssignmentService.IsCrewMemberAvailableAsync(crewId, assignmentDate, excludeAssignmentId);
                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking crew member availability", error = ex.Message });
            }
        }
    }
}

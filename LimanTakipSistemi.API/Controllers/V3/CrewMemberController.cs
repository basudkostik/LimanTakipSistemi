using Asp.Versioning;
using LimanTakipSistemi.API.CustomActionFilter;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using LimanTakipSistemi.API.Services.CrewMemberService;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V3
{
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CrewMemberController : ControllerBase
    {
        private readonly ICrewMemberService crewMemberService;

        public CrewMemberController(ICrewMemberService crewMemberService)
        {
            this.crewMemberService = crewMemberService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? crewId = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phoneNumber = null,
            [FromQuery] string? role = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var crewMembers = await crewMemberService.GetAllAsync(crewId, firstName, lastName, email, phoneNumber, role, pageNumber, pageSize);
                return Ok(crewMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving crew members", error = ex.Message });
            }
        }

        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var crewMember = await crewMemberService.GetByIdAsync(id);
                if (crewMember == null)
                {
                    return NotFound(new { message = "Crew member not found" });
                }
                return Ok(crewMember);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the crew member", error = ex.Message });
            }
        }

        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddCrewMemberRequestDto addCrewMemberRequestDto)
        {
            try
            {
                var crewMemberDto = await crewMemberService.CreateAsync(addCrewMemberRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = crewMemberDto.CrewId }, crewMemberDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the crew member", error = ex.Message });
            }
        }

        
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCrewMemberRequestDto updateCrewMemberRequestDto)
        {
            try
            {
                var crewMemberDto = await crewMemberService.UpdateAsync(id, updateCrewMemberRequestDto);
                if (crewMemberDto == null)
                {
                    return NotFound(new { message = "Crew member not found" });
                }
                return Ok(crewMemberDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the crew member", error = ex.Message });
            }
        }

       
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await crewMemberService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Crew member not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the crew member", error = ex.Message });
            }
        }

       
        [HttpGet]
        [Route("check-email")]
        public async Task<IActionResult> CheckEmailUnique([FromQuery] string email, [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await crewMemberService.IsEmailUniqueAsync(email, excludeId);
                return Ok(new { isUnique });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking email uniqueness", error = ex.Message });
            }
        }
    }
}

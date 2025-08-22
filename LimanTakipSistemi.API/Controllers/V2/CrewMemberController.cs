using Asp.Versioning;
using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using LimanTakipSistemi.API.Repositories.CrewMemberRepository;
using Microsoft.AspNetCore.Mvc;


namespace LimanTakipSistemi.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class CrewMemberController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICrewMemberRepository crewMemberRepository;

        public CrewMemberController(ICrewMemberRepository crewMemberRepository, IMapper mapper)
        {
            this.crewMemberRepository = crewMemberRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? crewId, [FromQuery] string? firstName, [FromQuery] string? lastName,
        [FromQuery] string? email, [FromQuery] string? phoneNumber, [FromQuery] string? role,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {

            var crewMembers = await crewMemberRepository.GetAllAsync(crewId, firstName, lastName, email, phoneNumber, role, pageNumber = 1, pageSize = 100);
            var crewMemberDto = mapper.Map<List<CrewMemberDto>>(crewMembers);
            return Ok(crewMemberDto);
        }


        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var crewMember = await crewMemberRepository.GetByIdAsync(id);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {
                var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);

                return Ok(crewMemberDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCrewMemberRequestDto addCrewMemberRequestDto)
        {
            var crewMember = mapper.Map<CrewMember>(addCrewMemberRequestDto);

            crewMember = await crewMemberRepository.CreateAsync(crewMember);

            var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);

            return CreatedAtAction(nameof(GetById), new { id = crewMemberDto.CrewId }, crewMemberDto);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCrewMemberRequestDto updateCrewMemberRequestDto)
        {

            var crewMember = mapper.Map<CrewMember>(updateCrewMemberRequestDto);
            crewMember = await crewMemberRepository.UpdateAsync(id, crewMember);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {

                var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);

                return Ok(crewMemberDto);
            }
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var crewMember = await crewMemberRepository.DeleteAsync(id);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {
                var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);
                return Ok(crewMemberDto);
            }

        }
    }

}

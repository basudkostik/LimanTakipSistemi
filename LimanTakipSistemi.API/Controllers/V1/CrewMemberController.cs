using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CrewMemberController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public CrewMemberController(LimanTakipDbContext dbContext , IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            var crewMembers = dbContext.CrewMembers.ToList();

            var crewMemberDto = new List<CrewMemberDto>();

            crewMemberDto = mapper.Map<List<CrewMemberDto>>(crewMembers);


            return Ok(crewMemberDto);
        }


        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var crewMember = dbContext.CrewMembers.FirstOrDefault(x => x.CrewId == id);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {
                var crewMemberDto =  mapper.Map<CrewMemberDto>(crewMember);

                return Ok(crewMemberDto);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddCrewMemberRequestDto addCrewMemberRequestDto)
        {
            var crewMember =  mapper.Map<CrewMember>(addCrewMemberRequestDto);

            dbContext.CrewMembers.Add(crewMember);
            dbContext.SaveChanges();

            var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);

            return CreatedAtAction(nameof(GetById), new { id = crewMemberDto.CrewId }, crewMemberDto);
        }


        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateCrewMemberRequestDto updateCrewMemberRequestDto)
        {
            var crewMember = dbContext.CrewMembers.FirstOrDefault(x => x.CrewId == id);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {
                crewMember.PhoneNumber = updateCrewMemberRequestDto.PhoneNumber;
                crewMember.FirstName = updateCrewMemberRequestDto.FirstName;
                crewMember.LastName = updateCrewMemberRequestDto.LastName;
                crewMember.Email = updateCrewMemberRequestDto.Email;
                crewMember.Role = updateCrewMemberRequestDto.Role;

                dbContext.SaveChanges();

                var crewMemberDto = mapper.Map<CrewMemberDto>(crewMember);

                return Ok(crewMemberDto);
            }
        }


        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete([FromRoute]int id)
        {
            var crewMember = dbContext.CrewMembers.FirstOrDefault(x => x.CrewId == id);

            if (crewMember == null)
            {
                return NotFound();
            }
            else
            {
                dbContext.CrewMembers.Remove(crewMember);
                dbContext.SaveChanges();

                var crewMemberDto =  mapper.Map<CrewMemberDto>(crewMember);

                return Ok(crewMemberDto);
            }

        }
    }
    
}

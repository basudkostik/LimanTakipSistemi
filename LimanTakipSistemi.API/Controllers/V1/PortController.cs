using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Port;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PortController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public PortController(LimanTakipDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var ports = dbContext.Ports.ToList();
            var portsDto = mapper.Map<List<PortDto>>(ports);
            return Ok(portsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var port = dbContext.Ports.FirstOrDefault(x => x.PortId == id);
            if (port == null)
            {
                return NotFound();
            }
            else
            {
                var portDto = mapper.Map<PortDto>(port);
                return Ok(portDto);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddPortRequestDto addPortRequestDto)
        {
            if (addPortRequestDto == null)
            {
                return BadRequest("Port data is required.");
            }
            else
            {

                var port = mapper.Map<Port>(addPortRequestDto);
                dbContext.Ports.Add(port);
                dbContext.SaveChanges();
                var portDto = mapper.Map<PortDto>(port);
                return CreatedAtAction(nameof(GetById), new { id = port.PortId }, portDto);
            }
        }


        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update([FromRoute]int id, [FromBody] UpdatePortRequestDto updatePortRequestDto)
        {
            if (updatePortRequestDto == null)
            {
                return BadRequest("Port data is required.");
            }
            var port = dbContext.Ports.FirstOrDefault(x => x.PortId == id);
            if (port == null)
            {
                return NotFound();
            }
            else
            {
                port.Name = updatePortRequestDto.Name;
                port.Country = updatePortRequestDto.Country;
                port.City = updatePortRequestDto.City;
                
                dbContext.SaveChanges();
                var portDto = mapper.Map<PortDto>(port);
                return Ok(portDto);
            }
                
        }

        [HttpDelete]
        [Route("{id:int}")] 
        public IActionResult Delete([FromRoute]int id)
        {
            var port = dbContext.Ports.FirstOrDefault(x => x.PortId == id);
            if (port == null)
            {
                return NotFound();
            }
            else
            {
                dbContext.Ports.Remove(port);
                dbContext.SaveChanges();
                
                var portDto = mapper.Map<PortDto>(port);
                return Ok(portDto);
            }
        }
    }
}

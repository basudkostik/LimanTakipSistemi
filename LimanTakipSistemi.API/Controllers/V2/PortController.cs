using Asp.Versioning;
using AutoMapper;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Port;
using LimanTakipSistemi.API.Repositories.PortRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace LimanTakipSistemi.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PortController : ControllerBase
    {

        private readonly IPortRepository portRepository;

        private readonly IMapper mapper;

        public PortController(IPortRepository portRepository, IMapper mapper)
        {
            this.portRepository = portRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? portId, [FromQuery] string? name, [FromQuery] string? country, [FromQuery] string? city, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var ports = await portRepository.GetAllAsync(portId, name, country, city, pageNumber = 1, pageSize = 100);
            var portsDto = mapper.Map<List<PortDto>>(ports);
            return Ok(portsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var port = await portRepository.GetByIdAsync(id);
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
        public async Task<IActionResult> Create([FromBody] AddPortRequestDto addPortRequestDto)
        {

            var port = mapper.Map<Port>(addPortRequestDto);
            port = await portRepository.CreateAsync(port);
            var portDto = mapper.Map<PortDto>(port);
            return CreatedAtAction(nameof(GetById), new { id = port.PortId }, portDto);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePortRequestDto updatePortRequestDto)
        {
            var port = mapper.Map<Port>(updatePortRequestDto);

            port = await portRepository.UpdateAsync(id, port);
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

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var port = await portRepository.DeleteAsync(id);
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
    }
}

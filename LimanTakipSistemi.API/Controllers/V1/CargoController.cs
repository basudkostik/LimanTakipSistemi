using AutoMapper;
using LimanTakipSistemi.API.Mapping;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LimanTakipSistemi.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CargoController : ControllerBase
    {
        private readonly LimanTakipDbContext dbContext;
        private readonly IMapper mapper;

        public CargoController(LimanTakipDbContext dbContext , IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var cargoes = dbContext.Cargoes.ToList();

            var cargoesDto = new List<CargoDto>();

           cargoesDto = mapper.Map<List<CargoDto>>(cargoes);

            return Ok(cargoesDto);
        }



        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            var cargo = dbContext.Cargoes.FirstOrDefault(x => x.CargoId == id);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                var cargoDto = mapper.Map<CargoDto>(cargo);


                return Ok(cargoDto);
            }
        }


        [HttpPost]
        public IActionResult Create([FromBody] AddCargoRequestDto addCargoRequestDto)
        {
            var cargo =  mapper.Map<Cargo>(addCargoRequestDto);

            dbContext.Cargoes.Add(cargo);
            dbContext.SaveChanges();

            var cargoDto =  mapper.Map<CargoDto>(cargo);    

            return CreatedAtAction(nameof(GetById), new { id = cargo.CargoId }, cargoDto);
        }


        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateCargoRequestDto updateCargoRequestDto)
        {
            var cargo = dbContext.Cargoes.FirstOrDefault(x => x.CargoId == id);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                cargo.Description = updateCargoRequestDto.Description;
                cargo.Weight = updateCargoRequestDto.Weight;
                cargo.ShipId = updateCargoRequestDto.ShipId;
                cargo.CargoType = updateCargoRequestDto.CargoType;

                dbContext.SaveChanges();

                var cargoDto = mapper.Map<CargoDto>(cargo);

                return Ok(cargoDto);
            }
        }


        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var cargo = dbContext.Cargoes.FirstOrDefault(x => x.CargoId == id);

            if (cargo == null)
            {
                return NotFound();
            }
            else
            {
                dbContext.Cargoes.Remove(cargo);
                dbContext.SaveChanges();

                var cargoDto =  mapper.Map<CargoDto>(cargo);


                return Ok(cargoDto);
            }

        }
    }
}

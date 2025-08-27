using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using LimanTakipSistemi.API.Repositories.CargoRepository;

namespace LimanTakipSistemi.API.Services.CargoService
{
    public class CargoService : ICargoService
    {
        private readonly ICargoRepository cargoRepository;
        private readonly IMapper mapper;

        public CargoService(ICargoRepository cargoRepository, IMapper mapper)
        {
            this.cargoRepository = cargoRepository;
            this.mapper = mapper;
        }

        public async Task<List<CargoDto>> GetAllAsync(string? cargoType = null, decimal? minWeight = null, decimal? maxWeight = null,
            string? description = null, int? shipId = null, int? cargoId = null, int pageNumber = 1, int pageSize = 100)
        {
            var cargos = await cargoRepository.GetAllAsync(cargoType, minWeight, maxWeight, description, shipId, cargoId, pageNumber, pageSize);
            return mapper.Map<List<CargoDto>>(cargos);
        }

        public async Task<CargoDto?> GetByIdAsync(int id)
        {
            var cargo = await cargoRepository.GetByIdAsync(id);
            if (cargo == null)
                return null;

            return mapper.Map<CargoDto>(cargo);
        }

        public async Task<CargoDto> CreateAsync(AddCargoRequestDto addCargoRequestDto)
        {
            
            var cargo = mapper.Map<Cargo>(addCargoRequestDto);
            var createdCargo = await cargoRepository.CreateAsync(cargo);
            return mapper.Map<CargoDto>(createdCargo);
        }

        public async Task<CargoDto?> UpdateAsync(int id, UpdateCargoRequestDto updateCargoRequestDto)
        {
            
            var cargo = mapper.Map<Cargo>(updateCargoRequestDto);
            var updatedCargo = await cargoRepository.UpdateAsync(id, cargo);
            
            if (updatedCargo == null)
                return null;

            return mapper.Map<CargoDto>(updatedCargo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedCargo = await cargoRepository.DeleteAsync(id);
            return deletedCargo != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var cargo = await cargoRepository.GetByIdAsync(id);
            return cargo != null;
        }
    }
}

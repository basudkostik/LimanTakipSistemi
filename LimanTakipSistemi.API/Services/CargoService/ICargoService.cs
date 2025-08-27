using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;

namespace LimanTakipSistemi.API.Services.CargoService
{
    public interface ICargoService
    {
        Task<List<CargoDto>> GetAllAsync(string? cargoType = null, decimal? minWeight = null, decimal? maxWeight = null,
            string? description = null, int? shipId = null, int? cargoId = null, int pageNumber = 1, int pageSize = 100);
        Task<CargoDto?> GetByIdAsync(int id);
        Task<CargoDto> CreateAsync(AddCargoRequestDto addCargoRequestDto);
        Task<CargoDto?> UpdateAsync(int id, UpdateCargoRequestDto updateCargoRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

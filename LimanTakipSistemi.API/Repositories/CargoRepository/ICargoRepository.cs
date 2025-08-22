using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.CargoRepository
{
    public interface ICargoRepository
    {
        Task<List<Cargo>> GetAllAsync(string? cargoType = null, decimal? minWeight = 0, decimal? maxWeight = null,
         string? description = null, int? shipId = null, int? cargoId = null, int pageNumber = 1, int pageSize = 100);
        Task<Cargo?> GetByIdAsync(int id);
        Task<Cargo> CreateAsync(Cargo cargo);
        Task<Cargo?> UpdateAsync(int id, Cargo cargo);
        Task<Cargo?> DeleteAsync(int id);
    }
}
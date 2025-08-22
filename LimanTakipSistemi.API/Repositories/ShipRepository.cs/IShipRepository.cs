using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.ShipRepository.cs
{
    public interface IShipRepository
    {
        Task<List<Ship>> GetAllAsync(int? shipId = null, string? name = null, string? IMO = null, string? type = null, string? flag = null, int? yearBuilt = null, int pageNumber = 1, int pageSize = 100);
        Task<Ship?> GetByIdAsync(int id);
        Task<Ship> CreateAsync(Ship ship);
        Task<Ship?> UpdateAsync(int id, Ship ship);
        Task<Ship?> DeleteAsync(int id);
    }
}
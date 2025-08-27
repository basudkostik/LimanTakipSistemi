using LimanTakipSistemi.API.Models.DTOs.Ship;

namespace LimanTakipSistemi.API.Services.ShipService
{
    public interface IShipService
    {
        Task<List<ShipDto>> GetAllAsync(int? shipId = null, string? name = null, string? IMO = null, 
            string? type = null, string? flag = null, int? yearBuilt = null, int pageNumber = 1, int pageSize = 100);
        Task<ShipDto?> GetByIdAsync(int id);
        Task<ShipDto> CreateAsync(AddShipRequestDto addShipRequestDto);
        Task<ShipDto?> UpdateAsync(int id, UpdateShipRequestDto updateShipRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsIMOUniqueAsync(string imo, int? excludeId = null);
    }
}

using LimanTakipSistemi.API.Models.DTOs.Port;

namespace LimanTakipSistemi.API.Services.PortService
{
    public interface IPortService
    {
        Task<List<PortDto>> GetAllAsync(int? portId = null, string? name = null, string? country = null, 
            string? city = null, int pageNumber = 1, int pageSize = 100);
        Task<PortDto?> GetByIdAsync(int id);
        Task<PortDto> CreateAsync(AddPortRequestDto addPortRequestDto);
        Task<PortDto?> UpdateAsync(int id, UpdatePortRequestDto updatePortRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsPortUniqueAsync(string name, string country, string city, int? excludeId = null);
    }
}

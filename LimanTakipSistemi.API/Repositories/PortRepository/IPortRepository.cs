using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.PortRepository
{
    public interface IPortRepository
    {
        Task<List<Port>> GetAllAsync(int? portId = null, string? name = null, string? country = null, string? city = null, int pageNumber = 1, int pageSize = 100);
        Task<Port?> GetByIdAsync(int id);
        Task<Port> CreateAsync(Port port);
        Task<Port?> UpdateAsync(int id, Port port);
        Task<Port?> DeleteAsync(int id);
    }
}
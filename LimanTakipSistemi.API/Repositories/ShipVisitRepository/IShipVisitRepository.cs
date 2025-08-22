using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.ShipVisitRepository
{
    public interface IShipVisitRepository
    {
        Task<List<ShipVisit>> GetAllAsync(int? visitId = null, int? shipId = null, int? portId = null,
        DateTime? arrivalDate = null, DateTime? departureDate = null, string? purpose = null, int pageNumber = 1, int pageSize = 100);
        Task<ShipVisit?> GetByIdAsync(int id);
        Task<ShipVisit> CreateAsync(ShipVisit shipVisit);
        Task<ShipVisit?> UpdateAsync(int id, ShipVisit shipVisit);
        Task<ShipVisit?> DeleteAsync(int id);
    }
}
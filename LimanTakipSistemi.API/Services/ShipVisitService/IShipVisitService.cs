using LimanTakipSistemi.API.Models.DTOs.ShipVisit;

namespace LimanTakipSistemi.API.Services.ShipVisitService
{
    public interface IShipVisitService
    {
        Task<List<ShipVisitDto>> GetAllAsync(int? visitId = null, int? shipId = null, int? portId = null,
            DateTime? arrivalDate = null, DateTime? departureDate = null, string? purpose = null, 
            int pageNumber = 1, int pageSize = 100);
        Task<ShipVisitDto?> GetByIdAsync(int id);
        Task<ShipVisitDto> CreateAsync(AddShipVisitRequestDto addShipVisitRequestDto);
        Task<ShipVisitDto?> UpdateAsync(int id, UpdateShipVisitRequestDto updateShipVisitRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsShipAvailableForVisitAsync(int shipId, DateTime arrivalDate, DateTime departureDate, int? excludeVisitId = null);
        Task<List<ShipVisitDto>> GetVisitsByShipAsync(int shipId);
        Task<List<ShipVisitDto>> GetVisitsByPortAsync(int portId);
        Task<List<ShipVisitDto>> GetActiveVisitsAsync();
        Task<List<ShipVisitDto>> GetUpcomingVisitsAsync();
    }
}

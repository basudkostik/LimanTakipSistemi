using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository
{
    public interface IShipCrewAssignmentRepository
    {
        Task<List<ShipCrewAssignment>> GetAllAsync(int? assignmentId = null, int? shipId = null, int? crewId = null, DateTime? assignmentDate = null, int pageNumber = 1, int pageSize = 100);
        Task<ShipCrewAssignment?> GetByIdAsync(int id);
        Task<ShipCrewAssignment> CreateAsync(ShipCrewAssignment shipCrewAssignment);
        Task<ShipCrewAssignment?> UpdateAsync(int id, ShipCrewAssignment shipCrewAssignment);
        Task<ShipCrewAssignment?> DeleteAsync(int id);
    }
}
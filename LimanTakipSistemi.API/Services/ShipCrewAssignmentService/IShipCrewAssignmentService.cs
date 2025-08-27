using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;

namespace LimanTakipSistemi.API.Services.ShipCrewAssignmentService
{
    public interface IShipCrewAssignmentService
    {
        Task<List<ShipCrewAssignmentDto>> GetAllAsync(int? assignmentId = null, int? shipId = null, int? crewId = null, 
            DateTime? assignmentDate = null, int pageNumber = 1, int pageSize = 100);
        Task<ShipCrewAssignmentDto?> GetByIdAsync(int id);
        Task<ShipCrewAssignmentDto> CreateAsync(AddShipCrewAssignmentRequestDto addShipCrewAssignmentRequestDto);
        Task<ShipCrewAssignmentDto?> UpdateAsync(int id, UpdateShipCrewAssignmentRequestDto updateShipCrewAssignmentRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsCrewMemberAvailableAsync(int crewId, DateTime assignmentDate, int? excludeAssignmentId = null);
        Task<List<ShipCrewAssignmentDto>> GetAssignmentsByShipAsync(int shipId);
        Task<List<ShipCrewAssignmentDto>> GetAssignmentsByCrewMemberAsync(int crewId);
    }
}

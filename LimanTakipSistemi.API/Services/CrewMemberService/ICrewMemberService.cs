using LimanTakipSistemi.API.Models.DTOs.CrewMember;

namespace LimanTakipSistemi.API.Services.CrewMemberService
{
    public interface ICrewMemberService
    {
        Task<List<CrewMemberDto>> GetAllAsync(int? crewId = null, string? firstName = null, string? lastName = null,
            string? email = null, string? phoneNumber = null, string? role = null, int pageNumber = 1, int pageSize = 100);
        Task<CrewMemberDto?> GetByIdAsync(int id);
        Task<CrewMemberDto> CreateAsync(AddCrewMemberRequestDto addCrewMemberRequestDto);
        Task<CrewMemberDto?> UpdateAsync(int id, UpdateCrewMemberRequestDto updateCrewMemberRequestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    }
}

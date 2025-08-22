using LimanTakipSistemi.API.Models.Domain;

namespace LimanTakipSistemi.API.Repositories.CrewMemberRepository
{
    public interface ICrewMemberRepository
    {
        Task<List<CrewMember>> GetAllAsync(int? crewId = null, string? firstName = null, string? lastName = null,
        string? email = null, string? phoneNumber = null, string? role = null,
        int pageNumber = 1, int pageSize = 100);
        Task<CrewMember?> GetByIdAsync(int id);
        Task<CrewMember> CreateAsync(CrewMember crewMember);
        Task<CrewMember?> UpdateAsync(int id, CrewMember crewMember);
        Task<CrewMember?> DeleteAsync(int id);


    }
}
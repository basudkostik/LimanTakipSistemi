using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using LimanTakipSistemi.API.Repositories.CrewMemberRepository;

namespace LimanTakipSistemi.API.Services.CrewMemberService
{
    public class CrewMemberService : ICrewMemberService
    {
        private readonly ICrewMemberRepository crewMemberRepository;
        private readonly IMapper mapper;

        public CrewMemberService(ICrewMemberRepository crewMemberRepository, IMapper mapper)
        {
            this.crewMemberRepository = crewMemberRepository;
            this.mapper = mapper;
        }

        public async Task<List<CrewMemberDto>> GetAllAsync(int? crewId = null, string? firstName = null, string? lastName = null,
            string? email = null, string? phoneNumber = null, string? role = null, int pageNumber = 1, int pageSize = 100)
        {
            var crewMembers = await crewMemberRepository.GetAllAsync(crewId, firstName, lastName, email, phoneNumber, role, pageNumber, pageSize);
            return mapper.Map<List<CrewMemberDto>>(crewMembers);
        }

        public async Task<CrewMemberDto?> GetByIdAsync(int id)
        {
            var crewMember = await crewMemberRepository.GetByIdAsync(id);
            if (crewMember == null)
                return null;

            return mapper.Map<CrewMemberDto>(crewMember);
        }

        public async Task<CrewMemberDto> CreateAsync(AddCrewMemberRequestDto addCrewMemberRequestDto)
        {
            // Business logic validation
            if (!await IsEmailUniqueAsync(addCrewMemberRequestDto.Email))
            {
                throw new InvalidOperationException("Email address already exists");
            }

            var crewMember = mapper.Map<CrewMember>(addCrewMemberRequestDto);
            var createdCrewMember = await crewMemberRepository.CreateAsync(crewMember);
            return mapper.Map<CrewMemberDto>(createdCrewMember);
        }

        public async Task<CrewMemberDto?> UpdateAsync(int id, UpdateCrewMemberRequestDto updateCrewMemberRequestDto)
        {
            // Business logic validation
            if (!await IsEmailUniqueAsync(updateCrewMemberRequestDto.Email, id))
            {
                throw new InvalidOperationException("Email address already exists");
            }

            var crewMember = mapper.Map<CrewMember>(updateCrewMemberRequestDto);
            var updatedCrewMember = await crewMemberRepository.UpdateAsync(id, crewMember);
            
            if (updatedCrewMember == null)
                return null;

            return mapper.Map<CrewMemberDto>(updatedCrewMember);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedCrewMember = await crewMemberRepository.DeleteAsync(id);
            return deletedCrewMember != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var crewMember = await crewMemberRepository.GetByIdAsync(id);
            return crewMember != null;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var existingMembers = await crewMemberRepository.GetAllAsync(email: email);
            
            if (excludeId.HasValue)
            {
                existingMembers = existingMembers.Where(cm => cm.CrewId != excludeId.Value).ToList();
            }
            
            return !existingMembers.Any();
        }
    }
}

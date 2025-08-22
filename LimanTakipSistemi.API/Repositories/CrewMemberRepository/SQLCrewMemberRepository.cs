
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.CrewMemberRepository
{
    public class SQLCrewMemberRepository(LimanTakipDbContext dbContext) : ICrewMemberRepository
    {
        private readonly LimanTakipDbContext dbContext = dbContext;


        public async Task<CrewMember> CreateAsync(CrewMember crewMember)
        {
            await dbContext.CrewMembers.AddAsync(crewMember);
            await dbContext.SaveChangesAsync();
            return crewMember;
        }

        public async Task<CrewMember?> DeleteAsync(int id)
        {
            var crewMember = await dbContext.CrewMembers.FirstOrDefaultAsync(x => x.CrewId == id);
            if (crewMember == null)
            {
                return null;
            }
            else
            {
                dbContext.CrewMembers.Remove(crewMember);
                await dbContext.SaveChangesAsync();
                return crewMember;
            }


        }

        public async Task<List<CrewMember>> GetAllAsync(int? crewId = null, string? firstName = null, string? lastName = null, string? email = null, string? phoneNumber = null, string? role = null, int pageNumber = 1, int pageSize = 100)
        {
            var crewMembers = dbContext.CrewMembers.AsQueryable();

            if (crewId.HasValue)
            {
                crewMembers = crewMembers.Where(c => c.CrewId == crewId);
            }
            if (string.IsNullOrWhiteSpace(firstName) == false)
            {
                crewMembers = crewMembers.Where(c => c.FirstName.Contains(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName) == false)
            {
                crewMembers = crewMembers.Where(c => c.LastName.Contains(lastName));
            }
            if (string.IsNullOrWhiteSpace(email) == false)
            {
                crewMembers = crewMembers.Where(c => c.Email.Contains(email));
            }
            if (string.IsNullOrWhiteSpace(phoneNumber) == false)
            {
                crewMembers = crewMembers.Where(c => c.PhoneNumber.Contains(phoneNumber));
            }
            if (string.IsNullOrWhiteSpace(role) == false)
            {
                crewMembers = crewMembers.Where(c => c.Role.Contains(role));
            }

            var skipResult = (pageNumber - 1) * pageSize;

            return await crewMembers.Skip(skipResult).Take(pageSize).ToListAsync();

        }




        public async Task<CrewMember?> GetByIdAsync(int id)
        {
            return await dbContext.CrewMembers.FirstOrDefaultAsync(c => c.CrewId == id);
        }

        public async Task<CrewMember?> UpdateAsync(int id, CrewMember crewMember)
        {
            var existingCrewMember = await dbContext.CrewMembers.FirstOrDefaultAsync(c => c.CrewId == id);
            if (existingCrewMember == null)
            {
                return null;
            }
            else
            {
                existingCrewMember.PhoneNumber = crewMember.PhoneNumber;
                existingCrewMember.FirstName = crewMember.FirstName;
                existingCrewMember.LastName = crewMember.LastName;
                existingCrewMember.Email = crewMember.Email;
                existingCrewMember.Role = crewMember.Role;

                await dbContext.SaveChangesAsync();
                return existingCrewMember;
            }
        }

    }
}
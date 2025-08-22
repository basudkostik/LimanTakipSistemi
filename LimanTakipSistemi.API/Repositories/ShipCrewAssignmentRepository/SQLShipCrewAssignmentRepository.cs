using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository
{
    public class SQLShipCrewAssignmentRepository : IShipCrewAssignmentRepository
    {
        private readonly LimanTakipDbContext dbContext;


        public SQLShipCrewAssignmentRepository(LimanTakipDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
        public async Task<ShipCrewAssignment> CreateAsync(ShipCrewAssignment shipCrewAssignment)
        {
            await dbContext.ShipCrewAssignments.AddAsync(shipCrewAssignment);
            await dbContext.SaveChangesAsync();
            return shipCrewAssignment;

        }

        public async Task<ShipCrewAssignment?> DeleteAsync(int id)
        {
            var shipCrewAssignment = await dbContext.ShipCrewAssignments.FirstOrDefaultAsync(p => p.AssignmentId == id);
            if (shipCrewAssignment == null)
            {
                return null;
            }
            else
            {
                dbContext.ShipCrewAssignments.Remove(shipCrewAssignment);
                await dbContext.SaveChangesAsync();
                return shipCrewAssignment;
            }
        }

        public async Task<List<ShipCrewAssignment>> GetAllAsync(int? assignmentId = null, int? shipId = null, int? crewId = null, DateTime? assignmentDate = null, int pageNumber = 1, int pageSize = 100)
        {
            var assignments = dbContext.ShipCrewAssignments.Include(a => a.Ship).Include(a => a.CrewMember).AsQueryable();

            if (assignmentId.HasValue)
            {
                assignments = assignments.Where(a => a.AssignmentId == assignmentId);
            }
            if (shipId.HasValue)
            {
                assignments = assignments.Where(a => a.ShipId == shipId);
            }
            if (shipId.HasValue)
            {
                assignments = assignments.Where(a => a.CrewId == crewId);
            }
            if (assignmentDate.HasValue)
            {
                assignments = assignments.Where(a => a.AssignmentDate.Date == assignmentDate.Value.Date);
            }

            var skipResult = (pageNumber - 1) * pageSize;

            return await assignments.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<ShipCrewAssignment?> GetByIdAsync(int id)
        {
            return await dbContext.ShipCrewAssignments.FirstOrDefaultAsync(c => c.AssignmentId == id);
        }

        public async Task<ShipCrewAssignment?> UpdateAsync(int id, ShipCrewAssignment shipCrewAssignment)
        {
            var existingAssignment = await dbContext.ShipCrewAssignments.FirstOrDefaultAsync(c => c.AssignmentId == id);
            if (existingAssignment == null)
            {
                return null;
            }
            else
            {
                existingAssignment.ShipId = shipCrewAssignment.ShipId;
                existingAssignment.CrewId = shipCrewAssignment.CrewId;
                existingAssignment.AssignmentDate = shipCrewAssignment.AssignmentDate;

                await dbContext.SaveChangesAsync();
                return existingAssignment;
            }
        }

    }
}
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.ShipVisitRepository
{
    public class SQLShipVisitRepository : IShipVisitRepository
    {

        private readonly LimanTakipDbContext dbContext;

        public SQLShipVisitRepository(LimanTakipDbContext dbContext)
        {

            this.dbContext = dbContext;
        }
        public async Task<ShipVisit> CreateAsync(ShipVisit shipVisit)
        {
            await dbContext.ShipVisits.AddAsync(shipVisit);
            await dbContext.SaveChangesAsync();
            return shipVisit;
        }

        public async Task<ShipVisit?> DeleteAsync(int id)
        {
            var shipVisit = await dbContext.ShipVisits.FirstOrDefaultAsync(p => p.VisitId == id);
            if (shipVisit == null)
            {
                return null;
            }
            else
            {
                dbContext.ShipVisits.Remove(shipVisit);
                await dbContext.SaveChangesAsync();
                return shipVisit;
            }
        }

        public async Task<List<ShipVisit>> GetAllAsync(int? visitId = null, int? shipId = null, int? portId = null, DateTime? arrivalDate = null, DateTime? departureDate = null, string? purpose = null, int pageNumber = 1, int pageSize = 100)
        {
            var visits = dbContext.ShipVisits.Include(a => a.Ship).Include(a => a.Port).AsQueryable();

            if (visitId.HasValue)
            {
                visits = visits.Where(a => a.VisitId == visitId);
            }
            if (shipId.HasValue)
            {
                visits = visits.Where(a => a.ShipId == shipId);
            }
            if (portId.HasValue)
            {
                visits = visits.Where(a => a.PortId == portId);
            }
            if (string.IsNullOrWhiteSpace(purpose) == false)
            {
                visits = visits.Where(c => c.Purpose.Contains(purpose));
            }
            if (arrivalDate.HasValue)
            {
                visits = visits.Where(a => a.ArrivalDate.Date == arrivalDate.Value.Date);
            }
            if (departureDate.HasValue)
            {
                visits = visits.Where(a => a.DepartureDate.Date == departureDate.Value.Date);
            }
            var skipResult = (pageNumber - 1) * pageSize;

            return await visits.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<ShipVisit?> GetByIdAsync(int id)
        {
            return await dbContext.ShipVisits.FirstOrDefaultAsync(c => c.VisitId == id);
        }

        public async Task<ShipVisit?> UpdateAsync(int id, ShipVisit shipVisit)
        {
            var existingVisit = await dbContext.ShipVisits.FirstOrDefaultAsync(c => c.VisitId == id);
            if (existingVisit == null)
            {
                return null;
            }
            else
            {
                existingVisit.ShipId = shipVisit.ShipId;
                existingVisit.PortId = shipVisit.PortId;
                existingVisit.ArrivalDate = shipVisit.ArrivalDate;
                existingVisit.DepartureDate = shipVisit.DepartureDate;
                existingVisit.Purpose = shipVisit.Purpose;

                await dbContext.SaveChangesAsync();
                return existingVisit;
            }
        }

    }
}
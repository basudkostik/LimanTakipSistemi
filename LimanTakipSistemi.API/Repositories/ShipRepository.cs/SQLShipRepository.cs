using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.ShipRepository.cs
{
    public class SQLShipRepository : IShipRepository
    {
        private readonly LimanTakipDbContext dbContext;

        public SQLShipRepository(LimanTakipDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
        public async Task<Ship> CreateAsync(Ship ship)
        {
            await dbContext.Ships.AddAsync(ship);
            await dbContext.SaveChangesAsync();
            return ship;
        }

        public async Task<Ship?> DeleteAsync(int id)
        {
            var ship = await dbContext.Ships.FirstOrDefaultAsync(s => s.ShipId == id);
            if (ship == null)
            {
                return null;
            }
            else
            {
                dbContext.Ships.Remove(ship);
                await dbContext.SaveChangesAsync();
                return ship;
            }
        }

        public async Task<List<Ship>> GetAllAsync(int? shipId = null, string? name = null, string? IMO = null, string? type = null, string? flag = null, int? yearBuilt = null, int pageNumber = 1, int pageSize = 100)
        {
            var ships = dbContext.Ships.AsQueryable();

            if (shipId.HasValue)
            {
                ships = ships.Where(s => s.ShipId == shipId);
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                ships = ships.Where(c => c.Name.Contains(name));
            }
            if (string.IsNullOrWhiteSpace(IMO) == false)
            {
                ships = ships.Where(c => c.IMO.Contains(IMO));
            }
            if (string.IsNullOrWhiteSpace(type) == false)
            {
                ships = ships.Where(c => c.Type.Contains(type));
            }
            if (string.IsNullOrWhiteSpace(flag) == false)
            {
                ships = ships.Where(c => c.Flag.Contains(flag));
            }
            if (yearBuilt.HasValue)
            {
                ships = ships.Where(s => s.YearBuilt == yearBuilt);
            }

            var skipResult = (pageNumber - 1) * pageSize;

            return await ships.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<Ship?> GetByIdAsync(int id)
        {
            return await dbContext.Ships.FirstOrDefaultAsync(c => c.ShipId == id);
        }

        public async Task<Ship?> UpdateAsync(int id, Ship ship)
        {
            var existingShip = await dbContext.Ships.FirstOrDefaultAsync(c => c.ShipId == id);
            if (existingShip == null)
            {
                return null;
            }
            else
            {
                existingShip.Name = ship.Name;
                existingShip.IMO = ship.IMO;
                existingShip.Type = ship.Type;
                existingShip.Flag = ship.Flag;
                existingShip.YearBuilt = ship.YearBuilt;

                await dbContext.SaveChangesAsync();
                return existingShip;
            }
        }

    }
}
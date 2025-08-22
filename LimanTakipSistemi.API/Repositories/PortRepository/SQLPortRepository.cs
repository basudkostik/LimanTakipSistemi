using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.PortRepository
{
    public class SQLPortRepository : IPortRepository
    {
        private readonly LimanTakipDbContext dbContext;


        public SQLPortRepository(LimanTakipDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
        public async Task<Port> CreateAsync(Port port)
        {
            await dbContext.Ports.AddAsync(port);
            await dbContext.SaveChangesAsync();
            return port;
        }

        public async Task<Port?> DeleteAsync(int id)
        {
            var port = await dbContext.Ports.FirstOrDefaultAsync(p => p.PortId == id);
            if (port == null)
            {
                return null;
            }
            else
            {
                dbContext.Ports.Remove(port);
                await dbContext.SaveChangesAsync();
                return port;
            }
        }

        public async Task<List<Port>> GetAllAsync(int? portId = null, string? name = null, string? country = null, string? city = null, int pageNumber = 1, int pageSize = 100)
        {
            var ports = dbContext.Ports.AsQueryable();

            if (portId.HasValue)
            {
                ports = ports.Where(p => p.PortId == portId);
            }
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                ports = ports.Where(c => c.Name.Contains(name));
            }
            if (string.IsNullOrWhiteSpace(country) == false)
            {
                ports = ports.Where(c => c.Country.Contains(country));
            }
            if (string.IsNullOrWhiteSpace(city) == false)
            {
                ports = ports.Where(c => c.City.Contains(city));
            }
            var skipResult = (pageNumber - 1) * pageSize;

            return await ports.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<Port?> GetByIdAsync(int id)
        {
            return await dbContext.Ports.FirstOrDefaultAsync(c => c.PortId == id);
        }

        public async Task<Port?> UpdateAsync(int id, Port port)
        {
            var existingPort = await dbContext.Ports.FirstOrDefaultAsync(c => c.PortId == id);
            if (existingPort == null)
            {
                return null;
            }
            else
            {
                existingPort.City = port.City;
                existingPort.Country = port.Country;
                existingPort.Name = port.Name;

                await dbContext.SaveChangesAsync();
                return existingPort;
            }
        }

    }
}
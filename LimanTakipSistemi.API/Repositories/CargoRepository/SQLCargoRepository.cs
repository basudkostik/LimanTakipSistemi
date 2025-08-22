using System.IO.Compression;
using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Repositories.CargoRepository
{
    public class SQLCargoRepository : ICargoRepository
    {
        private readonly LimanTakipDbContext dbContext;


        public SQLCargoRepository(LimanTakipDbContext dbContext)
        {
            this.dbContext = dbContext;

        }

        public async Task<List<Cargo>> GetAllAsync(string? cargoType = null, decimal? minWeight = 0, decimal? maxWeight = null,
         string? description = null, int? shipId = null, int? cargoId = null, int pageNumber = 1, int pageSize = 100)
        {
            var cargoes = dbContext.Cargoes.Include(c => c.Ship).AsQueryable();

            if (string.IsNullOrWhiteSpace(cargoType) == false)
            {
                cargoes = cargoes.Where(c => c.CargoType.Contains(cargoType));
            }
            if (minWeight.HasValue)
            {
                cargoes = cargoes.Where(c => c.Weight >= minWeight.Value);
            }

            if (maxWeight.HasValue)
            {
                cargoes = cargoes.Where(c => c.Weight <= maxWeight.Value);
            }
            if (string.IsNullOrWhiteSpace(description) == false)
            {
                cargoes = cargoes.Where(c => c.Description.Contains(description));
            }
            if (shipId.HasValue)
            {
                cargoes = cargoes.Where(c => c.ShipId == shipId);
            }
            if (cargoId.HasValue)
            {
                cargoes = cargoes.Where(c => c.CargoId == cargoId);
            }

            var skipResult = (pageNumber - 1) * pageSize;

            return await cargoes.Skip(skipResult).Take(pageSize).ToListAsync();

        }

        public async Task<Cargo?> GetByIdAsync(int id)
        {
            return await dbContext.Cargoes.FirstOrDefaultAsync(c => c.CargoId == id);
        }


        public async Task<Cargo> CreateAsync(Cargo cargo)
        {
            await dbContext.Cargoes.AddAsync(cargo);
            await dbContext.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo?> UpdateAsync(int id, Cargo cargo)
        {
            var existingCargo = await dbContext.Cargoes.FirstOrDefaultAsync(c => c.CargoId == id);

            if (existingCargo == null)
            {
                return null;
            }
            else
            {
                existingCargo.Description = cargo.Description;
                existingCargo.Weight = cargo.Weight;
                existingCargo.ShipId = cargo.ShipId;
                existingCargo.CargoType = cargo.CargoType;

                await dbContext.SaveChangesAsync();

                return existingCargo;
            }

        }



        public async Task<Cargo?> DeleteAsync(int id)
        {
            var cargo = await dbContext.Cargoes.FirstOrDefaultAsync(c => c.CargoId == id);

            if (cargo == null)
            {
                return null;
            }
            else
            {
                dbContext.Cargoes.Remove(cargo);
                await dbContext.SaveChangesAsync();
                return cargo;
            }
        }






    }

}
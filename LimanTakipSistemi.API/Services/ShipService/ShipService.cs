using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Repositories.ShipRepository.cs;

namespace LimanTakipSistemi.API.Services.ShipService
{
    public class ShipService : IShipService
    {
        private readonly IShipRepository shipRepository;
        private readonly IMapper mapper;

        public ShipService(IShipRepository shipRepository, IMapper mapper)
        {
            this.shipRepository = shipRepository;
            this.mapper = mapper;
        }

        public async Task<List<ShipDto>> GetAllAsync(int? shipId = null, string? name = null, string? IMO = null, 
            string? type = null, string? flag = null, int? yearBuilt = null, int pageNumber = 1, int pageSize = 100)
        {
            var ships = await shipRepository.GetAllAsync(shipId, name, IMO, type, flag, yearBuilt, pageNumber, pageSize);
            return mapper.Map<List<ShipDto>>(ships);
        }

        public async Task<ShipDto?> GetByIdAsync(int id)
        {
            var ship = await shipRepository.GetByIdAsync(id);
            if (ship == null)
                return null;

            return mapper.Map<ShipDto>(ship);
        }

        public async Task<ShipDto> CreateAsync(AddShipRequestDto addShipRequestDto)
        {
            // Business logic validation
            if (!await IsIMOUniqueAsync(addShipRequestDto.IMO))
            {
                throw new InvalidOperationException("IMO number already exists");
            }

            var ship = mapper.Map<Ship>(addShipRequestDto);
            var createdShip = await shipRepository.CreateAsync(ship);
            return mapper.Map<ShipDto>(createdShip);
        }

        public async Task<ShipDto?> UpdateAsync(int id, UpdateShipRequestDto updateShipRequestDto)
        {
            // Business logic validation
            if (!await IsIMOUniqueAsync(updateShipRequestDto.IMO, id))
            {
                throw new InvalidOperationException("IMO number already exists");
            }

            var ship = mapper.Map<Ship>(updateShipRequestDto);
            var updatedShip = await shipRepository.UpdateAsync(id, ship);
            
            if (updatedShip == null)
                return null;

            return mapper.Map<ShipDto>(updatedShip);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedShip = await shipRepository.DeleteAsync(id);
            return deletedShip != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var ship = await shipRepository.GetByIdAsync(id);
            return ship != null;
        }

        public async Task<bool> IsIMOUniqueAsync(string imo, int? excludeId = null)
        {
            var existingShips = await shipRepository.GetAllAsync(IMO: imo);
            
            if (excludeId.HasValue)
            {
                existingShips = existingShips.Where(s => s.ShipId != excludeId.Value).ToList();
            }
            
            return !existingShips.Any();
        }
    }
}

using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;
using LimanTakipSistemi.API.Repositories.ShipVisitRepository;
using LimanTakipSistemi.API.Services.ShipService;
using LimanTakipSistemi.API.Services.PortService;

namespace LimanTakipSistemi.API.Services.ShipVisitService
{
    public class ShipVisitService : IShipVisitService
    {
        private readonly IShipVisitRepository shipVisitRepository;
        private readonly IShipService shipService;
        private readonly IPortService portService;
        private readonly IMapper mapper;

        public ShipVisitService(
            IShipVisitRepository shipVisitRepository,
            IShipService shipService,
            IPortService portService,
            IMapper mapper)
        {
            this.shipVisitRepository = shipVisitRepository;
            this.shipService = shipService;
            this.portService = portService;
            this.mapper = mapper;
        }

        public async Task<List<ShipVisitDto>> GetAllAsync(int? visitId = null, int? shipId = null, int? portId = null,
            DateTime? arrivalDate = null, DateTime? departureDate = null, string? purpose = null, 
            int pageNumber = 1, int pageSize = 100)
        {
            var visits = await shipVisitRepository.GetAllAsync(visitId, shipId, portId, arrivalDate, departureDate, purpose, pageNumber, pageSize);
            return mapper.Map<List<ShipVisitDto>>(visits);
        }

        public async Task<ShipVisitDto?> GetByIdAsync(int id)
        {
            var visit = await shipVisitRepository.GetByIdAsync(id);
            if (visit == null)
                return null;

            return mapper.Map<ShipVisitDto>(visit);
        }

        public async Task<ShipVisitDto> CreateAsync(AddShipVisitRequestDto addShipVisitRequestDto)
        {
            // Business logic validations
            
            // 1. Check if ship exists
            if (!await shipService.ExistsAsync(addShipVisitRequestDto.ShipId))
            {
                throw new InvalidOperationException("Ship does not exist");
            }

            // 2. Check if port exists
            if (!await portService.ExistsAsync(addShipVisitRequestDto.PortId))
            {
                throw new InvalidOperationException("Port does not exist");
            }

            // 3. Validate dates
            if (addShipVisitRequestDto.ArrivalDate >= addShipVisitRequestDto.DepartureDate)
            {
                throw new InvalidOperationException("Departure date must be after arrival date");
            }

            // 4. Check if ship is available for the visit period
            if (!await IsShipAvailableForVisitAsync(addShipVisitRequestDto.ShipId, addShipVisitRequestDto.ArrivalDate, addShipVisitRequestDto.DepartureDate))
            {
                throw new InvalidOperationException("Ship is not available during the specified period");
            }

            var visit = mapper.Map<ShipVisit>(addShipVisitRequestDto);
            var createdVisit = await shipVisitRepository.CreateAsync(visit);
            return mapper.Map<ShipVisitDto>(createdVisit);
        }

        public async Task<ShipVisitDto?> UpdateAsync(int id, UpdateShipVisitRequestDto updateShipVisitRequestDto)
        {
            // Business logic validations
            
            // 1. Check if visit exists
            if (!await ExistsAsync(id))
            {
                return null;
            }

            // 2. Check if ship exists
            if (!await shipService.ExistsAsync(updateShipVisitRequestDto.ShipId))
            {
                throw new InvalidOperationException("Ship does not exist");
            }

            // 3. Check if port exists
            if (!await portService.ExistsAsync(updateShipVisitRequestDto.PortId))
            {
                throw new InvalidOperationException("Port does not exist");
            }

            // 4. Validate dates
            if (updateShipVisitRequestDto.ArrivalDate >= updateShipVisitRequestDto.DepartureDate)
            {
                throw new InvalidOperationException("Departure date must be after arrival date");
            }

            // 5. Check if ship is available for the visit period (excluding current visit)
            if (!await IsShipAvailableForVisitAsync(updateShipVisitRequestDto.ShipId, updateShipVisitRequestDto.ArrivalDate, updateShipVisitRequestDto.DepartureDate, id))
            {
                throw new InvalidOperationException("Ship is not available during the specified period");
            }

            var visit = mapper.Map<ShipVisit>(updateShipVisitRequestDto);
            var updatedVisit = await shipVisitRepository.UpdateAsync(id, visit);
            
            if (updatedVisit == null)
                return null;

            return mapper.Map<ShipVisitDto>(updatedVisit);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedVisit = await shipVisitRepository.DeleteAsync(id);
            return deletedVisit != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var visit = await shipVisitRepository.GetByIdAsync(id);
            return visit != null;
        }

        public async Task<bool> IsShipAvailableForVisitAsync(int shipId, DateTime arrivalDate, DateTime departureDate, int? excludeVisitId = null)
        {
            var existingVisits = await shipVisitRepository.GetAllAsync(shipId: shipId);
            
            if (excludeVisitId.HasValue)
            {
                existingVisits = existingVisits.Where(v => v.VisitId != excludeVisitId.Value).ToList();
            }

            // Check for overlapping periods
            foreach (var visit in existingVisits)
            {
                if ((arrivalDate < visit.DepartureDate) && (departureDate > visit.ArrivalDate))
                {
                    return false; // Overlapping period found
                }
            }

            return true;
        }

        public async Task<List<ShipVisitDto>> GetVisitsByShipAsync(int shipId)
        {
            var visits = await shipVisitRepository.GetAllAsync(shipId: shipId);
            return mapper.Map<List<ShipVisitDto>>(visits);
        }

        public async Task<List<ShipVisitDto>> GetVisitsByPortAsync(int portId)
        {
            var visits = await shipVisitRepository.GetAllAsync(portId: portId);
            return mapper.Map<List<ShipVisitDto>>(visits);
        }

        public async Task<List<ShipVisitDto>> GetActiveVisitsAsync()
        {
            var allVisits = await shipVisitRepository.GetAllAsync();
            var now = DateTime.UtcNow;
            
            var activeVisits = allVisits.Where(v => v.ArrivalDate <= now && v.DepartureDate > now).ToList();
            return mapper.Map<List<ShipVisitDto>>(activeVisits);
        }

        public async Task<List<ShipVisitDto>> GetUpcomingVisitsAsync()
        {
            var allVisits = await shipVisitRepository.GetAllAsync();
            var now = DateTime.UtcNow;
            
            var upcomingVisits = allVisits.Where(v => v.ArrivalDate > now).ToList();
            return mapper.Map<List<ShipVisitDto>>(upcomingVisits);
        }
    }
}

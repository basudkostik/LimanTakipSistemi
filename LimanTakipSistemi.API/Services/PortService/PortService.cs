using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Port;
using LimanTakipSistemi.API.Repositories.PortRepository;

namespace LimanTakipSistemi.API.Services.PortService
{
    public class PortService : IPortService
    {
        private readonly IPortRepository portRepository;
        private readonly IMapper mapper;

        public PortService(IPortRepository portRepository, IMapper mapper)
        {
            this.portRepository = portRepository;
            this.mapper = mapper;
        }

        public async Task<List<PortDto>> GetAllAsync(int? portId = null, string? name = null, string? country = null, 
            string? city = null, int pageNumber = 1, int pageSize = 100)
        {
            var ports = await portRepository.GetAllAsync(portId, name, country, city, pageNumber, pageSize);
            return mapper.Map<List<PortDto>>(ports);
        }

        public async Task<PortDto?> GetByIdAsync(int id)
        {
            var port = await portRepository.GetByIdAsync(id);
            if (port == null)
                return null;

            return mapper.Map<PortDto>(port);
        }

        public async Task<PortDto> CreateAsync(AddPortRequestDto addPortRequestDto)
        {
            // Business logic validation
            if (!await IsPortUniqueAsync(addPortRequestDto.Name, addPortRequestDto.Country, addPortRequestDto.City))
            {
                throw new InvalidOperationException("Port with same name, country and city already exists");
            }

            var port = mapper.Map<Port>(addPortRequestDto);
            var createdPort = await portRepository.CreateAsync(port);
            return mapper.Map<PortDto>(createdPort);
        }

        public async Task<PortDto?> UpdateAsync(int id, UpdatePortRequestDto updatePortRequestDto)
        {
            // Business logic validation
            if (!await IsPortUniqueAsync(updatePortRequestDto.Name, updatePortRequestDto.Country, updatePortRequestDto.City, id))
            {
                throw new InvalidOperationException("Port with same name, country and city already exists");
            }

            var port = mapper.Map<Port>(updatePortRequestDto);
            var updatedPort = await portRepository.UpdateAsync(id, port);
            
            if (updatedPort == null)
                return null;

            return mapper.Map<PortDto>(updatedPort);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedPort = await portRepository.DeleteAsync(id);
            return deletedPort != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var port = await portRepository.GetByIdAsync(id);
            return port != null;
        }

        public async Task<bool> IsPortUniqueAsync(string name, string country, string city, int? excludeId = null)
        {
            var existingPorts = await portRepository.GetAllAsync(name: name, country: country, city: city);
            
            if (excludeId.HasValue)
            {
                existingPorts = existingPorts.Where(p => p.PortId != excludeId.Value).ToList();
            }
            
            return !existingPorts.Any();
        }
    }
}

using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository;
using LimanTakipSistemi.API.Services.ShipService;
using LimanTakipSistemi.API.Services.CrewMemberService;

namespace LimanTakipSistemi.API.Services.ShipCrewAssignmentService
{
    public class ShipCrewAssignmentService : IShipCrewAssignmentService
    {
        private readonly IShipCrewAssignmentRepository shipCrewAssignmentRepository;
        private readonly IShipService shipService;
        private readonly ICrewMemberService crewMemberService;
        private readonly IMapper mapper;

        public ShipCrewAssignmentService(
            IShipCrewAssignmentRepository shipCrewAssignmentRepository, 
            IShipService shipService,
            ICrewMemberService crewMemberService,
            IMapper mapper)
        {
            this.shipCrewAssignmentRepository = shipCrewAssignmentRepository;
            this.shipService = shipService;
            this.crewMemberService = crewMemberService;
            this.mapper = mapper;
        }

        public async Task<List<ShipCrewAssignmentDto>> GetAllAsync(int? assignmentId = null, int? shipId = null, int? crewId = null, 
            DateTime? assignmentDate = null, int pageNumber = 1, int pageSize = 100)
        {
            var assignments = await shipCrewAssignmentRepository.GetAllAsync(assignmentId, shipId, crewId, assignmentDate, pageNumber, pageSize);
            return mapper.Map<List<ShipCrewAssignmentDto>>(assignments);
        }

        public async Task<ShipCrewAssignmentDto?> GetByIdAsync(int id)
        {
            var assignment = await shipCrewAssignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return null;

            return mapper.Map<ShipCrewAssignmentDto>(assignment);
        }

        public async Task<ShipCrewAssignmentDto> CreateAsync(AddShipCrewAssignmentRequestDto addShipCrewAssignmentRequestDto)
        {
            // Business logic validations
            
            // 1. Check if ship exists
            if (!await shipService.ExistsAsync(addShipCrewAssignmentRequestDto.ShipId))
            {
                throw new InvalidOperationException("Ship does not exist");
            }

            // 2. Check if crew member exists
            if (!await crewMemberService.ExistsAsync(addShipCrewAssignmentRequestDto.CrewId))
            {
                throw new InvalidOperationException("Crew member does not exist");
            }

            // 3. Check if crew member is available on the assignment date
            if (!await IsCrewMemberAvailableAsync(addShipCrewAssignmentRequestDto.CrewId, addShipCrewAssignmentRequestDto.AssignmentDate))
            {
                throw new InvalidOperationException("Crew member is already assigned to another ship on this date");
            }

            var assignment = mapper.Map<ShipCrewAssignment>(addShipCrewAssignmentRequestDto);
            var createdAssignment = await shipCrewAssignmentRepository.CreateAsync(assignment);
            return mapper.Map<ShipCrewAssignmentDto>(createdAssignment);
        }

        public async Task<ShipCrewAssignmentDto?> UpdateAsync(int id, UpdateShipCrewAssignmentRequestDto updateShipCrewAssignmentRequestDto)
        {
            // Business logic validations
            
            // 1. Check if assignment exists
            if (!await ExistsAsync(id))
            {
                return null;
            }

            // 2. Check if ship exists
            if (!await shipService.ExistsAsync(updateShipCrewAssignmentRequestDto.ShipId))
            {
                throw new InvalidOperationException("Ship does not exist");
            }

            // 3. Check if crew member exists
            if (!await crewMemberService.ExistsAsync(updateShipCrewAssignmentRequestDto.CrewId))
            {
                throw new InvalidOperationException("Crew member does not exist");
            }

            // 4. Check if crew member is available on the assignment date (excluding current assignment)
            if (!await IsCrewMemberAvailableAsync(updateShipCrewAssignmentRequestDto.CrewId, updateShipCrewAssignmentRequestDto.AssignmentDate, id))
            {
                throw new InvalidOperationException("Crew member is already assigned to another ship on this date");
            }

            var assignment = mapper.Map<ShipCrewAssignment>(updateShipCrewAssignmentRequestDto);
            var updatedAssignment = await shipCrewAssignmentRepository.UpdateAsync(id, assignment);
            
            if (updatedAssignment == null)
                return null;

            return mapper.Map<ShipCrewAssignmentDto>(updatedAssignment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deletedAssignment = await shipCrewAssignmentRepository.DeleteAsync(id);
            return deletedAssignment != null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var assignment = await shipCrewAssignmentRepository.GetByIdAsync(id);
            return assignment != null;
        }

        public async Task<bool> IsCrewMemberAvailableAsync(int crewId, DateTime assignmentDate, int? excludeAssignmentId = null)
        {
            var existingAssignments = await shipCrewAssignmentRepository.GetAllAsync(crewId: crewId, assignmentDate: assignmentDate);
            
            if (excludeAssignmentId.HasValue)
            {
                existingAssignments = existingAssignments.Where(a => a.AssignmentId != excludeAssignmentId.Value).ToList();
            }
            
            return !existingAssignments.Any();
        }

        public async Task<List<ShipCrewAssignmentDto>> GetAssignmentsByShipAsync(int shipId)
        {
            var assignments = await shipCrewAssignmentRepository.GetAllAsync(shipId: shipId);
            return mapper.Map<List<ShipCrewAssignmentDto>>(assignments);
        }

        public async Task<List<ShipCrewAssignmentDto>> GetAssignmentsByCrewMemberAsync(int crewId)
        {
            var assignments = await shipCrewAssignmentRepository.GetAllAsync(crewId: crewId);
            return mapper.Map<List<ShipCrewAssignmentDto>>(assignments);
        }
    }
}

using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository;
using LimanTakipSistemi.API.Services.ShipService;
using LimanTakipSistemi.API.Services.CrewMemberService;
using LimanTakipSistemi.API.Services.ShipCrewAssignmentService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class ShipCrewAssignmentServiceTests : TestBase
    {
        private readonly Mock<IShipCrewAssignmentRepository> _mockAssignmentRepository;
        private readonly Mock<IShipService> _mockShipService;
        private readonly Mock<ICrewMemberService> _mockCrewMemberService;
        private readonly ShipCrewAssignmentService _assignmentService;

        public ShipCrewAssignmentServiceTests()
        {
            _mockAssignmentRepository = new Mock<IShipCrewAssignmentRepository>();
            _mockShipService = new Mock<IShipService>();
            _mockCrewMemberService = new Mock<ICrewMemberService>();
            _assignmentService = new ShipCrewAssignmentService(
                _mockAssignmentRepository.Object,
                _mockShipService.Object,
                _mockCrewMemberService.Object,
                Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAssignmentDtoList_WhenAssignmentsExist()
        {
            // Arrange
            var assignments = new List<ShipCrewAssignment>
            {
                new ShipCrewAssignment
                {
                    AssignmentId = 1,
                    ShipId = 1,
                    CrewId = 1,
                    AssignmentDate = DateTime.UtcNow
                },
                new ShipCrewAssignment
                {
                    AssignmentId = 2,
                    ShipId = 2,
                    CrewId = 2,
                    AssignmentDate = DateTime.UtcNow.AddDays(1)
                }
            };

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, null, null, 1, 100))
                .ReturnsAsync(assignments);

            // Act
            var result = await _assignmentService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].ShipId);
            Assert.Equal(1, result[0].CrewId);
            
            _mockAssignmentRepository.Verify(x => x.GetAllAsync(null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var assignments = new List<ShipCrewAssignment>();
            var assignmentDate = DateTime.UtcNow;

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(1, 1, 1, assignmentDate, 1, 50))
                .ReturnsAsync(assignments);

            // Act
            await _assignmentService.GetAllAsync(1, 1, 1, assignmentDate, 1, 50);

            // Assert
            _mockAssignmentRepository.Verify(x => x.GetAllAsync(1, 1, 1, assignmentDate, 1, 50), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsAssignmentDto_WhenAllValidationsPass()
        {
            // Arrange
            var addAssignmentRequest = new AddShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = DateTime.UtcNow
            };

            var createdAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = addAssignmentRequest.AssignmentDate
            };

            // Setup validations
            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockCrewMemberService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, addAssignmentRequest.AssignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment>());

            _mockAssignmentRepository.Setup(x => x.CreateAsync(It.IsAny<ShipCrewAssignment>()))
                .ReturnsAsync(createdAssignment);

            // Act
            var result = await _assignmentService.CreateAsync(addAssignmentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ShipId);
            Assert.Equal(1, result.CrewId);
            _mockAssignmentRepository.Verify(x => x.CreateAsync(It.IsAny<ShipCrewAssignment>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenShipDoesNotExist()
        {
            // Arrange
            var addAssignmentRequest = new AddShipCrewAssignmentRequestDto
            {
                ShipId = 999,
                CrewId = 1,
                AssignmentDate = DateTime.UtcNow
            };

            _mockShipService.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _assignmentService.CreateAsync(addAssignmentRequest));

            Assert.Equal("Ship does not exist", exception.Message);
            _mockAssignmentRepository.Verify(x => x.CreateAsync(It.IsAny<ShipCrewAssignment>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenCrewMemberDoesNotExist()
        {
            // Arrange
            var addAssignmentRequest = new AddShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 999,
                AssignmentDate = DateTime.UtcNow
            };

            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockCrewMemberService.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _assignmentService.CreateAsync(addAssignmentRequest));

            Assert.Equal("Crew member does not exist", exception.Message);
            _mockAssignmentRepository.Verify(x => x.CreateAsync(It.IsAny<ShipCrewAssignment>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenCrewMemberNotAvailable()
        {
            // Arrange
            var assignmentDate = DateTime.UtcNow;
            var addAssignmentRequest = new AddShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = assignmentDate
            };

            var existingAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                ShipId = 2,
                CrewId = 1,
                AssignmentDate = assignmentDate // Same crew member, same date
            };

            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockCrewMemberService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, assignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment> { existingAssignment });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _assignmentService.CreateAsync(addAssignmentRequest));

            Assert.Equal("Crew member is already assigned to another ship on this date", exception.Message);
            _mockAssignmentRepository.Verify(x => x.CreateAsync(It.IsAny<ShipCrewAssignment>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsAssignmentDto_WhenAllValidationsPass()
        {
            // Arrange
            var updateAssignmentRequest = new UpdateShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = DateTime.UtcNow
            };

            var updatedAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = updateAssignmentRequest.AssignmentDate
            };

            // Setup validations
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(updatedAssignment);
            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockCrewMemberService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, updateAssignmentRequest.AssignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment>());

            _mockAssignmentRepository.Setup(x => x.UpdateAsync(1, It.IsAny<ShipCrewAssignment>()))
                .ReturnsAsync(updatedAssignment);

            // Act
            var result = await _assignmentService.UpdateAsync(1, updateAssignmentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ShipId);
            Assert.Equal(1, result.CrewId);
            _mockAssignmentRepository.Verify(x => x.UpdateAsync(1, It.IsAny<ShipCrewAssignment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var updateAssignmentRequest = new UpdateShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = DateTime.UtcNow
            };

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((ShipCrewAssignment)null);

            // Act
            var result = await _assignmentService.UpdateAsync(999, updateAssignmentRequest);

            // Assert
            Assert.Null(result);
            _mockAssignmentRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ShipCrewAssignment>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsInvalidOperationException_WhenCrewMemberNotAvailableForUpdate()
        {
            // Arrange
            var assignmentDate = DateTime.UtcNow;
            var updateAssignmentRequest = new UpdateShipCrewAssignmentRequestDto
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = assignmentDate
            };

            var existingAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = assignmentDate
            };

            var conflictingAssignment = new ShipCrewAssignment
            {
                AssignmentId = 2, // Different assignment ID
                ShipId = 2,
                CrewId = 1,
                AssignmentDate = assignmentDate // Same crew member, same date
            };

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingAssignment);
            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockCrewMemberService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, assignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment> { conflictingAssignment });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _assignmentService.UpdateAsync(1, updateAssignmentRequest));

            Assert.Equal("Crew member is already assigned to another ship on this date", exception.Message);
            _mockAssignmentRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ShipCrewAssignment>()), Times.Never);
        }

        #endregion

        #region IsCrewMemberAvailableAsync Tests

        [Fact]
        public async Task IsCrewMemberAvailableAsync_ReturnsTrue_WhenNoConflictingAssignments()
        {
            // Arrange
            var assignmentDate = DateTime.UtcNow;

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, assignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment>());

            // Act
            var result = await _assignmentService.IsCrewMemberAvailableAsync(1, assignmentDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsCrewMemberAvailableAsync_ReturnsFalse_WhenConflictingAssignmentExists()
        {
            // Arrange
            var assignmentDate = DateTime.UtcNow;
            var existingAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                CrewId = 1,
                AssignmentDate = assignmentDate
            };

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, assignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment> { existingAssignment });

            // Act
            var result = await _assignmentService.IsCrewMemberAvailableAsync(1, assignmentDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCrewMemberAvailableAsync_ReturnsTrue_WhenConflictingAssignmentIsExcluded()
        {
            // Arrange
            var assignmentDate = DateTime.UtcNow;
            var existingAssignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                CrewId = 1,
                AssignmentDate = assignmentDate
            };

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, assignmentDate, 1, 100))
                .ReturnsAsync(new List<ShipCrewAssignment> { existingAssignment });

            // Act
            var result = await _assignmentService.IsCrewMemberAvailableAsync(1, assignmentDate, 1); // Exclude assignment 1

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Specialized Query Tests

        [Fact]
        public async Task GetAssignmentsByShipAsync_ReturnsAssignmentsForSpecificShip()
        {
            // Arrange
            var assignments = new List<ShipCrewAssignment>
            {
                new ShipCrewAssignment { AssignmentId = 1, ShipId = 1, CrewId = 1 },
                new ShipCrewAssignment { AssignmentId = 2, ShipId = 1, CrewId = 2 }
            };

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, 1, null, null, 1, 100))
                .ReturnsAsync(assignments);

            // Act
            var result = await _assignmentService.GetAssignmentsByShipAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(1, a.ShipId));
        }

        [Fact]
        public async Task GetAssignmentsByCrewMemberAsync_ReturnsAssignmentsForSpecificCrewMember()
        {
            // Arrange
            var assignments = new List<ShipCrewAssignment>
            {
                new ShipCrewAssignment { AssignmentId = 1, ShipId = 1, CrewId = 1 },
                new ShipCrewAssignment { AssignmentId = 2, ShipId = 2, CrewId = 1 }
            };

            _mockAssignmentRepository.Setup(x => x.GetAllAsync(null, null, 1, null, 1, 100))
                .ReturnsAsync(assignments);

            // Act
            var result = await _assignmentService.GetAssignmentsByCrewMemberAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(1, a.CrewId));
        }

        #endregion

        #region Standard CRUD Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsAssignmentDto_WhenAssignmentExists()
        {
            // Arrange
            var assignment = new ShipCrewAssignment
            {
                AssignmentId = 1,
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = DateTime.UtcNow
            };

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(assignment);

            // Act
            var result = await _assignmentService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AssignmentId);
            Assert.Equal(1, result.ShipId);
            Assert.Equal(1, result.CrewId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenAssignmentDoesNotExist()
        {
            // Arrange
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((ShipCrewAssignment)null);

            // Act
            var result = await _assignmentService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenAssignmentIsDeleted()
        {
            // Arrange
            var deletedAssignment = new ShipCrewAssignment { AssignmentId = 1 };
            _mockAssignmentRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedAssignment);

            // Act
            var result = await _assignmentService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockAssignmentRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenAssignmentDoesNotExist()
        {
            // Arrange
            _mockAssignmentRepository.Setup(x => x.DeleteAsync(999))
                .ReturnsAsync((ShipCrewAssignment)null);

            // Act
            var result = await _assignmentService.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenAssignmentExists()
        {
            // Arrange
            var assignment = new ShipCrewAssignment { AssignmentId = 1 };
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(assignment);

            // Act
            var result = await _assignmentService.ExistsAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenAssignmentDoesNotExist()
        {
            // Arrange
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((ShipCrewAssignment)null);

            // Act
            var result = await _assignmentService.ExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}

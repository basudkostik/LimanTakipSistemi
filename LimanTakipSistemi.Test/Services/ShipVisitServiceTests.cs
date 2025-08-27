using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;
using LimanTakipSistemi.API.Repositories.ShipVisitRepository;
using LimanTakipSistemi.API.Services.ShipService;
using LimanTakipSistemi.API.Services.PortService;
using LimanTakipSistemi.API.Services.ShipVisitService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class ShipVisitServiceTests : TestBase
    {
        private readonly Mock<IShipVisitRepository> _mockShipVisitRepository;
        private readonly Mock<IShipService> _mockShipService;
        private readonly Mock<IPortService> _mockPortService;
        private readonly ShipVisitService _shipVisitService;

        public ShipVisitServiceTests()
        {
            _mockShipVisitRepository = new Mock<IShipVisitRepository>();
            _mockShipService = new Mock<IShipService>();
            _mockPortService = new Mock<IPortService>();
            _shipVisitService = new ShipVisitService(
                _mockShipVisitRepository.Object,
                _mockShipService.Object,
                _mockPortService.Object,
                Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsShipVisitDtoList_WhenVisitsExist()
        {
            // Arrange
            var visits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ShipId = 1,
                    PortId = 1,
                    ArrivalDate = DateTime.UtcNow.AddDays(-1),
                    DepartureDate = DateTime.UtcNow.AddDays(1),
                    Purpose = "Loading cargo"
                },
                new ShipVisit
                {
                    VisitId = 2,
                    ShipId = 2,
                    PortId = 2,
                    ArrivalDate = DateTime.UtcNow.AddDays(1),
                    DepartureDate = DateTime.UtcNow.AddDays(3),
                    Purpose = "Unloading cargo"
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(visits);

            // Act
            var result = await _shipVisitService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Loading cargo", result[0].Purpose);
            Assert.Equal("Unloading cargo", result[1].Purpose);
            
            _mockShipVisitRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var visits = new List<ShipVisit>();
            var arrivalDate = DateTime.UtcNow;
            var departureDate = DateTime.UtcNow.AddDays(1);

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(1, 1, 1, arrivalDate, departureDate, "Loading", 1, 50))
                .ReturnsAsync(visits);

            // Act
            await _shipVisitService.GetAllAsync(1, 1, 1, arrivalDate, departureDate, "Loading", 1, 50);

            // Assert
            _mockShipVisitRepository.Verify(x => x.GetAllAsync(1, 1, 1, arrivalDate, departureDate, "Loading", 1, 50), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsShipVisitDto_WhenAllValidationsPass()
        {
            // Arrange
            var addVisitRequest = new AddShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Loading cargo"
            };

            var createdVisit = new ShipVisit
            {
                VisitId = 1,
                ShipId = 1,
                PortId = 1,
                ArrivalDate = addVisitRequest.ArrivalDate,
                DepartureDate = addVisitRequest.DepartureDate,
                Purpose = "Loading cargo"
            };

            // Setup validations
            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockPortService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(new List<ShipVisit>());

            _mockShipVisitRepository.Setup(x => x.CreateAsync(It.IsAny<ShipVisit>()))
                .ReturnsAsync(createdVisit);

            // Act
            var result = await _shipVisitService.CreateAsync(addVisitRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Loading cargo", result.Purpose);
            Assert.Equal(1, result.ShipId);
            Assert.Equal(1, result.PortId);
            _mockShipVisitRepository.Verify(x => x.CreateAsync(It.IsAny<ShipVisit>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenShipDoesNotExist()
        {
            // Arrange
            var addVisitRequest = new AddShipVisitRequestDto
            {
                ShipId = 999,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Loading cargo"
            };

            _mockShipService.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipVisitService.CreateAsync(addVisitRequest));

            Assert.Equal("Ship does not exist", exception.Message);
            _mockShipVisitRepository.Verify(x => x.CreateAsync(It.IsAny<ShipVisit>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenPortDoesNotExist()
        {
            // Arrange
            var addVisitRequest = new AddShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 999,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Loading cargo"
            };

            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockPortService.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipVisitService.CreateAsync(addVisitRequest));

            Assert.Equal("Port does not exist", exception.Message);
            _mockShipVisitRepository.Verify(x => x.CreateAsync(It.IsAny<ShipVisit>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenDepartureDateIsBeforeArrivalDate()
        {
            // Arrange
            var addVisitRequest = new AddShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(3),
                DepartureDate = DateTime.UtcNow.AddDays(1), // Before arrival
                Purpose = "Loading cargo"
            };

            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockPortService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipVisitService.CreateAsync(addVisitRequest));

            Assert.Equal("Departure date must be after arrival date", exception.Message);
            _mockShipVisitRepository.Verify(x => x.CreateAsync(It.IsAny<ShipVisit>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenShipIsNotAvailable()
        {
            // Arrange
            var addVisitRequest = new AddShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Loading cargo"
            };

            var existingVisit = new ShipVisit
            {
                VisitId = 1,
                ShipId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(0),
                DepartureDate = DateTime.UtcNow.AddDays(2) // Overlaps with new visit
            };

            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockPortService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(new List<ShipVisit> { existingVisit });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipVisitService.CreateAsync(addVisitRequest));

            Assert.Equal("Ship is not available during the specified period", exception.Message);
            _mockShipVisitRepository.Verify(x => x.CreateAsync(It.IsAny<ShipVisit>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsShipVisitDto_WhenAllValidationsPass()
        {
            // Arrange
            var updateVisitRequest = new UpdateShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Updated purpose"
            };

            var updatedVisit = new ShipVisit
            {
                VisitId = 1,
                ShipId = 1,
                PortId = 1,
                ArrivalDate = updateVisitRequest.ArrivalDate,
                DepartureDate = updateVisitRequest.DepartureDate,
                Purpose = "Updated purpose"
            };

            // Setup validations
            _mockShipVisitRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(updatedVisit);
            _mockShipService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockPortService.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(new List<ShipVisit>());

            _mockShipVisitRepository.Setup(x => x.UpdateAsync(1, It.IsAny<ShipVisit>()))
                .ReturnsAsync(updatedVisit);

            // Act
            var result = await _shipVisitService.UpdateAsync(1, updateVisitRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated purpose", result.Purpose);
            _mockShipVisitRepository.Verify(x => x.UpdateAsync(1, It.IsAny<ShipVisit>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenVisitDoesNotExist()
        {
            // Arrange
            var updateVisitRequest = new UpdateShipVisitRequestDto
            {
                ShipId = 1,
                PortId = 1,
                ArrivalDate = DateTime.UtcNow.AddDays(1),
                DepartureDate = DateTime.UtcNow.AddDays(3),
                Purpose = "Updated purpose"
            };

            _mockShipVisitRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((ShipVisit)null);

            // Act
            var result = await _shipVisitService.UpdateAsync(999, updateVisitRequest);

            // Assert
            Assert.Null(result);
            _mockShipVisitRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ShipVisit>()), Times.Never);
        }

        #endregion

        #region IsShipAvailableForVisitAsync Tests

        [Fact]
        public async Task IsShipAvailableForVisitAsync_ReturnsTrue_WhenNoOverlappingVisits()
        {
            // Arrange
            var arrivalDate = DateTime.UtcNow.AddDays(1);
            var departureDate = DateTime.UtcNow.AddDays(3);

            var existingVisits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ShipId = 1,
                    ArrivalDate = DateTime.UtcNow.AddDays(-2),
                    DepartureDate = DateTime.UtcNow.AddDays(0) // Ends before new visit starts
                },
                new ShipVisit
                {
                    VisitId = 2,
                    ShipId = 1,
                    ArrivalDate = DateTime.UtcNow.AddDays(4),
                    DepartureDate = DateTime.UtcNow.AddDays(6) // Starts after new visit ends
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(existingVisits);

            // Act
            var result = await _shipVisitService.IsShipAvailableForVisitAsync(1, arrivalDate, departureDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsShipAvailableForVisitAsync_ReturnsFalse_WhenOverlappingVisitExists()
        {
            // Arrange
            var arrivalDate = DateTime.UtcNow.AddDays(1);
            var departureDate = DateTime.UtcNow.AddDays(3);

            var existingVisits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ShipId = 1,
                    ArrivalDate = DateTime.UtcNow.AddDays(0),
                    DepartureDate = DateTime.UtcNow.AddDays(2) // Overlaps with new visit
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(existingVisits);

            // Act
            var result = await _shipVisitService.IsShipAvailableForVisitAsync(1, arrivalDate, departureDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsShipAvailableForVisitAsync_ReturnsTrue_WhenOverlappingVisitIsExcluded()
        {
            // Arrange
            var arrivalDate = DateTime.UtcNow.AddDays(1);
            var departureDate = DateTime.UtcNow.AddDays(3);

            var existingVisits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ShipId = 1,
                    ArrivalDate = DateTime.UtcNow.AddDays(0),
                    DepartureDate = DateTime.UtcNow.AddDays(2) // Would overlap but is excluded
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(existingVisits);

            // Act
            var result = await _shipVisitService.IsShipAvailableForVisitAsync(1, arrivalDate, departureDate, 1); // Exclude visit 1

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Specialized Query Tests

        [Fact]
        public async Task GetVisitsByShipAsync_ReturnsVisitsForSpecificShip()
        {
            // Arrange
            var visits = new List<ShipVisit>
            {
                new ShipVisit { VisitId = 1, ShipId = 1, Purpose = "Visit 1" },
                new ShipVisit { VisitId = 2, ShipId = 1, Purpose = "Visit 2" }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, 1, null, null, null, null, 1, 100))
                .ReturnsAsync(visits);

            // Act
            var result = await _shipVisitService.GetVisitsByShipAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, v => Assert.Equal(1, v.ShipId));
        }

        [Fact]
        public async Task GetVisitsByPortAsync_ReturnsVisitsForSpecificPort()
        {
            // Arrange
            var visits = new List<ShipVisit>
            {
                new ShipVisit { VisitId = 1, PortId = 1, Purpose = "Visit 1" },
                new ShipVisit { VisitId = 2, PortId = 1, Purpose = "Visit 2" }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, null, 1, null, null, null, 1, 100))
                .ReturnsAsync(visits);

            // Act
            var result = await _shipVisitService.GetVisitsByPortAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, v => Assert.Equal(1, v.PortId));
        }

        [Fact]
        public async Task GetActiveVisitsAsync_ReturnsOnlyCurrentlyActiveVisits()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var visits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ArrivalDate = now.AddDays(-1),
                    DepartureDate = now.AddDays(1), // Currently active
                    Purpose = "Active visit"
                },
                new ShipVisit
                {
                    VisitId = 2,
                    ArrivalDate = now.AddDays(1),
                    DepartureDate = now.AddDays(3), // Future visit
                    Purpose = "Future visit"
                },
                new ShipVisit
                {
                    VisitId = 3,
                    ArrivalDate = now.AddDays(-3),
                    DepartureDate = now.AddDays(-1), // Past visit
                    Purpose = "Past visit"
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(visits);

            // Act
            var result = await _shipVisitService.GetActiveVisitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Active visit", result[0].Purpose);
        }

        [Fact]
        public async Task GetUpcomingVisitsAsync_ReturnsOnlyFutureVisits()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var visits = new List<ShipVisit>
            {
                new ShipVisit
                {
                    VisitId = 1,
                    ArrivalDate = now.AddDays(-1),
                    DepartureDate = now.AddDays(1), // Currently active
                    Purpose = "Active visit"
                },
                new ShipVisit
                {
                    VisitId = 2,
                    ArrivalDate = now.AddDays(1),
                    DepartureDate = now.AddDays(3), // Future visit
                    Purpose = "Future visit"
                }
            };

            _mockShipVisitRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(visits);

            // Act
            var result = await _shipVisitService.GetUpcomingVisitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Future visit", result[0].Purpose);
        }

        #endregion

        #region Standard CRUD Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsShipVisitDto_WhenVisitExists()
        {
            // Arrange
            var visit = new ShipVisit
            {
                VisitId = 1,
                ShipId = 1,
                PortId = 1,
                Purpose = "Test visit"
            };

            _mockShipVisitRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(visit);

            // Act
            var result = await _shipVisitService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.VisitId);
            Assert.Equal("Test visit", result.Purpose);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenVisitIsDeleted()
        {
            // Arrange
            var deletedVisit = new ShipVisit { VisitId = 1, Purpose = "Deleted visit" };
            _mockShipVisitRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedVisit);

            // Act
            var result = await _shipVisitService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockShipVisitRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenVisitExists()
        {
            // Arrange
            var visit = new ShipVisit { VisitId = 1, Purpose = "Test visit" };
            _mockShipVisitRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(visit);

            // Act
            var result = await _shipVisitService.ExistsAsync(1);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}

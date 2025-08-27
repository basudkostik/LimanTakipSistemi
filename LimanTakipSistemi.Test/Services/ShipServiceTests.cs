using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Repositories.ShipRepository.cs;
using LimanTakipSistemi.API.Services.ShipService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class ShipServiceTests : TestBase
    {
        private readonly Mock<IShipRepository> _mockShipRepository;
        private readonly ShipService _shipService;

        public ShipServiceTests()
        {
            _mockShipRepository = new Mock<IShipRepository>();
            _shipService = new ShipService(_mockShipRepository.Object, Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsShipDtoList_WhenShipsExist()
        {
            // Arrange
            var ships = new List<Ship>
            {
                new Ship
                {
                    ShipId = 1,
                    Name = "Test Ship 1",
                    IMO = "IMO1234567",
                    Type = "Container",
                    Flag = "Turkey",
                    YearBuilt = 2020
                },
                new Ship
                {
                    ShipId = 2,
                    Name = "Test Ship 2",
                    IMO = "IMO7654321",
                    Type = "Tanker",
                    Flag = "Greece",
                    YearBuilt = 2018
                }
            };

            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(ships);

            // Act
            var result = await _shipService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Ship 1", result[0].Name);
            Assert.Equal("IMO1234567", result[0].IMO);
            Assert.Equal("Test Ship 2", result[1].Name);
            
            _mockShipRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoShipsExist()
        {
            // Arrange
            var ships = new List<Ship>();
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(ships);

            // Act
            var result = await _shipService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockShipRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var ships = new List<Ship>();
            _mockShipRepository.Setup(x => x.GetAllAsync(1, "Test", "IMO123", "Container", "Turkey", 2020, 1, 50))
                .ReturnsAsync(ships);

            // Act
            await _shipService.GetAllAsync(1, "Test", "IMO123", "Container", "Turkey", 2020, 1, 50);

            // Assert
            _mockShipRepository.Verify(x => x.GetAllAsync(1, "Test", "IMO123", "Container", "Turkey", 2020, 1, 50), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsShipDto_WhenShipExists()
        {
            // Arrange
            var ship = new Ship
            {
                ShipId = 1,
                Name = "Test Ship",
                IMO = "IMO1234567",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2020
            };

            _mockShipRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(ship);

            // Act
            var result = await _shipService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ShipId);
            Assert.Equal("Test Ship", result.Name);
            Assert.Equal("IMO1234567", result.IMO);
            _mockShipRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenShipDoesNotExist()
        {
            // Arrange
            _mockShipRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Ship)null);

            // Act
            var result = await _shipService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockShipRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsShipDto_WhenIMOIsUnique()
        {
            // Arrange
            var addShipRequest = new AddShipRequestDto
            {
                Name = "New Ship",
                IMO = "IMO1111111",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            var createdShip = new Ship
            {
                ShipId = 1,
                Name = "New Ship",
                IMO = "IMO1111111",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            // Setup IMO uniqueness check
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1111111", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship>());

            _mockShipRepository.Setup(x => x.CreateAsync(It.IsAny<Ship>()))
                .ReturnsAsync(createdShip);

            // Act
            var result = await _shipService.CreateAsync(addShipRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Ship", result.Name);
            Assert.Equal("IMO1111111", result.IMO);
            _mockShipRepository.Verify(x => x.CreateAsync(It.IsAny<Ship>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenIMOAlreadyExists()
        {
            // Arrange
            var addShipRequest = new AddShipRequestDto
            {
                Name = "New Ship",
                IMO = "IMO1234567",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            var existingShip = new Ship
            {
                ShipId = 2,
                Name = "Existing Ship",
                IMO = "IMO1234567",
                Type = "Tanker",
                Flag = "Greece",
                YearBuilt = 2020
            };

            // Setup IMO uniqueness check - return existing ship
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1234567", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship> { existingShip });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipService.CreateAsync(addShipRequest));

            Assert.Equal("IMO number already exists", exception.Message);
            _mockShipRepository.Verify(x => x.CreateAsync(It.IsAny<Ship>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsShipDto_WhenIMOIsUniqueAndShipExists()
        {
            // Arrange
            var updateShipRequest = new UpdateShipRequestDto
            {
                Name = "Updated Ship",
                IMO = "IMO2222222",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            var updatedShip = new Ship
            {
                ShipId = 1,
                Name = "Updated Ship",
                IMO = "IMO2222222",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            // Setup IMO uniqueness check
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO2222222", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship>());

            _mockShipRepository.Setup(x => x.UpdateAsync(1, It.IsAny<Ship>()))
                .ReturnsAsync(updatedShip);

            // Act
            var result = await _shipService.UpdateAsync(1, updateShipRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Ship", result.Name);
            Assert.Equal("IMO2222222", result.IMO);
            _mockShipRepository.Verify(x => x.UpdateAsync(1, It.IsAny<Ship>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsInvalidOperationException_WhenIMOExistsForDifferentShip()
        {
            // Arrange
            var updateShipRequest = new UpdateShipRequestDto
            {
                Name = "Updated Ship",
                IMO = "IMO1234567",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            var existingShip = new Ship
            {
                ShipId = 2, // Different ship ID
                Name = "Existing Ship",
                IMO = "IMO1234567",
                Type = "Tanker",
                Flag = "Greece",
                YearBuilt = 2020
            };

            // Setup IMO uniqueness check - return existing ship with different ID
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1234567", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship> { existingShip });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shipService.UpdateAsync(1, updateShipRequest));

            Assert.Equal("IMO number already exists", exception.Message);
            _mockShipRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Ship>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenShipDoesNotExist()
        {
            // Arrange
            var updateShipRequest = new UpdateShipRequestDto
            {
                Name = "Updated Ship",
                IMO = "IMO2222222",
                Type = "Container",
                Flag = "Turkey",
                YearBuilt = 2023
            };

            // Setup IMO uniqueness check
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO2222222", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship>());

            _mockShipRepository.Setup(x => x.UpdateAsync(999, It.IsAny<Ship>()))
                .ReturnsAsync((Ship)null);

            // Act
            var result = await _shipService.UpdateAsync(999, updateShipRequest);

            // Assert
            Assert.Null(result);
            _mockShipRepository.Verify(x => x.UpdateAsync(999, It.IsAny<Ship>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenShipIsDeleted()
        {
            // Arrange
            var deletedShip = new Ship { ShipId = 1, Name = "Deleted Ship" };
            _mockShipRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedShip);

            // Act
            var result = await _shipService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockShipRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenShipDoesNotExist()
        {
            // Arrange
            _mockShipRepository.Setup(x => x.DeleteAsync(999))
                .ReturnsAsync((Ship)null);

            // Act
            var result = await _shipService.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockShipRepository.Verify(x => x.DeleteAsync(999), Times.Once);
        }

        #endregion

        #region ExistsAsync Tests

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenShipExists()
        {
            // Arrange
            var ship = new Ship { ShipId = 1, Name = "Test Ship" };
            _mockShipRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(ship);

            // Act
            var result = await _shipService.ExistsAsync(1);

            // Assert
            Assert.True(result);
            _mockShipRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenShipDoesNotExist()
        {
            // Arrange
            _mockShipRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Ship)null);

            // Act
            var result = await _shipService.ExistsAsync(999);

            // Assert
            Assert.False(result);
            _mockShipRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region IsIMOUniqueAsync Tests

        [Fact]
        public async Task IsIMOUniqueAsync_ReturnsTrue_WhenIMOIsUnique()
        {
            // Arrange
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO9999999", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship>());

            // Act
            var result = await _shipService.IsIMOUniqueAsync("IMO9999999");

            // Assert
            Assert.True(result);
            _mockShipRepository.Verify(x => x.GetAllAsync(null, null, "IMO9999999", null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task IsIMOUniqueAsync_ReturnsFalse_WhenIMOExists()
        {
            // Arrange
            var existingShip = new Ship { ShipId = 1, IMO = "IMO1234567" };
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1234567", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship> { existingShip });

            // Act
            var result = await _shipService.IsIMOUniqueAsync("IMO1234567");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsIMOUniqueAsync_ReturnsTrue_WhenIMOExistsForExcludedShip()
        {
            // Arrange
            var existingShip = new Ship { ShipId = 1, IMO = "IMO1234567" };
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1234567", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship> { existingShip });

            // Act
            var result = await _shipService.IsIMOUniqueAsync("IMO1234567", 1); // Exclude the same ship

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsIMOUniqueAsync_ReturnsFalse_WhenIMOExistsForDifferentShip()
        {
            // Arrange
            var existingShip = new Ship { ShipId = 2, IMO = "IMO1234567" };
            _mockShipRepository.Setup(x => x.GetAllAsync(null, null, "IMO1234567", null, null, null, 1, 100))
                .ReturnsAsync(new List<Ship> { existingShip });

            // Act
            var result = await _shipService.IsIMOUniqueAsync("IMO1234567", 1); // Exclude different ship

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}

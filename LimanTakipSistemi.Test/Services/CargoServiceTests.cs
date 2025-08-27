using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using LimanTakipSistemi.API.Repositories.CargoRepository;
using LimanTakipSistemi.API.Services.CargoService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class CargoServiceTests : TestBase
    {
        private readonly Mock<ICargoRepository> _mockCargoRepository;
        private readonly CargoService _cargoService;

        public CargoServiceTests()
        {
            _mockCargoRepository = new Mock<ICargoRepository>();
            _cargoService = new CargoService(_mockCargoRepository.Object, Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsCargoDtoList_WhenCargosExist()
        {
            // Arrange
            var cargos = new List<Cargo>
            {
                new Cargo
                {
                    CargoId = 1,
                    ShipId = 1,
                    Description = "Container cargo",
                    Weight = 1500.50m,
                    CargoType = "Container"
                },
                new Cargo
                {
                    CargoId = 2,
                    ShipId = 2,
                    Description = "Bulk cargo",
                    Weight = 2300.75m,
                    CargoType = "Bulk"
                }
            };

            _mockCargoRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(cargos);

            // Act
            var result = await _cargoService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Container cargo", result[0].Description);
            Assert.Equal(1500.50m, result[0].Weight);
            Assert.Equal("Container", result[0].CargoType);
            Assert.Equal("Bulk cargo", result[1].Description);
            
            _mockCargoRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoCargosExist()
        {
            // Arrange
            var cargos = new List<Cargo>();
            _mockCargoRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(cargos);

            // Act
            var result = await _cargoService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCargoRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var cargos = new List<Cargo>();
            _mockCargoRepository.Setup(x => x.GetAllAsync("Container", 100m, 2000m, "test cargo", 1, 1, 1, 50))
                .ReturnsAsync(cargos);

            // Act
            await _cargoService.GetAllAsync("Container", 100m, 2000m, "test cargo", 1, 1, 1, 50);

            // Assert
            _mockCargoRepository.Verify(x => x.GetAllAsync("Container", 100m, 2000m, "test cargo", 1, 1, 1, 50), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsCargoDto_WhenCargoExists()
        {
            // Arrange
            var cargo = new Cargo
            {
                CargoId = 1,
                ShipId = 1,
                Description = "Container cargo",
                Weight = 1500.50m,
                CargoType = "Container"
            };

            _mockCargoRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(cargo);

            // Act
            var result = await _cargoService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CargoId);
            Assert.Equal(1, result.ShipId);
            Assert.Equal("Container cargo", result.Description);
            Assert.Equal(1500.50m, result.Weight);
            Assert.Equal("Container", result.CargoType);
            _mockCargoRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenCargoDoesNotExist()
        {
            // Arrange
            _mockCargoRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Cargo)null);

            // Act
            var result = await _cargoService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockCargoRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsCargoDto_WhenCargoIsCreated()
        {
            // Arrange
            var addCargoRequest = new AddCargoRequestDto
            {
                ShipId = 1,
                Description = "New container cargo",
                Weight = 1800.25m,
                CargoType = "Container"
            };

            var createdCargo = new Cargo
            {
                CargoId = 1,
                ShipId = 1,
                Description = "New container cargo",
                Weight = 1800.25m,
                CargoType = "Container"
            };

            _mockCargoRepository.Setup(x => x.CreateAsync(It.IsAny<Cargo>()))
                .ReturnsAsync(createdCargo);

            // Act
            var result = await _cargoService.CreateAsync(addCargoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New container cargo", result.Description);
            Assert.Equal(1800.25m, result.Weight);
            Assert.Equal("Container", result.CargoType);
            Assert.Equal(1, result.ShipId);
            _mockCargoRepository.Verify(x => x.CreateAsync(It.IsAny<Cargo>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryWithMappedCargo()
        {
            // Arrange
            var addCargoRequest = new AddCargoRequestDto
            {
                ShipId = 1,
                Description = "Test cargo",
                Weight = 1000m,
                CargoType = "Test"
            };

            var createdCargo = new Cargo { CargoId = 1 };

            _mockCargoRepository.Setup(x => x.CreateAsync(It.IsAny<Cargo>()))
                .ReturnsAsync(createdCargo);

            // Act
            await _cargoService.CreateAsync(addCargoRequest);

            // Assert
            _mockCargoRepository.Verify(x => x.CreateAsync(It.Is<Cargo>(c => 
                c.ShipId == 1 && 
                c.Description == "Test cargo" && 
                c.Weight == 1000m && 
                c.CargoType == "Test")), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsCargoDto_WhenCargoIsUpdated()
        {
            // Arrange
            var updateCargoRequest = new UpdateCargoRequestDto
            {
                ShipId = 1,
                Description = "Updated container cargo",
                Weight = 1900.75m,
                CargoType = "Container"
            };

            var updatedCargo = new Cargo
            {
                CargoId = 1,
                ShipId = 1,
                Description = "Updated container cargo",
                Weight = 1900.75m,
                CargoType = "Container"
            };

            _mockCargoRepository.Setup(x => x.UpdateAsync(1, It.IsAny<Cargo>()))
                .ReturnsAsync(updatedCargo);

            // Act
            var result = await _cargoService.UpdateAsync(1, updateCargoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated container cargo", result.Description);
            Assert.Equal(1900.75m, result.Weight);
            Assert.Equal("Container", result.CargoType);
            _mockCargoRepository.Verify(x => x.UpdateAsync(1, It.IsAny<Cargo>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenCargoDoesNotExist()
        {
            // Arrange
            var updateCargoRequest = new UpdateCargoRequestDto
            {
                ShipId = 1,
                Description = "Updated cargo",
                Weight = 1500m,
                CargoType = "Container"
            };

            _mockCargoRepository.Setup(x => x.UpdateAsync(999, It.IsAny<Cargo>()))
                .ReturnsAsync((Cargo)null);

            // Act
            var result = await _cargoService.UpdateAsync(999, updateCargoRequest);

            // Assert
            Assert.Null(result);
            _mockCargoRepository.Verify(x => x.UpdateAsync(999, It.IsAny<Cargo>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepositoryWithMappedCargo()
        {
            // Arrange
            var updateCargoRequest = new UpdateCargoRequestDto
            {
                ShipId = 2,
                Description = "Test updated cargo",
                Weight = 2000m,
                CargoType = "Bulk"
            };

            var updatedCargo = new Cargo { CargoId = 1 };

            _mockCargoRepository.Setup(x => x.UpdateAsync(1, It.IsAny<Cargo>()))
                .ReturnsAsync(updatedCargo);

            // Act
            await _cargoService.UpdateAsync(1, updateCargoRequest);

            // Assert
            _mockCargoRepository.Verify(x => x.UpdateAsync(1, It.Is<Cargo>(c => 
                c.ShipId == 2 && 
                c.Description == "Test updated cargo" && 
                c.Weight == 2000m && 
                c.CargoType == "Bulk")), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenCargoIsDeleted()
        {
            // Arrange
            var deletedCargo = new Cargo { CargoId = 1, Description = "Deleted cargo" };
            _mockCargoRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedCargo);

            // Act
            var result = await _cargoService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockCargoRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenCargoDoesNotExist()
        {
            // Arrange
            _mockCargoRepository.Setup(x => x.DeleteAsync(999))
                .ReturnsAsync((Cargo)null);

            // Act
            var result = await _cargoService.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockCargoRepository.Verify(x => x.DeleteAsync(999), Times.Once);
        }

        #endregion

        #region ExistsAsync Tests

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenCargoExists()
        {
            // Arrange
            var cargo = new Cargo { CargoId = 1, Description = "Test cargo" };
            _mockCargoRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(cargo);

            // Act
            var result = await _cargoService.ExistsAsync(1);

            // Assert
            Assert.True(result);
            _mockCargoRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenCargoDoesNotExist()
        {
            // Arrange
            _mockCargoRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Cargo)null);

            // Act
            var result = await _cargoService.ExistsAsync(999);

            // Assert
            Assert.False(result);
            _mockCargoRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion
    }
}

using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Port;
using LimanTakipSistemi.API.Repositories.PortRepository;
using LimanTakipSistemi.API.Services.PortService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class PortServiceTests : TestBase
    {
        private readonly Mock<IPortRepository> _mockPortRepository;
        private readonly PortService _portService;

        public PortServiceTests()
        {
            _mockPortRepository = new Mock<IPortRepository>();
            _portService = new PortService(_mockPortRepository.Object, Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsPortDtoList_WhenPortsExist()
        {
            // Arrange
            var ports = new List<Port>
            {
                new Port
                {
                    PortId = 1,
                    Name = "Istanbul Port",
                    Country = "Turkey",
                    City = "Istanbul"
                },
                new Port
                {
                    PortId = 2,
                    Name = "Piraeus Port",
                    Country = "Greece",
                    City = "Piraeus"
                }
            };

            _mockPortRepository.Setup(x => x.GetAllAsync(null, null, null, null, 1, 100))
                .ReturnsAsync(ports);

            // Act
            var result = await _portService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Istanbul Port", result[0].Name);
            Assert.Equal("Turkey", result[0].Country);
            Assert.Equal("Istanbul", result[0].City);
            
            _mockPortRepository.Verify(x => x.GetAllAsync(null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var ports = new List<Port>();
            _mockPortRepository.Setup(x => x.GetAllAsync(1, "Istanbul", "Turkey", "Istanbul", 1, 50))
                .ReturnsAsync(ports);

            // Act
            await _portService.GetAllAsync(1, "Istanbul", "Turkey", "Istanbul", 1, 50);

            // Assert
            _mockPortRepository.Verify(x => x.GetAllAsync(1, "Istanbul", "Turkey", "Istanbul", 1, 50), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsPortDto_WhenPortIsUnique()
        {
            // Arrange
            var addPortRequest = new AddPortRequestDto
            {
                Name = "New Port",
                Country = "Turkey",
                City = "Izmir"
            };

            var createdPort = new Port
            {
                PortId = 1,
                Name = "New Port",
                Country = "Turkey",
                City = "Izmir"
            };

            // Setup uniqueness check
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "New Port", "Turkey", "Izmir", 1, 100))
                .ReturnsAsync(new List<Port>());

            _mockPortRepository.Setup(x => x.CreateAsync(It.IsAny<Port>()))
                .ReturnsAsync(createdPort);

            // Act
            var result = await _portService.CreateAsync(addPortRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Port", result.Name);
            Assert.Equal("Turkey", result.Country);
            Assert.Equal("Izmir", result.City);
            _mockPortRepository.Verify(x => x.CreateAsync(It.IsAny<Port>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenPortAlreadyExists()
        {
            // Arrange
            var addPortRequest = new AddPortRequestDto
            {
                Name = "Istanbul Port",
                Country = "Turkey",
                City = "Istanbul"
            };

            var existingPort = new Port
            {
                PortId = 1,
                Name = "Istanbul Port",
                Country = "Turkey",
                City = "Istanbul"
            };

            // Setup uniqueness check - return existing port
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Istanbul Port", "Turkey", "Istanbul", 1, 100))
                .ReturnsAsync(new List<Port> { existingPort });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _portService.CreateAsync(addPortRequest));

            Assert.Equal("Port with same name, country and city already exists", exception.Message);
            _mockPortRepository.Verify(x => x.CreateAsync(It.IsAny<Port>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsPortDto_WhenPortIsUniqueAndExists()
        {
            // Arrange
            var updatePortRequest = new UpdatePortRequestDto
            {
                Name = "Updated Port",
                Country = "Turkey",
                City = "Antalya"
            };

            var updatedPort = new Port
            {
                PortId = 1,
                Name = "Updated Port",
                Country = "Turkey",
                City = "Antalya"
            };

            // Setup uniqueness check
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Updated Port", "Turkey", "Antalya", 1, 100))
                .ReturnsAsync(new List<Port>());

            _mockPortRepository.Setup(x => x.UpdateAsync(1, It.IsAny<Port>()))
                .ReturnsAsync(updatedPort);

            // Act
            var result = await _portService.UpdateAsync(1, updatePortRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Port", result.Name);
            Assert.Equal("Turkey", result.Country);
            Assert.Equal("Antalya", result.City);
            _mockPortRepository.Verify(x => x.UpdateAsync(1, It.IsAny<Port>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsInvalidOperationException_WhenPortExistsForDifferentPort()
        {
            // Arrange
            var updatePortRequest = new UpdatePortRequestDto
            {
                Name = "Istanbul Port",
                Country = "Turkey",
                City = "Istanbul"
            };

            var existingPort = new Port
            {
                PortId = 2, // Different port ID
                Name = "Istanbul Port",
                Country = "Turkey",
                City = "Istanbul"
            };

            // Setup uniqueness check - return existing port with different ID
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Istanbul Port", "Turkey", "Istanbul", 1, 100))
                .ReturnsAsync(new List<Port> { existingPort });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _portService.UpdateAsync(1, updatePortRequest));

            Assert.Equal("Port with same name, country and city already exists", exception.Message);
            _mockPortRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Port>()), Times.Never);
        }

        #endregion

        #region IsPortUniqueAsync Tests

        [Fact]
        public async Task IsPortUniqueAsync_ReturnsTrue_WhenPortIsUnique()
        {
            // Arrange
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Unique Port", "Turkey", "Unique City", 1, 100))
                .ReturnsAsync(new List<Port>());

            // Act
            var result = await _portService.IsPortUniqueAsync("Unique Port", "Turkey", "Unique City");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsPortUniqueAsync_ReturnsFalse_WhenPortExists()
        {
            // Arrange
            var existingPort = new Port { PortId = 1, Name = "Existing Port", Country = "Turkey", City = "Istanbul" };
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Existing Port", "Turkey", "Istanbul", 1, 100))
                .ReturnsAsync(new List<Port> { existingPort });

            // Act
            var result = await _portService.IsPortUniqueAsync("Existing Port", "Turkey", "Istanbul");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsPortUniqueAsync_ReturnsTrue_WhenPortExistsForExcludedPort()
        {
            // Arrange
            var existingPort = new Port { PortId = 1, Name = "Existing Port", Country = "Turkey", City = "Istanbul" };
            _mockPortRepository.Setup(x => x.GetAllAsync(null, "Existing Port", "Turkey", "Istanbul", 1, 100))
                .ReturnsAsync(new List<Port> { existingPort });

            // Act
            var result = await _portService.IsPortUniqueAsync("Existing Port", "Turkey", "Istanbul", 1); // Exclude the same port

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Standard CRUD Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsPortDto_WhenPortExists()
        {
            // Arrange
            var port = new Port
            {
                PortId = 1,
                Name = "Test Port",
                Country = "Turkey",
                City = "Istanbul"
            };

            _mockPortRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(port);

            // Act
            var result = await _portService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PortId);
            Assert.Equal("Test Port", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenPortDoesNotExist()
        {
            // Arrange
            _mockPortRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Port)null);

            // Act
            var result = await _portService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenPortIsDeleted()
        {
            // Arrange
            var deletedPort = new Port { PortId = 1, Name = "Deleted Port" };
            _mockPortRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedPort);

            // Act
            var result = await _portService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockPortRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenPortDoesNotExist()
        {
            // Arrange
            _mockPortRepository.Setup(x => x.DeleteAsync(999))
                .ReturnsAsync((Port)null);

            // Act
            var result = await _portService.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenPortExists()
        {
            // Arrange
            var port = new Port { PortId = 1, Name = "Test Port" };
            _mockPortRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(port);

            // Act
            var result = await _portService.ExistsAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenPortDoesNotExist()
        {
            // Arrange
            _mockPortRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Port)null);

            // Act
            var result = await _portService.ExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}

using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using LimanTakipSistemi.API.Repositories.CrewMemberRepository;
using LimanTakipSistemi.API.Services.CrewMemberService;
using Moq;
using Xunit;

namespace LimanTakipSistemi.Test.Services
{
    public class CrewMemberServiceTests : TestBase
    {
        private readonly Mock<ICrewMemberRepository> _mockCrewMemberRepository;
        private readonly CrewMemberService _crewMemberService;

        public CrewMemberServiceTests()
        {
            _mockCrewMemberRepository = new Mock<ICrewMemberRepository>();
            _crewMemberService = new CrewMemberService(_mockCrewMemberRepository.Object, Mapper);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsCrewMemberDtoList_WhenCrewMembersExist()
        {
            // Arrange
            var crewMembers = new List<CrewMember>
            {
                new CrewMember
                {
                    CrewId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+90 555 123 45 67",
                    Role = "Captain"
                },
                new CrewMember
                {
                    CrewId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "+90 555 987 65 43",
                    Role = "First Officer"
                }
            };

            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(crewMembers);

            // Act
            var result = await _crewMemberService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Doe", result[0].LastName);
            Assert.Equal("john.doe@example.com", result[0].Email);
            Assert.Equal("Captain", result[0].Role);
            
            _mockCrewMemberRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoCrewMembersExist()
        {
            // Arrange
            var crewMembers = new List<CrewMember>();
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100))
                .ReturnsAsync(crewMembers);

            // Act
            var result = await _crewMemberService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCrewMemberRepository.Verify(x => x.GetAllAsync(null, null, null, null, null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var crewMembers = new List<CrewMember>();
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(1, "John", "Doe", "john@example.com", "+90555", "Captain", 1, 50))
                .ReturnsAsync(crewMembers);

            // Act
            await _crewMemberService.GetAllAsync(1, "John", "Doe", "john@example.com", "+90555", "Captain", 1, 50);

            // Assert
            _mockCrewMemberRepository.Verify(x => x.GetAllAsync(1, "John", "Doe", "john@example.com", "+90555", "Captain", 1, 50), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsCrewMemberDto_WhenCrewMemberExists()
        {
            // Arrange
            var crewMember = new CrewMember
            {
                CrewId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+90 555 123 45 67",
                Role = "Captain"
            };

            _mockCrewMemberRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(crewMember);

            // Act
            var result = await _crewMemberService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CrewId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.Equal("Captain", result.Role);
            _mockCrewMemberRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenCrewMemberDoesNotExist()
        {
            // Arrange
            _mockCrewMemberRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((CrewMember)null);

            // Act
            var result = await _crewMemberService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockCrewMemberRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsCrewMemberDto_WhenEmailIsUnique()
        {
            // Arrange
            var addCrewMemberRequest = new AddCrewMemberRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+90 555 123 45 67",
                Role = "Captain"
            };

            var createdCrewMember = new CrewMember
            {
                CrewId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+90 555 123 45 67",
                Role = "Captain"
            };

            // Setup email uniqueness check
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "john.doe@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember>());

            _mockCrewMemberRepository.Setup(x => x.CreateAsync(It.IsAny<CrewMember>()))
                .ReturnsAsync(createdCrewMember);

            // Act
            var result = await _crewMemberService.CreateAsync(addCrewMemberRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.Equal("Captain", result.Role);
            _mockCrewMemberRepository.Verify(x => x.CreateAsync(It.IsAny<CrewMember>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenEmailAlreadyExists()
        {
            // Arrange
            var addCrewMemberRequest = new AddCrewMemberRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                PhoneNumber = "+90 555 123 45 67",
                Role = "Captain"
            };

            var existingCrewMember = new CrewMember
            {
                CrewId = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "existing@example.com",
                PhoneNumber = "+90 555 987 65 43",
                Role = "First Officer"
            };

            // Setup email uniqueness check - return existing crew member
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "existing@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember> { existingCrewMember });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _crewMemberService.CreateAsync(addCrewMemberRequest));

            Assert.Equal("Email address already exists", exception.Message);
            _mockCrewMemberRepository.Verify(x => x.CreateAsync(It.IsAny<CrewMember>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsCrewMemberDto_WhenEmailIsUniqueAndCrewMemberExists()
        {
            // Arrange
            var updateCrewMemberRequest = new UpdateCrewMemberRequestDto
            {
                FirstName = "John",
                LastName = "Updated",
                Email = "john.updated@example.com",
                PhoneNumber = "+90 555 999 88 77",
                Role = "Captain"
            };

            var updatedCrewMember = new CrewMember
            {
                CrewId = 1,
                FirstName = "John",
                LastName = "Updated",
                Email = "john.updated@example.com",
                PhoneNumber = "+90 555 999 88 77",
                Role = "Captain"
            };

            // Setup email uniqueness check
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "john.updated@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember>());

            _mockCrewMemberRepository.Setup(x => x.UpdateAsync(1, It.IsAny<CrewMember>()))
                .ReturnsAsync(updatedCrewMember);

            // Act
            var result = await _crewMemberService.UpdateAsync(1, updateCrewMemberRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Updated", result.LastName);
            Assert.Equal("john.updated@example.com", result.Email);
            _mockCrewMemberRepository.Verify(x => x.UpdateAsync(1, It.IsAny<CrewMember>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsInvalidOperationException_WhenEmailExistsForDifferentCrewMember()
        {
            // Arrange
            var updateCrewMemberRequest = new UpdateCrewMemberRequestDto
            {
                FirstName = "John",
                LastName = "Updated",
                Email = "existing@example.com",
                PhoneNumber = "+90 555 999 88 77",
                Role = "Captain"
            };

            var existingCrewMember = new CrewMember
            {
                CrewId = 2, // Different crew member ID
                FirstName = "Jane",
                LastName = "Smith",
                Email = "existing@example.com",
                PhoneNumber = "+90 555 987 65 43",
                Role = "First Officer"
            };

            // Setup email uniqueness check - return existing crew member with different ID
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "existing@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember> { existingCrewMember });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _crewMemberService.UpdateAsync(1, updateCrewMemberRequest));

            Assert.Equal("Email address already exists", exception.Message);
            _mockCrewMemberRepository.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<CrewMember>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenCrewMemberDoesNotExist()
        {
            // Arrange
            var updateCrewMemberRequest = new UpdateCrewMemberRequestDto
            {
                FirstName = "John",
                LastName = "Updated",
                Email = "john.updated@example.com",
                PhoneNumber = "+90 555 999 88 77",
                Role = "Captain"
            };

            // Setup email uniqueness check
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "john.updated@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember>());

            _mockCrewMemberRepository.Setup(x => x.UpdateAsync(999, It.IsAny<CrewMember>()))
                .ReturnsAsync((CrewMember)null);

            // Act
            var result = await _crewMemberService.UpdateAsync(999, updateCrewMemberRequest);

            // Assert
            Assert.Null(result);
            _mockCrewMemberRepository.Verify(x => x.UpdateAsync(999, It.IsAny<CrewMember>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenCrewMemberIsDeleted()
        {
            // Arrange
            var deletedCrewMember = new CrewMember { CrewId = 1, FirstName = "John", LastName = "Doe" };
            _mockCrewMemberRepository.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(deletedCrewMember);

            // Act
            var result = await _crewMemberService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockCrewMemberRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenCrewMemberDoesNotExist()
        {
            // Arrange
            _mockCrewMemberRepository.Setup(x => x.DeleteAsync(999))
                .ReturnsAsync((CrewMember)null);

            // Act
            var result = await _crewMemberService.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockCrewMemberRepository.Verify(x => x.DeleteAsync(999), Times.Once);
        }

        #endregion

        #region ExistsAsync Tests

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenCrewMemberExists()
        {
            // Arrange
            var crewMember = new CrewMember { CrewId = 1, FirstName = "John", LastName = "Doe" };
            _mockCrewMemberRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(crewMember);

            // Act
            var result = await _crewMemberService.ExistsAsync(1);

            // Assert
            Assert.True(result);
            _mockCrewMemberRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenCrewMemberDoesNotExist()
        {
            // Arrange
            _mockCrewMemberRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((CrewMember)null);

            // Act
            var result = await _crewMemberService.ExistsAsync(999);

            // Assert
            Assert.False(result);
            _mockCrewMemberRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        #endregion

        #region IsEmailUniqueAsync Tests

        [Fact]
        public async Task IsEmailUniqueAsync_ReturnsTrue_WhenEmailIsUnique()
        {
            // Arrange
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "unique@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember>());

            // Act
            var result = await _crewMemberService.IsEmailUniqueAsync("unique@example.com");

            // Assert
            Assert.True(result);
            _mockCrewMemberRepository.Verify(x => x.GetAllAsync(null, null, null, "unique@example.com", null, null, 1, 100), Times.Once);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ReturnsFalse_WhenEmailExists()
        {
            // Arrange
            var existingCrewMember = new CrewMember { CrewId = 1, Email = "existing@example.com" };
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "existing@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember> { existingCrewMember });

            // Act
            var result = await _crewMemberService.IsEmailUniqueAsync("existing@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ReturnsTrue_WhenEmailExistsForExcludedCrewMember()
        {
            // Arrange
            var existingCrewMember = new CrewMember { CrewId = 1, Email = "existing@example.com" };
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "existing@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember> { existingCrewMember });

            // Act
            var result = await _crewMemberService.IsEmailUniqueAsync("existing@example.com", 1); // Exclude the same crew member

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ReturnsFalse_WhenEmailExistsForDifferentCrewMember()
        {
            // Arrange
            var existingCrewMember = new CrewMember { CrewId = 2, Email = "existing@example.com" };
            _mockCrewMemberRepository.Setup(x => x.GetAllAsync(null, null, null, "existing@example.com", null, null, 1, 100))
                .ReturnsAsync(new List<CrewMember> { existingCrewMember });

            // Act
            var result = await _crewMemberService.IsEmailUniqueAsync("existing@example.com", 1); // Exclude different crew member

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}

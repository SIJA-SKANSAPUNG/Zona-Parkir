using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Parking_Zone.Controllers.Api;
using Parking_Zone.Models;
using System.Threading.Tasks;
using Xunit;

namespace Parking_Zone.Tests
{
    public class AuthApiControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;

        public AuthApiControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser" };
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };

            _userManagerMock.Setup(u => u.FindByNameAsync("testuser"))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, "password", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var jwtSettingsMock = new Mock<IConfigurationSection>();
            jwtSettingsMock.Setup(s => s["SecretKey"]).Returns("your-test-secret-key-here-make-it-long-enough");
            jwtSettingsMock.Setup(s => s["Issuer"]).Returns("test-issuer");
            jwtSettingsMock.Setup(s => s["Audience"]).Returns("test-audience");
            _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(jwtSettingsMock.Object);

            var controller = new AuthApiController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object);

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.True(returnValue.ContainsKey("token"));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "wrongpassword" };

            _userManagerMock.Setup(u => u.FindByNameAsync("testuser"))
                .ReturnsAsync((ApplicationUser)null);

            var controller = new AuthApiController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object);

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
} 
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using LaundryApi.Controllers;
using LaundryApi.Exceptions;
using LaundryApi.Models;

namespace LaundryUnitTest;
public class IsAliveControllerTests
{
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly string _testFilePath;
    private readonly string _jsonContent;

    public IsAliveControllerTests()
    {
        _mockEnv = new Mock<IWebHostEnvironment>();
        _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "build-info.json");
        _jsonContent = """
        {
            "commit": "abc123",
            "commitDate": "2024-06-14T12:00:00Z",
            "buildTime": "2024-06-14T12:30:00Z"
        }
        """;

        File.WriteAllText(_testFilePath, _jsonContent);

        _mockEnv.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
    }

    [Fact]
    public void IsAlive_ReturnsExpectedJson_WhenFileExists()
    {
        var controller = new IsAliveController(_mockEnv.Object);

        var result = controller.isAlive();

        var okResult = Assert.IsType<OkObjectResult>(result);
        
        var jsonResponse = Assert.IsType<IsAliveResponse>(okResult.Value);
        Assert.Equal("Alive", jsonResponse.Status);
        Assert.Equal("abc123", jsonResponse.Commit);
        Assert.Equal("2024-06-14T12:00:00Z", jsonResponse.CommitDate);
        Assert.Equal("2024-06-14T12:30:00Z", jsonResponse.BuildTime);
    }

    [Fact]

    public void IsAlive_ThrowsCustomException_WhenFileMissing()
    {
        File.Delete(_testFilePath); // simulate missing file
        var controller = new IsAliveController(_mockEnv.Object);

        var ex = Assert.Throws<CustomException>(() => controller.isAlive());

        Assert.Equal("Build info file is not found", ex.Message);
        Assert.Equal(404, ex.StatusCode);
    }
}

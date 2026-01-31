using Xunit;
using Moq;
using System;
using Microsoft.Extensions.Caching.Memory;
using TenantApi.Helpers;

namespace TenantApiUnitTest;

public class HelperFunctionsTests
{
    private readonly HelperFunctions _helper;

    public HelperFunctionsTests()
    {
        _helper = new HelperFunctions();
    }

    [Fact]
    public void TrackFailedAttempt_FirstTime_SetsInitialAttempt()
    {
        var email = "test@example.com";
        var failedEmail = $"failed_{email}";

        var mockCache = new Mock<IMemoryCache>();
        var mockEntry = new Mock<ICacheEntry>();

        object outVal = 0;

        mockCache.Setup(c => c.TryGetValue(failedEmail, out outVal!)).Returns(false);

        mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(mockEntry.Object);

        _helper.TrackFailedAttempt(email, mockCache.Object);
        mockCache.Verify(c => c.CreateEntry(failedEmail), Times.Exactly(2));
    }
    [Fact]
    public void TrackFailedAttempt_FifthTime_SetsLockoutAndRemovesFailed()
    {
        var email = "test@example.com";
        var failedEmail = $"failed_{email}";
        var lockEmail = $"lockout_{email}";

        var mockCache = new Mock<IMemoryCache>();
        var mockEntry = new Mock<ICacheEntry>();

        object outVal = 4;

        mockCache.Setup(c => c.TryGetValue(failedEmail, out outVal!)).Returns(true);

        mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(mockEntry.Object);

        _helper.TrackFailedAttempt(email, mockCache.Object);

        mockCache.Verify(c => c.CreateEntry(lockEmail), Times.Exactly(1));
        mockCache.Verify(c => c.Remove(failedEmail), Times.Exactly(1));
    }

    [Fact]
    public void TrackFailedAttempt_Intermediate_AttemptsIncremented()
    {
        var email = "mid@example.com";
        var failedEmail = $"failed_{email}";

        var mockCache = new Mock<IMemoryCache>();
        var mockEntry = new Mock<ICacheEntry>();

        object outVal = 2;

        mockCache.Setup(c => c.TryGetValue(failedEmail, out outVal!)).Returns(true);

        mockCache.Setup(c => c.CreateEntry(It.IsAny<object>()))
            .Returns(mockEntry.Object);


        for (int i = 0; i < 3; i++)
        {
            _helper.TrackFailedAttempt(email, mockCache.Object);
        }


        mockCache.Verify(c => c.CreateEntry(failedEmail), Times.Exactly(3));
    }

    [Fact]
    public void ResetFailedAttempts_RemovesCorrectKeys()
    {
        var email = "reset@example.com";
        var failedEmail = $"failed_{email}";
        var lockoutEmail = $"lockout_{email}";

        var mockCache = new Mock<IMemoryCache>();

        _helper.ResetFailedAttempts(email, mockCache.Object);

        mockCache.Verify(c => c.Remove(failedEmail), Times.Once);
        mockCache.Verify(c => c.Remove(lockoutEmail), Times.Once);
    }
}
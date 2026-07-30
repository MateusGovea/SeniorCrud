using FluentAssertions;
using SeniorCrud.Infrastructure.Authentication;

namespace SeniorCrud.UnitTests.Infrastructure;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_And_VerifyPassword_Should_Work()
    {
        var hasher = new PasswordHasher();
        var password = "super-secret";

        var hash = hasher.HashPassword(password);

        hash.Should().NotBeNullOrWhiteSpace();
        hasher.VerifyPassword(password, hash).Should().BeTrue();
        hasher.VerifyPassword("other", hash).Should().BeFalse();
    }
}

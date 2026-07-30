using FluentAssertions;
using SeniorCrud.Domain.Exceptions;
using SeniorCrud.Domain.ValueObjects;

namespace SeniorCrud.UnitTests.Domain;

public class ValueObjectsTests
{
    [Fact]
    public void Email_Should_Normalize_Value()
    {
        var email = new Email("  USER@TEST.COM ");

        email.Value.Should().Be("user@test.com");
    }

    [Fact]
    public void Email_Should_Throw_When_Invalid()
    {
        var action = () => new Email("invalid-email");

        action.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void Cpf_Should_Normalize_Digits()
    {
        var cpf = new Cpf("390.533.447-05");

        cpf.Value.Should().Be("39053344705");
    }

    [Fact]
    public void Cpf_Should_Throw_When_Invalid()
    {
        var action = () => new Cpf("111.111.111-11");

        action.Should().Throw<InvalidCpfException>();
    }

    [Fact]
    public void Cep_Should_Normalize_Digits()
    {
        var cep = new Cep("01001-000");

        cep.Value.Should().Be("01001000");
    }

    [Fact]
    public void Cep_Should_Throw_When_Invalid()
    {
        var action = () => new Cep("123");

        action.Should().Throw<InvalidCepException>();
    }

    [Fact]
    public void AddressNumber_Should_Normalize_To_Upper()
    {
        var number = new AddressNumber(" 12b ");

        number.Value.Should().Be("12B");
    }

    [Fact]
    public void AddressNumber_Should_Throw_When_Empty()
    {
        var action = () => new AddressNumber(" ");

        action.Should().Throw<InvalidAddressNumberException>();
    }

    [Fact]
    public void PasswordHash_Should_Accept_Valid_Length()
    {
        var hash = new PasswordHash(new string('h', 60));

        hash.Value.Should().HaveLength(60);
    }

    [Fact]
    public void PasswordHash_Should_Throw_When_TooShort()
    {
        var action = () => new PasswordHash("short");

        action.Should().Throw<InvalidPasswordHashException>();
    }
}

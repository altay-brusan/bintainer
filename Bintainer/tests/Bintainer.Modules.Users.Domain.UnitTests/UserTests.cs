using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Domain.UnitTests;

public class UserTests
{
    [Fact]
    public void Create_ReturnsUserWithProperties()
    {
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        user.Email.Should().Be("test@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.IdentityId.Should().Be("identity-123");
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_RaisesUserRegisteredDomainEvent()
    {
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserRegisteredDomainEvent>()
            .Which.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Create_TwoUsers_HaveDifferentIds()
    {
        var user1 = User.Create("a@b.com", "A", "B", "id-1");
        var user2 = User.Create("c@d.com", "C", "D", "id-2");

        user1.Id.Should().NotBe(user2.Id);
    }
}

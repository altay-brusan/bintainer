using Bintainer.Common.Domain;

namespace Bintainer.Common.Domain.UnitTests;

public class EntityTests
{
    private sealed class TestDomainEvent : DomainEvent;

    private sealed class TestEntity : Entity
    {
        public TestEntity() : base(Guid.NewGuid()) { }
        public TestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Constructor_WithId_SetsId()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void DomainEvents_InitiallyEmpty()
    {
        var entity = new TestEntity();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Raise_AddsDomainEvent()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        entity.Raise(domainEvent);

        entity.DomainEvents.Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void Raise_MultipleTimes_AddsAllEvents()
    {
        var entity = new TestEntity();
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        entity.Raise(event1);
        entity.Raise(event2);

        entity.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var entity = new TestEntity();
        entity.Raise(new TestDomainEvent());
        entity.Raise(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }
}

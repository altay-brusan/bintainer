using Bintainer.Common.Domain;

namespace Bintainer.Common.Domain.UnitTests;

public class DomainEventTests
{
    private sealed class TestDomainEvent : DomainEvent
    {
        public TestDomainEvent() : base() { }
        public TestDomainEvent(Guid id, DateTime occurredOnUtc) : base(id, occurredOnUtc) { }
    }

    [Fact]
    public void DefaultConstructor_GeneratesIdAndTimestamp()
    {
        var before = DateTime.UtcNow;

        var domainEvent = new TestDomainEvent();

        domainEvent.Id.Should().NotBeEmpty();
        domainEvent.OccurredOnUtc.Should().BeOnOrAfter(before);
        domainEvent.OccurredOnUtc.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void ParameterizedConstructor_SetsValues()
    {
        var id = Guid.NewGuid();
        var timestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var domainEvent = new TestDomainEvent(id, timestamp);

        domainEvent.Id.Should().Be(id);
        domainEvent.OccurredOnUtc.Should().Be(timestamp);
    }
}

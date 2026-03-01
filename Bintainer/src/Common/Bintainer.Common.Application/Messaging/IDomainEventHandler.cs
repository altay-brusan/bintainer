using Bintainer.Common.Domain;
using MediatR;

namespace Bintainer.Common.Application.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;

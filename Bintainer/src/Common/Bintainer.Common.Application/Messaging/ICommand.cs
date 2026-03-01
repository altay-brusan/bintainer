using Bintainer.Common.Domain;
using MediatR;

namespace Bintainer.Common.Application.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;

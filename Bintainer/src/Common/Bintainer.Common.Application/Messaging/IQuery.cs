using Bintainer.Common.Domain;
using MediatR;

namespace Bintainer.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;

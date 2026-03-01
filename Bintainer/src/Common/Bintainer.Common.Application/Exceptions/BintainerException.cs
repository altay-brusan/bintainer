using Bintainer.Common.Domain;

namespace Bintainer.Common.Application.Exceptions;

public sealed class BintainerException : Exception
{
    public BintainerException(string requestName, Error? error = null, Exception? innerException = null)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }
    public Error? Error { get; }
}

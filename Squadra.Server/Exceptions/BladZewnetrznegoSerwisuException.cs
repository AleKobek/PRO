namespace Squadra.Server.Exceptions;

public class BladZewnetrznegoSerwisuException : Exception
{
    public BladZewnetrznegoSerwisuException()
    {
    }

    public BladZewnetrznegoSerwisuException(string? message) : base(message)
    {
    }

    public BladZewnetrznegoSerwisuException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
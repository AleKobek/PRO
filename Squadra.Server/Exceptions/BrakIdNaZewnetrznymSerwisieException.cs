namespace Squadra.Server.Exceptions;

public class BrakIdNaZewnetrznymSerwisieException : Exception
{
    public BrakIdNaZewnetrznymSerwisieException ()
    {}

    public BrakIdNaZewnetrznymSerwisieException (string message) 
        : base(message)
    {}

    public BrakIdNaZewnetrznymSerwisieException (string message, Exception innerException)
        : base (message, innerException)
    {}
}
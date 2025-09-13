namespace Squadra.Exceptions;

public class NieZnalezionoWBazieException : Exception
{
    public NieZnalezionoWBazieException ()
    {}

    public NieZnalezionoWBazieException (string message) 
        : base(message)
    {}

    public NieZnalezionoWBazieException (string message, Exception innerException)
        : base (message, innerException)
    {}    
}
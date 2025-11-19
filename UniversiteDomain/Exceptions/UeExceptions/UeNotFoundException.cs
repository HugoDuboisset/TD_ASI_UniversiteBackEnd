namespace UniversiteDomain.Exceptions.UeExceptions;

public class UeNotFoundException : Exception

{
    public UeNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public UeNotFoundException(string message) : base(message)
    {
    }

    public UeNotFoundException() : base()
    {
    }
}





namespace Security.AcceptanceTests.Exceptions;

public class InternalServerErrorException(string message) : Exception(message)
{
}

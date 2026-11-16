namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class DomainException(string message, int errorCode) : Exception(message)
{
    public int ErrorCode { get; private set; } = errorCode;
}

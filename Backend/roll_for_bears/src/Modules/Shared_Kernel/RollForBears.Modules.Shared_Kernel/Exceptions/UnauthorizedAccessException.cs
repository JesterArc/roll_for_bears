namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class UnauthorizedAccessException(string message) : DomainException(message, 401)
{
    
}

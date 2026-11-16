namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class AccessForbiddenException(string message) : DomainException(message, 403)
{
    
}

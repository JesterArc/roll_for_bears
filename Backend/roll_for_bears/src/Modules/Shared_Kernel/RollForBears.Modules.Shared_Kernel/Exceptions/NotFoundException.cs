namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class NotFoundException(string message) : DomainException(message, 404)
{
    
}

namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class BadRequestException(string message) : DomainException(message, 400)
{
    
}

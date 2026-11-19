namespace RollForBears.Modules.Shared_Kernel.Exceptions;

public class InvalidFormatException(string message)  : DomainException(message, 400)
{
    
}
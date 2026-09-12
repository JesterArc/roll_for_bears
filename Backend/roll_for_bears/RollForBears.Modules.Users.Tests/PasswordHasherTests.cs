using RollForBears.Modules.Users.Security;
using NUnit.Framework;

namespace RollForBears.Modules.Users.Tests;

public class PasswordHasherTests
{
    private const string Pepper = "bd2NjYVcQr3neFq4MmZNlXsfZlsIYz6Ap76iK3W00bA=";

    [Test]
    public void Verify_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        var passwordHasher = new PasswordHasher(Pepper);

        const string password = "maslo";
        
        string hash = passwordHasher.Hash(password);
        
        bool result = passwordHasher.Verify(password, hash);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void Verify_ShouldReturnFalse_WhenPasswordIsNotCorrect()
    {
        var passwordHasher = new PasswordHasher(Pepper);
        const string password = "maslo";
        string hash = passwordHasher.Hash(password);
        bool result = passwordHasher.Verify("haslo", hash);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Verify_IfSaltGeneratesDifferentHashes()
    {
        var passwordHasher = new PasswordHasher(Pepper);
        const string password = "maslo";
        string hash = passwordHasher.Hash(password);
        string hash2 = passwordHasher.Hash(password);
        Assert.That(hash, Is.Not.EqualTo(hash2));
    }
}
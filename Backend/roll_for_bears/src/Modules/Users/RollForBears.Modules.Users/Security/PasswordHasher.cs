using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace RollForBears.Modules.Users.Security;

public partial class PasswordHasher
{
    // ustawienia takie jak zalecane na QWASP
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MemorySize = 19456;
    private const int Iterations = 2;
    private const int Parallelism = 1;

    private readonly byte[] _pepper;

    public PasswordHasher(string pepper)
    {
        _pepper = Encoding.UTF8.GetBytes(pepper);
    }
    
    public String Hash(string password)
    {   
        byte [] passwordToByte = Encoding.UTF8.GetBytes(password);
        byte [] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);
        var argron2 = new Argon2id(passwordToByte)
        {
            Salt = salt,
            MemorySize = MemorySize,
            Iterations =  Iterations,
            DegreeOfParallelism =  Parallelism,
            KnownSecret = _pepper
        };
        
        byte [] hashWord = argron2.GetBytes(HashSize);
        string saltWord = Convert.ToBase64String(salt);
        string hashString = Convert.ToBase64String(hashWord);
        return $"MemorySize:{MemorySize};Iterations:{Iterations};" +
               $"Parallelism:{Parallelism};Salt:{saltWord};Hash:{hashString}";
    }

    public bool Verify(string password, string hash)
    {
        string[] parts = hash.Split(";");
        
        int memorySize = int.Parse(parts[0].Replace("MemorySize:", ""));
        int iterations = int.Parse(parts[1].Replace("Iterations:", ""));
        int parallelism = int.Parse(parts[2].Replace("Parallelism:", ""));
        string saltString = parts[3].Replace("Salt:", "");
        string hashString = parts[4].Replace("Hash:", "");
        
        byte [] salt = Convert.FromBase64String(saltString);
        byte [] expectedHash = Convert.FromBase64String(hashString);

        byte[] passwordToByte = Encoding.UTF8.GetBytes(password);

        var argon2 = new Argon2id(passwordToByte)
        {
            Salt = salt,
            KnownSecret = _pepper,
            MemorySize = memorySize,
            DegreeOfParallelism = parallelism,
            Iterations = iterations,
        };

        byte [] actualHash = argon2.GetBytes(expectedHash.Length);
        
        bool isEqual = CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        return isEqual;
    }
}

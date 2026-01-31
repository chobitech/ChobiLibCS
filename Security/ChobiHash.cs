
using System.Security.Cryptography;

namespace Chobitech.Security;

public class ChobiHash(byte[]? key = null)
{
    public const int DefaultKeySize = 32;
    public static byte[] GenerateRandomKey(int size = DefaultKeySize) => ChobiLib.GenerateRandomBytes(size);

    private HMACSHA256 _hmac = new HMACSHA256(key ?? GenerateRandomKey());

    public ChobiHash(int keySize) : this(GenerateRandomKey(keySize))
    {        
    }

    public byte[] CalcHash(byte[] data) => _hmac.ComputeHash(data);

    public bool CompareHash(byte[] srcData, byte[] hashedData)
    {
        var hData = CalcHash(srcData);
        return CryptographicOperations.FixedTimeEquals(hData, hashedData);
    }
}

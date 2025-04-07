using System.Security.Cryptography;

namespace ChobiLib;

public static class ChobiAes256
{
    public const int KeyLengthBits = 256;
    public const int BlockSizeLengthBits = 128;

    public const int IvLengthBytes = 16;

    public const CipherMode CA256CipherMode = CipherMode.CBC;

    public const PaddingMode CA256PaddingMode = PaddingMode.PKCS7;

    private static byte[] GenerateRandomBytes(int size)
    {
        using var rng = RandomNumberGenerator.Create();

        var data = new byte[size];
        rng.GetBytes(data);
        
        return data;
    }

    public static byte[] GenerateKey() => GenerateRandomBytes(KeyLengthBits / 8);

    private static T ProcessInAes<T>(byte[] key, Func<Aes, T> func)
    {
        using var aes = Aes.Create();

        aes.KeySize = KeyLengthBits;
        aes.BlockSize = BlockSizeLengthBits;
        aes.Mode = CA256CipherMode;
        aes.Padding = CA256PaddingMode;

        aes.Key = key;

        return func(aes);
    }

    
    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        return ProcessInAes(key, aes =>
        {
            var iv = GenerateRandomBytes(IvLengthBytes);

            aes.IV = iv;

            var encrypter = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encrypter, CryptoStreamMode.Write);

            cs.Write(data, 0, data.Length);

            var encData = ms.ToArray();

            var res = new byte[IvLengthBytes + encData.Length];

            Array.Copy(iv, 0, res, 0, IvLengthBytes);
            Array.Copy(encData, 0, res, IvLengthBytes, encData.Length);

            return res;
        });
    }

    public static byte[] Decrypt(byte[] data, byte[] key)
    {
        
    }
}

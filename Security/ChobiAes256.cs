using System.Security.Cryptography;
using System.Text;

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

    
    public static byte[] Encrypt(byte[] data, byte[] key, byte[]? iv = null)
    {
        return ProcessInAes(key, aes =>
        {
            var ivData = iv ?? GenerateRandomBytes(IvLengthBytes);

            aes.IV = ivData;

            var encrypter = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encrypter, CryptoStreamMode.Write);

            cs.Write(data, 0, data.Length);

            var encData = ms.ToArray();

            var res = new byte[IvLengthBytes + encData.Length];

            Array.Copy(ivData, 0, res, 0, IvLengthBytes);
            Array.Copy(encData, 0, res, IvLengthBytes, encData.Length);

            return res;
        });
    }

    public static byte[] Encrypt(string s, byte[] key, byte[]? iv = null, Encoding? stringEncoding = null) => Encrypt(
        s.ConvertToByteArray(stringEncoding),
        key,
        iv
    );




    public static byte[] Decrypt(byte[] data, byte[] key, byte[]? iv = null)
    {
        byte[] ivData;
        int offset, length;

        if (iv != null)
        {
            ivData = iv;
            offset = 0;
            length = data.Length;
        }
        else
        {
            ivData = new byte[IvLengthBytes];
            Array.Copy(data, 0, ivData, 0, ivData.Length);
            offset = ivData.Length;
            length = data.Length - ivData.Length;
        }

        return ProcessInAes(key, (aes) =>
        {
            aes.IV = ivData;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var dataMs = new MemoryStream(data, offset, length);
            using var cs = new CryptoStream(dataMs, decryptor, CryptoStreamMode.Read);
            using var readMs = new MemoryStream();
            cs.CopyTo(readMs);

            return readMs.ToArray();
        });
    }



    public static string EncryptToBase64(byte[] data, byte[] key, byte[]? iv = null)
    {
        return Encrypt(data, key, iv).ConvertToBase64String();
    }

    public static byte[] DecryptFromBase64(string base64, byte[] key, byte[]? iv = null)
    {
        return Decrypt(
            base64.ConvertToByteArray(),
            key,
            iv
        );
    }




    public static string EncryptToHexString(byte[] data, byte[] key, byte[]? iv = null) => Encrypt(data, key, iv).ToHexString();

    public static string EncryptToHexString(string s, byte[] key, byte[]? iv = null, Encoding? stringEncoding = null) => EncryptToHexString(
        s.ConvertToByteArray(stringEncoding),
        key,
        iv
    );


    public static byte[] DecryptFromHexString(string hexString, byte[] key, byte[]? iv = null) => Decrypt(
        hexString.HexToBytes(),
        key,
        iv
    );
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class EncryptDecrypt
    {
        public static string Encrypt(string clearData, string algorithmName, string Key, string IV)
        {
            byte[] value = Encoding.UTF8.GetBytes(clearData);

            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);

            alg.Key = Encoding.UTF8.GetBytes(Key);
            alg.IV = Encoding.UTF8.GetBytes(IV);

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(value, 0, value.Length);
            cs.Close();

            return ToHex(ms.ToArray());
        }

        public static string Decrypt(string cipherData, string algorithmName, string Key, string IV)
        {
            byte[] data = HexToBytes(cipherData);

            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);
            alg.Key = Encoding.UTF8.GetBytes(Key);
            alg.IV = Encoding.UTF8.GetBytes(IV);


            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexToBytes(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        private static SymmetricAlgorithm GetAlgorithm(string name)
        {
            switch (name)
            {
                case "Rijndael":
                    return Rijndael.Create();
                case "TripleDES":
                    return TripleDES.Create();
                default:
                    throw new Exception(string.Format("Not supported algorithm: {0}", name));
            }
        }
    }
}

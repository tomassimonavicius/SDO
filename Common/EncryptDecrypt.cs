﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class EncryptDecrypt
    {
        public static string Encrypt(string clearData, string algorithmName, string key, string iv)
        {
            byte[] value = Encoding.UTF8.GetBytes(clearData);

            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);

            alg.Key = Encoding.UTF8.GetBytes(key);
            alg.IV = Encoding.UTF8.GetBytes(iv);

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(value, 0, value.Length);
            cs.Close();

            return ToHex(ms.ToArray());
        }

        public static string Decrypt(string cipherData, string algorithmName, string key, string iv)
        {
            byte[] data = HexToBytes(cipherData);

            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);
            alg.Key = Encoding.UTF8.GetBytes(key);
            alg.IV = Encoding.UTF8.GetBytes(iv);


            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string ToHex(byte[] bytes)
        {
            int bytesLength = bytes.Length;
            char[] c = new char[bytesLength * 2];

            for (int bx = 0, cx = 0; bx < bytesLength; ++bx, ++cx)
            {
                byte b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexToBytes(string str)
        {
            int strLength = str.Length;

            if (strLength == 0 || strLength % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[strLength / 2];
            int bufferLength = buffer.Length;
            for (int bx = 0, sx = 0; bx < bufferLength; ++bx, ++sx)
            {
                char c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

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

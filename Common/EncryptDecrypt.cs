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
            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);

            alg.Key = Encoding.UTF8.GetBytes(Key);
            alg.IV = Encoding.UTF8.GetBytes(IV);

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(Encoding.UTF8.GetBytes(clearData), 0, clearData.Length);
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string Decrypt(string cipherData, string algorithmName, string Key, string IV)
        {
            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);
            alg.Key = Encoding.UTF8.GetBytes(Key);
            alg.IV = Encoding.UTF8.GetBytes(IV);


            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(Encoding.UTF8.GetBytes(cipherData), 0, cipherData.Length);
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
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

using System;
using System.IO;
using System.Security.Cryptography; 

namespace Server
{
    public class EncryptDecrypt
    {
        public static byte[] Encrypt(byte[] clearData, string algorithmName, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);

            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();

            return ms.ToArray(); ;
        }

        public static byte[] Decrypt(byte[] cipherData, string algorithmName, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            SymmetricAlgorithm alg = GetAlgorithm(algorithmName);
            alg.Key = Key;
            alg.IV = IV;


            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();

            return ms.ToArray();
        }


        public static SymmetricAlgorithm GetAlgorithm(string name)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IniTranslator.Helpers
{
    internal static class CryptoHelper
    {
        // Define static key and IV for AES encryption (this is just an example; consider generating these securely)
        private static readonly byte[] Key = Convert.FromBase64String("HJ8zBYnltAmFoOrwhRrW1Xrm5ZBtfG+hKvfj2/oh8V4=");
        private static readonly byte[] IV = Convert.FromBase64String("ys3v2l+h/bogf8JZGGud1g==");

        internal static string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
            using (StreamWriter sw = new(cs))
            {
                sw.Write(value);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        internal static string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream ms = new(Convert.FromBase64String(value));
            using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }
    }
}

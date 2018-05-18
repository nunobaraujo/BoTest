using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public static class Common
    {
        public static string SHA1Encrypt(string plainText, string encryptionKey, string salt, string IV)
        {
            return RijndaelSimple.Encrypt(plainText, encryptionKey, salt, "SHA1", 1, IV, 128);
        }

        public static string SHA1Decrypt(string encryptedText, string encryptionKey, string salt, string IV)
        {
            return RijndaelSimple.Decrypt(encryptedText, encryptionKey, salt, "SHA1", 1, IV, 128);
        }
    }
}

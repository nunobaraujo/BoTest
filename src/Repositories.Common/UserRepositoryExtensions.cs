using Contracts;
using Contracts.Models;
using Core.Extensions;
using System;

namespace Repositories.Common
{
    public static class UserRepositoryExtensions
    {
        private const string IV = "BA8E98B7C5A5310F";
        private const string DataProtectionSalt = "3770C1DD-C84F-4B19-BF23-0CCD6E68D5AC";

        private static string Encrypt(string plainText, string encryptionKey, string salt)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));

            return Core.Common.SHA1Encrypt(plainText, encryptionKey, salt, IV);
        }
        private static string Decrypt(string encryptedText, string encryptionKey, string salt)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));

            return Core.Common.SHA1Decrypt(encryptedText, encryptionKey, salt, IV);
        }

        internal static void SetPassword(this User src, string password, string encryptionKey)
        {
            src.Salt = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            src.PasswordHash = Encrypt(password, encryptionKey, src.Salt);
        }

        public static bool CheckPassword(this IUser src, string password, string encryptionKey)
        {
            var hash = Encrypt(password, encryptionKey, src.Salt);
            return src.PasswordHash == hash;
        }

        internal static IUser EncryptUser(this IUser src, string encryptionKey)
        {
            var encrypted = src.ToDto();
            encrypted.Address = src.Address.EncryptDPString(encryptionKey);
            encrypted.City = src.City.EncryptDPString(encryptionKey);
            encrypted.Country = src.Country.EncryptDPString(encryptionKey);
            encrypted.Email = src.Email.EncryptDPString(encryptionKey);
            encrypted.FirstName = src.FirstName.EncryptDPString(encryptionKey);
            encrypted.LastName = src.LastName.EncryptDPString(encryptionKey);
            encrypted.PostalCode = src.PostalCode.EncryptDPString(encryptionKey);
            return encrypted;
        }
        internal static IUser DecryptUser(this IUser src, string encryptionKey)
        {
            var decrypted = src.ToDto();
            decrypted.Address = src.Address.DecryptDPString(encryptionKey);
            decrypted.City = src.City.DecryptDPString(encryptionKey);
            decrypted.Country = src.Country.DecryptDPString(encryptionKey);
            decrypted.Email = src.Email.DecryptDPString(encryptionKey);
            decrypted.FirstName = src.FirstName.DecryptDPString(encryptionKey);
            decrypted.LastName = src.LastName.DecryptDPString(encryptionKey);
            decrypted.PostalCode = src.PostalCode.DecryptDPString(encryptionKey);
            return decrypted;
        }

        internal static ICompany EncryptCompany(this ICompany src, string encryptionKey)
        {
            var encrypted = src.ToDto();
            encrypted.Name = src.Name.EncryptDPString(encryptionKey);
            encrypted.Address = src.Address.EncryptDPString(encryptionKey);
            encrypted.City = src.City.EncryptDPString(encryptionKey);
            encrypted.Country = src.Country.EncryptDPString(encryptionKey);
            encrypted.EMail = src.EMail.EncryptDPString(encryptionKey);
            encrypted.PostalCode = src.PostalCode.EncryptDPString(encryptionKey);
            encrypted.MobilePhone = src.MobilePhone.EncryptDPString(encryptionKey);
            encrypted.Telephone = src.Telephone.EncryptDPString(encryptionKey);
            encrypted.Fax = src.Fax.EncryptDPString(encryptionKey);
            return encrypted;
        }
        internal static ICompany DecryptCompany(this ICompany src, string encryptionKey)
        {
            var decrypted = src.ToDto();
            decrypted.Name = src.Name.DecryptDPString(encryptionKey);
            decrypted.Address = src.Address.DecryptDPString(encryptionKey);
            decrypted.City = src.City.DecryptDPString(encryptionKey);
            decrypted.Country = src.Country.DecryptDPString(encryptionKey);
            decrypted.EMail = src.EMail.DecryptDPString(encryptionKey);
            decrypted.PostalCode = src.PostalCode.DecryptDPString(encryptionKey);
            decrypted.MobilePhone = src.MobilePhone.DecryptDPString(encryptionKey);
            decrypted.Telephone = src.Telephone.DecryptDPString(encryptionKey);
            decrypted.Fax = src.Fax.DecryptDPString(encryptionKey);
            return decrypted;
        }

        internal static string EncryptDPString(this string src, string encryptionKey)
        {
            return Encrypt(src, encryptionKey, DataProtectionSalt);
        }
        internal static string DecryptDPString(this string src, string encryptionKey)
        {
            return Decrypt(src, encryptionKey, DataProtectionSalt);
        }
    }
}

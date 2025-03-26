using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class SecurityExtensions
    {
        public static string HashPassword(this string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public static string GenerateRandomToken(int length = 32)
        {
            using var rng = new RNGCryptoServiceProvider();
            var tokenBytes = new byte[length];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        public static bool IsStrongPassword(this string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            var hasMinLength = new Regex(@".{8,}");

            return hasNumber.IsMatch(password) &&
                   hasUpperChar.IsMatch(password) &&
                   hasLowerChar.IsMatch(password) &&
                   hasSymbols.IsMatch(password) &&
                   hasMinLength.IsMatch(password);
        }

        public static string SanitizeInput(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove potentially dangerous characters
            return Regex.Replace(input, @"[<>()&;]", string.Empty);
        }

        public static string GenerateSecureFileName(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            // Remove invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedFileName = new string(fileName
                .Where(ch => !invalidChars.Contains(ch))
                .ToArray());

            // Add random suffix for uniqueness
            var extension = Path.GetExtension(sanitizedFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(sanitizedFileName);
            var randomSuffix = GenerateRandomToken(8);

            return $"{nameWithoutExtension}_{randomSuffix}{extension}";
        }

        public static string MaskSensitiveData(this string data, int visibleChars = 4)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            if (data.Length <= visibleChars)
                return new string('*', data.Length);

            return new string('*', data.Length - visibleChars) + data.Substring(data.Length - visibleChars);
        }

        public static bool IsValidIpAddress(this string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }

        public static string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }

        public static bool ValidateApiKey(this string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return false;

            try
            {
                var data = Convert.FromBase64String(apiKey);
                return data.Length == 32;
            }
            catch
            {
                return false;
            }
        }

        public static string EncryptData(this string data, string key)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16];

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string DecryptData(this string encryptedData, string key)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16];

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }
    }
} 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parking_Zone.Extensions
{
    public static class ValidationExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            // Indonesian phone number pattern
            var pattern = @"^(\+62|62|0)8[1-9][0-9]{7,10}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public static bool IsValidUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsValidDate(this string date)
        {
            return DateTime.TryParse(date, out _);
        }

        public static bool IsValidTime(this string time)
        {
            return TimeSpan.TryParse(time, out _);
        }

        public static bool IsValidDecimal(this string value)
        {
            return decimal.TryParse(value, out _);
        }

        public static bool IsValidInteger(this string value)
        {
            return int.TryParse(value, out _);
        }

        public static bool IsValidGuid(this string value)
        {
            return Guid.TryParse(value, out _);
        }

        public static bool IsValidRange<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        public static bool IsValidLength(this string value, int minLength, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return minLength == 0;

            return value.Length >= minLength && value.Length <= maxLength;
        }

        public static bool IsValidPassword(this string password)
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

        public static bool IsValidCreditCard(this string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return false;

            // Remove any non-digit characters
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Luhn algorithm
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n = (n % 10) + 1;
                }
                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }

        public static ValidationResult ValidateObject(this object obj)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, results, validateAllProperties: true))
            {
                var errors = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                return new ValidationResult(errors);
            }

            return ValidationResult.Success;
        }

        public static bool IsValidJson(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                System.Text.Json.JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidXml(this string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return false;

            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 
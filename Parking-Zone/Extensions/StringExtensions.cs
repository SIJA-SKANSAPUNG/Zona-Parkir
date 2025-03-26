using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        public static string ToSlug(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-");
            text = text.Trim('-');

            return text;
        }

        public static string FormatLicensePlate(this string licensePlate)
        {
            if (string.IsNullOrEmpty(licensePlate))
                return licensePlate;

            // Remove any whitespace and special characters
            licensePlate = Regex.Replace(licensePlate, @"[^a-zA-Z0-9]", "").ToUpper();

            // Format based on common Indonesian license plate pattern (e.g., B1234ABC)
            if (licensePlate.Length >= 3)
            {
                var region = licensePlate.Substring(0, 1);
                var numbers = Regex.Match(licensePlate.Substring(1), @"\d+").Value;
                var letters = Regex.Match(licensePlate.Substring(1 + numbers.Length), @"[A-Z]+").Value;

                return $"{region} {numbers} {letters}".Trim();
            }

            return licensePlate;
        }

        public static bool IsValidLicensePlate(this string licensePlate)
        {
            if (string.IsNullOrEmpty(licensePlate))
                return false;

            // Indonesian license plate pattern: 1-2 letters, 1-4 numbers, 1-3 letters
            var pattern = @"^[A-Z]{1,2}\s?\d{1,4}\s?[A-Z]{1,3}$";
            return Regex.IsMatch(licensePlate.ToUpper(), pattern);
        }

        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public static string ToInitials(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var initials = words.Select(w => w[0]);
            return string.Join("", initials).ToUpper();
        }

        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ToNumeric(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return new string(text.Where(char.IsDigit).ToArray());
        }

        public static string ToAlphanumeric(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        }

        public static string FormatPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            // Remove any non-numeric characters
            var numericOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Format for Indonesian phone numbers
            if (numericOnly.StartsWith("0"))
                numericOnly = "+62" + numericOnly.Substring(1);
            else if (numericOnly.StartsWith("62"))
                numericOnly = "+" + numericOnly;

            return numericOnly;
        }
    }
} 
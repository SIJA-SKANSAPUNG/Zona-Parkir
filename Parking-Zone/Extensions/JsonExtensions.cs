using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parking_Zone.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            if (obj == null)
                return null;

            return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
        }

        public static T FromJson<T>(this string json, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }

        public static bool TryParseJson<T>(this string json, out T result, JsonSerializerOptions options = null)
        {
            try
            {
                result = json.FromJson<T>(options);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static object ToObject(this string json, Type type, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize(json, type, options ?? DefaultOptions);
        }

        public static bool IsValidJson(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static JsonElement? GetProperty(this JsonElement element, string propertyName)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            if (element.TryGetProperty(propertyName, out JsonElement value))
                return value;

            return null;
        }

        public static T GetPropertyValue<T>(this JsonElement element, string propertyName, T defaultValue = default)
        {
            var property = element.GetProperty(propertyName);
            if (property == null)
                return defaultValue;

            try
            {
                return JsonSerializer.Deserialize<T>(property.Value.GetRawText());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string PrettyPrint(this string json)
        {
            if (!json.IsValidJson())
                return json;

            var element = JsonSerializer.Deserialize<JsonElement>(json);
            return JsonSerializer.Serialize(element, new JsonSerializerOptions { WriteIndented = true });
        }

        public static string ToJsonWithoutNull<T>(this T obj)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(obj, options);
        }

        public static string ToJsonWithEnums<T>(this T obj)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Serialize(obj, options);
        }
    }
}
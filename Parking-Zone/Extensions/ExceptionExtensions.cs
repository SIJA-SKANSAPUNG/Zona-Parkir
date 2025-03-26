using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception ex)
        {
            var messages = new List<string>();
            var currentEx = ex;

            while (currentEx != null)
            {
                messages.Add(currentEx.Message);
                currentEx = currentEx.InnerException;
            }

            return string.Join(" -> ", messages);
        }

        public static string GetStackTraceWithSource(this Exception ex)
        {
            var sb = new StringBuilder();
            var currentEx = ex;

            while (currentEx != null)
            {
                if (sb.Length > 0)
                    sb.AppendLine("--- Inner Exception ---");

                sb.AppendLine($"Exception Type: {currentEx.GetType().FullName}");
                sb.AppendLine($"Message: {currentEx.Message}");
                sb.AppendLine($"Source: {currentEx.Source}");
                sb.AppendLine($"Stack Trace:");
                sb.AppendLine(currentEx.StackTrace);

                currentEx = currentEx.InnerException;
            }

            return sb.ToString();
        }

        public static IDictionary<string, string> ToErrorDictionary(this Exception ex)
        {
            var dict = new Dictionary<string, string>
            {
                { "Type", ex.GetType().Name },
                { "Message", ex.Message },
                { "Source", ex.Source ?? "Unknown" },
                { "StackTrace", ex.StackTrace ?? "Not available" }
            };

            if (ex.InnerException != null)
            {
                dict.Add("InnerException", ex.InnerException.Message);
            }

            // Add additional info for specific exception types
            if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                dict.Add("ErrorNumber", sqlEx.Number.ToString());
                dict.Add("Procedure", sqlEx.Procedure ?? "Unknown");
            }
            else if (ex is System.Net.WebException webEx)
            {
                dict.Add("Status", webEx.Status.ToString());
                dict.Add("Response", webEx.Response?.ToString() ?? "No response");
            }

            return dict;
        }

        public static bool IsCritical(this Exception ex)
        {
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is System.Threading.ThreadAbortException ||
                   ex is AccessViolationException ||
                   ex is AppDomainUnloadedException ||
                   ex is BadImageFormatException ||
                   ex is System.Data.SqlClient.SqlException;
        }

        public static bool IsTransient(this Exception ex)
        {
            return ex is TimeoutException ||
                   ex is System.Net.Sockets.SocketException ||
                   ex is System.IO.IOException ||
                   (ex is System.Data.SqlClient.SqlException sqlEx && 
                    new[] { -2, -1, 2, 53, 121, 233 }.Contains(sqlEx.Number));
        }

        public static string ToLogString(this Exception ex, bool includeStackTrace = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Exception Details:");
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.GetFullMessage()}");
            sb.AppendLine($"Source: {ex.Source ?? "Unknown"}");

            if (includeStackTrace && !string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(ex.StackTrace);
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner Exception:");
                sb.AppendLine(ex.InnerException.ToLogString(includeStackTrace));
            }

            return sb.ToString();
        }
    }
} 
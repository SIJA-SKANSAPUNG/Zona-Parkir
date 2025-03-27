using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Parking_Zone.Extensions
{
    public static class FileExtensions
    {
        public static async Task<string> SaveAsAsync(this IFormFile file, string path, string fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty", nameof(file));

            fileName = fileName ?? Path.GetRandomFileName();
            var extension = Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName + extension);

            Directory.CreateDirectory(path);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        public static bool IsImage(this IFormFile file)
        {
            if (file == null)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return Array.IndexOf(allowedExtensions, extension) >= 0;
        }

        public static async Task<string> SaveAsImageAsync(this IFormFile file, string path, int maxWidth = 800, int maxHeight = 600, string fileName = null)
        {
            if (!file.IsImage())
                throw new ArgumentException("File is not an image", nameof(file));

            fileName = fileName ?? Path.GetRandomFileName();
            var extension = Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName + extension);

            Directory.CreateDirectory(path);

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));
                await image.SaveAsync(fullPath);
            }

            return fullPath;
        }

        public static string GetSizeString(this IFormFile file)
        {
            if (file == null)
                return "0 B";

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = file.Length;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public static string GetContentType(this string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        public static async Task<byte[]> ToByteArrayAsync(this IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static string GetUniqueFileName(this IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            return $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
        }

        public static bool IsSafeFileName(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(c => invalidChars.Contains(c));
        }
    }
} 
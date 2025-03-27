using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Parking_Zone.Services
{
    public class StorageService
    {
        private readonly ILogger<StorageService> _logger;
        private readonly string _baseStoragePath;

        public StorageService(ILogger<StorageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _baseStoragePath = configuration["Storage:BasePath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage");
            EnsureStorageDirectories();
        }

        private void EnsureStorageDirectories()
        {
            var directories = new[]
            {
                Path.Combine(_baseStoragePath, "Images"),
                Path.Combine(_baseStoragePath, "Documents"),
                Path.Combine(_baseStoragePath, "Tickets"),
                Path.Combine(_baseStoragePath, "Reports")
            };

            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.LogInformation($"Created storage directory: {dir}");
                }
            }
        }

        public async Task<string> SaveFileAsync(string category, string fileName, Stream fileStream)
        {
            try
            {
                var categoryPath = Path.Combine(_baseStoragePath, category);
                var filePath = Path.Combine(categoryPath, fileName);

                using (var fileStreamWriter = File.Create(filePath))
                {
                    await fileStream.CopyToAsync(fileStreamWriter);
                }

                _logger.LogInformation($"File saved successfully: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving file {fileName} in category {category}");
                throw;
            }
        }

        public async Task<string> SaveImage(string fileName, byte[] imageBytes)
        {
            try
            {
                var imagePath = Path.Combine(_baseStoragePath, "Images", fileName);
                await File.WriteAllBytesAsync(imagePath, imageBytes);
                _logger.LogInformation($"Image saved successfully: {imagePath}");
                return imagePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving image {fileName}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string category, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_baseStoragePath, category, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted successfully: {filePath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file {fileName} from category {category}");
                throw;
            }
        }

        public string GetFilePath(string category, string fileName)
        {
            return Path.Combine(_baseStoragePath, category, fileName);
        }
    }
}
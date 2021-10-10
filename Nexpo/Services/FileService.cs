using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Nexpo.Services
{
    public class FileService
    {
        private readonly string _uploadsPath = "uploads";
        private readonly Dictionary<string, string> mimeExtensionMappings = new()
        {
            { "image/png", ".png" },
            { "image/jpeg", ".jpg" },
            { "application/pdf", ".pdf" }
        };
        private readonly IConfig _config;

        public FileService(IConfig iConfig)
        {
            _config = iConfig;
        }

        private string Sha256Hash(string file)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var readStream = File.OpenRead(file))
                {
                    var hash = Convert.ToHexString(sha256.ComputeHash(readStream));
                    return hash;
                }
            }
        }

        public string GetLocalFileName(string filename)
        {
            return Path.GetFullPath($"{_uploadsPath}{Path.DirectorySeparatorChar}{filename}");
        }

        public string GetFileAccessUrl(string filename)
        {
            // There is no way to get the action name from here so we hardcode it
            return $"{_config.BaseUrl}/api/files/{filename}";
        }

        public string GetFilenameFromUrl(string url)
        {
            var baseUrl = GetFileAccessUrl("");
            return url.Remove(0, baseUrl.Length);
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            var extension = mimeExtensionMappings.GetValueOrDefault(file.ContentType, "");
            if (extension == "")
            {
                Console.Error.WriteLine($"Mime of {file.ContentType} does not have an extension mapping, using no extension");
            }

            var tempLocation = Path.GetTempFileName();
            using (var writeStream = File.OpenWrite(tempLocation))
            {
                await file.CopyToAsync(writeStream);
            }

            var hash = Sha256Hash(tempLocation);
            var filename = $"{hash}{extension}";
            var localFilename = GetLocalFileName(filename);
            File.Move(tempLocation, localFilename);

            return GetFileAccessUrl(filename);
        }

        public void RemoveFile(string url)
        {
            var filename = GetFilenameFromUrl(url);
            var localFilename = GetLocalFileName(filename);
            if (File.Exists(localFilename))
            {
                File.Delete(localFilename);
            }
        }
    }
}

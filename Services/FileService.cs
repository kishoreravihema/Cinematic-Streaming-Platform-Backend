using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Netflix_BackendAPI.Class;
using Netflix_BackendAPI.DTO;
using Netflix_BackendAPI.Interface.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Netflix_BackendAPI.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mp3", ".avi", ".mov"
        };

        private const long MaxFileSize = 50L * 1024 * 1024 * 1024; // 50 GB

        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<BaseResponse<string>> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "No file uploaded",
                    Code = ResponseCode.BadRequest,
                    Errors = new List<string> { "File is null or empty." }
                };
            }

            if (file.Length > MaxFileSize)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "File size exceeds limit.",
                    Code = ResponseCode.BadRequest,
                    Errors = new List<string> { $"Max allowed size is {MaxFileSize / (1024 * 1024 * 1024)} GB." }
                };
            }

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Unsupported file type.",
                    Code = ResponseCode.UnsupportedMediaType,
                    Errors = new List<string> { $"Extension '{extension}' is not allowed." }
                };
            }

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                _logger.LogInformation("File uploaded successfully: {FileName}", uniqueFileName);

                var publicUrl = $"/uploads/{uniqueFileName}";

                return new BaseResponse<string>
                {
                    Success = true,
                    Message = "File uploaded successfully.",
                    Code = ResponseCode.Created,
                    Data = publicUrl
                };
            }
            catch (Exception ex)
            {
                LogExceptionHandler.LogException(ex, nameof(UploadFileAsync));
                _logger.LogError(ex, "File upload failed.");

                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "File upload failed.",
                    Code = ResponseCode.ServerError,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

using FluentResults;

namespace Labubu.Main.Services;

public interface IMinioService
{
    Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Result<string>> GetFileUrlAsync(string fileName);
    Task<Result> DeleteFileAsync(string fileName);
}


using FluentResults;
using Labubu.Main.Configuration;
using Microsoft.Extensions.Options;
using Minio;

namespace Labubu.Main.Services;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioConfig _config;

    public MinioService(IOptions<MinioConfig> config)
    {
        _config = config.Value;
        
        _minioClient = new MinioClient()
            .WithEndpoint(_config.Endpoint)
            .WithCredentials(_config.AccessKey, _config.SecretKey)
            .WithSSL(_config.UseSSL)
            .Build();
        
        _ = Task.Run(async () => await InitializeBucketAsync());
    }

    private async Task InitializeBucketAsync()
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_config.BucketName);
            
            var found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_config.BucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }
        }
        catch
        {
        }
    }

    public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);
            
            await _minioClient.PutObjectAsync(putObjectArgs);
            
            var url = await GetFileUrlAsync(fileName);
            return url;
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to upload file: {ex.Message}");
        }
    }

    public async Task<Result<string>> GetFileUrlAsync(string fileName)
    {
        try
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(fileName)
                .WithExpiry(3600 * 24 * 7);
            
            var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            return Result.Ok(url);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to get file URL: {ex.Message}");
        }
    }
    

    public async Task<Result> DeleteFileAsync(string fileName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_config.BucketName)
                .WithObject(fileName);
            
            await _minioClient.RemoveObjectAsync(removeObjectArgs);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to delete file: {ex.Message}");
        }
    }
}

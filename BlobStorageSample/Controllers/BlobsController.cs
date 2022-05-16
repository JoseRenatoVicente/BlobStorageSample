using BlobStorageSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlobStorageSample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlobsController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;

    public BlobsController(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [HttpGet("{fileName}")]
    public async Task<string> Get(string fileName)
    {
        return await _blobStorageService.GetBlob(fileName);
    }
    [HttpPost]
    public async Task<string> Post(IFormFile file)
    {
        string fileName = await _blobStorageService.ExistAsync(file.FileName)
            ? Path.GetFileNameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName)
            : file.FileName;

        return await _blobStorageService.UploadFileToStorageAsync(fileName, file.OpenReadStream());
    }

    [HttpDelete("{fileName}")]
    public async Task Delete(string fileName)
    {
        await _blobStorageService.DeleteFileIfExistsAsync(fileName);
    }
}
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using BlobStorageSample.Settings;

namespace BlobStorageSample.Services;

public interface IBlobStorageService
{
    Task<string> GetBlob(string fileName);
    Task<string> UploadFileToStorageAsync(string fileName, Stream fileContent);
    Task DeleteFileIfExistsAsync(string fileName);
    Task<bool> ExistAsync(string fileName);

}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _account;
    private readonly BlobStorageSettings _storageSettings;

    public BlobStorageService(BlobStorageSettings storageSettings)
    {
        _storageSettings = storageSettings;
        _account = new BlobContainerClient(storageSettings.ConnectionString, storageSettings.ContainerName);
        _account.CreateIfNotExists();
    }

    public async Task<string> GetBlob(string fileName)
    {
        var blockBlob = _account.GetBlobClient(fileName);

        return (await GetUserDelegationSasBlob(blockBlob)).ToString();
    }

    public async Task<string> UploadFileToStorageAsync(string fileName, Stream fileContent)
    {
        var blockBlob = _account.GetBlobClient(fileName);

        await blockBlob.UploadAsync(fileContent);

        return (await GetUserDelegationSasBlob(blockBlob)).ToString();
    }
    public async Task DeleteFileIfExistsAsync(string fileName)
    {
        var blockBlob = _account.GetBlobClient(fileName);

        await blockBlob.DeleteIfExistsAsync();
    }

    public async Task<bool> ExistAsync(string fileName)
    {
        var blockBlob = _account.GetBlobClient(fileName);

        return await blockBlob.ExistsAsync();
    }

    private Task<Uri> GetUserDelegationSasBlob(BlobClient blobClient)
    {
        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var keyCred = new StorageSharedKeyCredential(_storageSettings.AccountName, _storageSettings.AccountKey);

        BlobUriBuilder blobUriBuilder = new(blobClient.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(keyCred)
        };

        return Task.Run(() => blobUriBuilder.ToUri());
    }
}
namespace BlobStorageSample.Settings;

public sealed class BlobStorageSettings
{
    public string? AccountName { get; set; }
    public string? AccountKey { get; set; }
    public string? ContainerName { get; set; }
    public string? EndpointSuffix { get; set; }
    public string ConnectionString => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix={EndpointSuffix}";
}
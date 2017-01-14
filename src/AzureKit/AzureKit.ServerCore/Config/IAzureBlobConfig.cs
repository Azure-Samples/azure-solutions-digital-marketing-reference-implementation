namespace AzureKit.Config
{
    public interface IAzureBlobConfig
    {
        string CDNAddress { get; set; }
        string ImageContainerName { get; set; }
        int SASURLExpiryDuration { get; set; }
        string StorageAccountKey { get; set; }
        string StorageAccountName { get; set; }
        string VideoContainerName { get; set; }
    }
}
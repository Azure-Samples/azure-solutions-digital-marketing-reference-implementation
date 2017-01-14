namespace AzureKit.Config
{
    public static class Constants
    {
        public static readonly string KEY_AZURE_STORAGE_ACCT_NAME ="azureStorageAccountName";
        public static readonly string KEY_AZURE_STORAGE_ACCT_KEY = "azureStorageAccountKey";
        public static readonly string KEY_AZURE_STORAGE_VIDEO_CONTAINER = "mediaStorageVideoContainerName";
        public static readonly string KEY_AZURE_STORAGE_IMAGES_CONTAINER = "mediaStorageImageContainerName";
        public static readonly string KEY_AZURE_STORAGE_UPLOAD_POLICY_TIMEOUT = "mediaStorageUploadPolicyTimeout";
        public static readonly string FORMAT_AZURE_STORAGE_ADDRESS = "https://{0}.blob.core.windows.net";


        public static readonly string FORMAT_AZURE_STORAGE_URL = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
        public static readonly string FORMAT_AZURE_DOCDB_URL = "https://{0}.documents.azure.com:443/";
        public static readonly string FORMAT_AZURE_DOCDB_COLLECTION = "dbs/{0}/colls/{1}";
        public static readonly string FORMAT_AZURE_DOCDB_ITEM = "dbs/{0}/colls/{1}/{2}";
        
        public static readonly int DEFAULT_SASURL_EXPIRY = 10;

        public static readonly int DEFAULT_THUMBNAIL_HEIGHT = 200;

        public static readonly string KEY_DOCDB_ACCT_KEY = "azureDocumentDBKey";
        public static readonly string KEY_DOCDB_CONTENT_COLLECTION = "azureDocumentDBContentCollection";
        public static readonly string KEY_DOCDB_MEDIA_COLLECTION = "azureDocumentDBMediaCollection";
        public static readonly string KEY_DOCDB_DBNAME = "azureDocDBDatabaseName";
        public static readonly string KEY_DOCDB_SERVER_NAME = "azureDocumentDBServer";

        public static readonly string KEY_SQL_CONNECTION_STRING = "SqlDbConnection";


        public static readonly string PATH_STATIC_VIDEO_THUMBNAIL = "/content/images/videothumb.png";
        public static readonly string KEY_AZURE_STORAGE_CDN = "azureStorageCDN";
        public static readonly string KEY_AZURE_REDIS_CONNECTION = "redisCacheConnection";

        public static readonly string KEY_BANNER_CONTENT = "siteBanner";

        public const string CACHE_DEFAULT_PROFILE = "DefaultContentProfile";
    }
}
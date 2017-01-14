using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Configuration;

namespace AzureKit.Config
{
    /// <summary>
    /// Represents the configuration found in the web.config related to Document DB
    /// </summary>
    public class DocumentDBConfig : IDocumentDBConfig
    {
        private static DocumentClient s_dbClient;
        public DocumentDBConfig()
        {
            ConnectionPolicy = new ConnectionPolicy();
                
            Consistency = ConsistencyLevel.Eventual;
                
            DatabaseName = ConfigurationManager.AppSettings[Constants.KEY_DOCDB_DBNAME];
            ServerName = ConfigurationManager.AppSettings[Constants.KEY_DOCDB_SERVER_NAME];
            AccessKey = ConfigurationManager.AppSettings[Constants.KEY_DOCDB_ACCT_KEY];
            MainContentCollectionName = ConfigurationManager.AppSettings[Constants.KEY_DOCDB_CONTENT_COLLECTION];
            MediaContentCollectionName = ConfigurationManager.AppSettings[Constants.KEY_DOCDB_MEDIA_COLLECTION];
            
            Uri parsedUri;

            if (Uri.TryCreate(
                String.Format(Constants.FORMAT_AZURE_DOCDB_URL,ServerName),
                UriKind.Absolute, out parsedUri))
            {
                DatabaseUrl = parsedUri;
                
                //initialize the db client
                s_dbClient = new DocumentClient(
                    DatabaseUrl, AccessKey,
                    ConnectionPolicy, Consistency);
            }          
        }
        public Uri DatabaseUrl { get; set; }

        public string AccessKey { get; set; }

        public ConnectionPolicy ConnectionPolicy{ get; set; }

        public ConsistencyLevel Consistency { get; set; }

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }


        public string MediaContentCollectionName { get; set; }

        public string MainContentCollectionName { get; set; }

        public string MediaContentCollectionUrl
        {
            get { return string.Format(Constants.FORMAT_AZURE_DOCDB_COLLECTION, DatabaseName, MediaContentCollectionName); }
        }
        public string SiteContentCollectionUrl
        {
            get { return string.Format(Constants.FORMAT_AZURE_DOCDB_COLLECTION, DatabaseName, MainContentCollectionName); }
        }

        public DocumentClient Client
        {
            get { return s_dbClient; }
        }
    }
}
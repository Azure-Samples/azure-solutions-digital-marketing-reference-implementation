using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AzureKit.Config
{
    public interface IDocumentDBConfig
    {
        string AccessKey { get; set; }
        DocumentClient Client { get; }
        ConnectionPolicy ConnectionPolicy { get; set; }
        ConsistencyLevel Consistency { get; set; }
        string DatabaseName { get; set; }
        Uri DatabaseUrl { get; set; }
        string MainContentCollectionName { get; set; }
        string MediaContentCollectionName { get; set; }
        string MediaContentCollectionUrl { get; }
        string ServerName { get; set; }
        string SiteContentCollectionUrl { get; }
    }
}
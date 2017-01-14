using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Management.Automation;

namespace AzureKit.PowerShell
{
    /// <summary>
    /// Cmdlet to create a DocumentDB database and collection.
    /// </summary>
    /// <remarks>At that time this kit was created, there were no PS cmdlets to handle this work. If official cmdlets exist, the script should be updated to use them.</remarks>
    [Cmdlet(VerbsCommon.New, "DocDbDatabaseAndCollection")]
    public class DocDBCommands : PSCmdlet
    {
        [Parameter(Mandatory = true,
            HelpMessage = "The URI for the docdb server on which to create the database and collection",
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromRemainingArguments = false)]
        public string ServerName { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The primary access key for the docdb server on which to create the database and collection",
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromRemainingArguments = false)]
        public string PrimaryKey { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The name of the database to be created",
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromRemainingArguments = false)]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "The name of the collection to be created",
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = true,
            ValueFromRemainingArguments = false)]
        public string CollectionName { get; set; }


        private Uri DatabaseUri
        {
            get
            {
                return new Uri(string.Format("https://{0}.documents.azure.com:443/", ServerName));
            }
        }

        /// <summary>
        /// Executed when the cmdlet should process the pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.WriteVerbose(DatabaseUri.ToString());
            using (var client = new DocumentClient(
                DatabaseUri, PrimaryKey, ConnectionPolicy.Default, ConsistencyLevel.Eventual))
            {
                try
                {

                    ResourceResponse<Database> targetDatabase = null;
                    string dbSelfLink = null;

                    base.WriteProgress(
                        new ProgressRecord(0, "Create docdb database", "Starting"));

                    //verify db does not already exist 

                    bool databaseExists = DoesDatabaseExist(DatabaseName, client, out targetDatabase);
                    if (!databaseExists)
                    {
                        //create the database if it doesn't already exist
                        var db = new Database { Id = DatabaseName.ToLower() };
                        var dbtask = client.CreateDatabaseAsync(db);
                        dbtask.Wait();

                        base.WriteProgress(
                            new ProgressRecord(1, "Create docdb database", "Complete"));

                        //get the self link to the new database
                        dbSelfLink = dbtask.Result.Resource.SelfLink;
                    }
                    else
                    {
                        //notify the user and use the link from the existing database reference
                        WriteWarning("Database already exists");
                        dbSelfLink = targetDatabase.Resource.SelfLink;
                    }

                    //verify collection does not exist already
                    bool collectionExists = DoesCollectionExist(CollectionName, DatabaseName, client);
                    if (!collectionExists)
                    {
                        var coll = new DocumentCollection { Id = CollectionName };
                        //coll.IndexingPolicy.Automatic = true;

                        base.WriteProgress(
                            new ProgressRecord(2, "Create docdb collection", "Starting"));
                        var collTask = client.CreateDocumentCollectionAsync(dbSelfLink, coll);
                        collTask.Wait();
                        base.WriteProgress(
                            new ProgressRecord(3, "Create docdb collection", "Complete"));
                    }
                    else
                    {
                        WriteWarning("Collection already exists");
                    }
                }
                catch (Exception ex)
                {
                    base.WriteVerbose(ex.Message);
                    base.WriteError(
                        new ErrorRecord(ex, "ERROR_DOCDB", ErrorCategory.InvalidOperation, null));
                }

            }

        }

        /// <summary>
        /// Attempts to query for the collection specified.
        /// </summary>
        /// <param name="collectionName">The name of the collection to test</param>
        /// <param name="dbName">The name of the database where the collection might exist.</param>
        /// <param name="client">The document client to use to test for the collection</param>
        /// <returns>True if the collection can be queried or false if it cannot.</returns>
        private bool DoesCollectionExist(string collectionName, string dbName, DocumentClient client)
        {
            try
            {
                string collectionPath = String.Format("/dbs/{0}/colls/{1}", dbName.ToLower(), collectionName);
                var queryTask = client.ReadDocumentCollectionAsync(collectionPath);
                queryTask.Wait();

                return queryTask.Result.Resource != null;
            }

            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Tests for the existence of a database on the database server.
        /// </summary>
        /// <param name="dbName">The name of the database that might exist.</param>
        /// <param name="client">The documentClient to use to test the existence. This should be already configured for the database server</param>
        /// <param name="targetDatabase">A reference to the existing database if it was found to exist.</param>
        /// <returns></returns>
        private bool DoesDatabaseExist(string dbName, DocumentClient client, out ResourceResponse<Database> targetDatabase)
        {
            try
            {
                var queryTask = client.ReadDatabaseAsync("/dbs/" + dbName.ToLower());
                queryTask.Wait();
                targetDatabase = queryTask.Result;
                return queryTask.Result.Resource != null;
            }
            catch
            {
                targetDatabase = null;
                return false;
            }

        }
    }
}

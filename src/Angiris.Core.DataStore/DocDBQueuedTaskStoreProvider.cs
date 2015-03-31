namespace Angiris.Core.DataStore
{
    using Angiris.Core.Models;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

	 
    public class DocDBQueuedTaskStoreProvider<T> : IDisposable, INoSQLStoreProvider<T> where T : IQueuedTask
	{
        public DocDBQueuedTaskStoreProvider(string serviceEndpoint, string authKey, string databaseId, string collectionId)
        {
            this.HostName = serviceEndpoint;
            this.AuthKey = authKey;
            this.DatabaseId = databaseId;
            this.CollectionId = collectionId;
        }

        public void Initialize()
        {
            //perf tips: http://azure.microsoft.com/blog/2015/01/20/performance-tips-for-azure-documentdb-part-1-2/
            if(client == null)
            {
                client = new DocumentClient(new Uri(this.HostName), this.AuthKey, new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

                client.OpenAsync().Wait();
            }

            if (database == null)
                database = GetOrCreateDatabaseAsync(this.DatabaseId).GetAwaiter().GetResult();

            if (collection == null)
                collection = GetOrCreateCollectionAsync(database.SelfLink, this.CollectionId).GetAwaiter().GetResult();
        }

        public async Task<T> CreateEntity(T entity)
        {
            //dynamic docEntity = new { id = entity.TaskID, Data = entity};

            Document created = await client.CreateDocumentAsync(collection.SelfLink, entity);
            if (created != null)
                return entity;
            else
                return default(T);
        }

        public async Task<T> ReadEntity(string id)
        {
            return await Task.Run(() => {

                dynamic doc = client.CreateDocumentQuery(collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();

                if (doc != null)
                {
                    var entity = (T)doc;
                    return entity;
                }
                else
                    return default(T);
            });

        }

        public async Task<T> UpdateEntity(string id, T entity)
        {
            if (entity.TaskID.ToString() == id)
            {
                //dynamic docEntity = new { id = entity.TaskID, Data = entity };
                Document doc = client.CreateDocumentQuery(collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();
                if (doc != null)
                {
                    Document updated = await client.ReplaceDocumentAsync(doc.SelfLink, entity);
                    if (updated != null)
                        return entity;
                }
            }
            return default(T);            
        }

        public async Task DeleteEntity(string id)
        {
            Document doc = client.CreateDocumentQuery(collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();
            if (doc != null)
                await client.DeleteDocumentAsync(doc.SelfLink);
        }
 

        public string HostName
        {
            get;
            private set;
        }

        public string AuthKey
        {
            get;
            private set;
        }

        public string DatabaseId
        {
            get;
            private set;
        }

        public string CollectionId
        {
            get;
            private set;
        }

        DocumentClient client;
        Database database;
        DocumentCollection collection;

        public void Dispose()
        {
            client.Dispose();
        }


        private async Task<Database> GetOrCreateDatabaseAsync(string id)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }

            return database;
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();
            if (collection != null)
            {
                collection = await client.DeleteDocumentCollectionAsync(collection.SelfLink);
            }
        }
    }
}


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

	/// <summary>
	/// Obsoleted.
	/// </summary>
	/// <typeparam name="T"></typeparam>
    //public class DocDbQueuedTaskStoreProvider<T> :  INoSqlStoreProvider<T> where T : IQueuedTask
    //{
    //    public DocDbQueuedTaskStoreProvider(string serviceEndpoint, string authKey, string databaseId, string collectionId)
    //    {
    //        this.HostName = serviceEndpoint;
    //        this.AuthKey = authKey;
    //        this.DatabaseId = databaseId;
    //        this.CollectionId = collectionId;
    //    }

    //    public void Initialize()
    //    {
    //        //perf tips: http://azure.microsoft.com/blog/2015/01/20/performance-tips-for-azure-documentdb-part-1-2/
    //        if(_client == null)
    //        {
    //            _client = new DocumentClient(new Uri(this.HostName), this.AuthKey, new ConnectionPolicy
    //            {
    //                ConnectionMode = ConnectionMode.Direct,
    //                ConnectionProtocol = Protocol.Tcp
    //            });

    //            _client.OpenAsync().Wait();
    //        }

    //        if (_database == null)
    //            _database = GetOrCreateDatabaseAsync(this.DatabaseId).GetAwaiter().GetResult();

    //        if (_collection == null)
    //            _collection = GetOrCreateCollectionAsync(_database.SelfLink, this.CollectionId).GetAwaiter().GetResult();
    //    }

    //    public async Task<T> CreateEntity(T entity)
    //    {
    //        //dynamic docEntity = new { id = entity.TaskID, Data = entity};

    //        Document created = await _client.CreateDocumentAsync(_collection.SelfLink, entity);
    //        if (created != null)
    //            return entity;
    //        else
    //            return default(T);
    //    }

    //    public async Task<T> ReadEntity(string id)
    //    {
    //        return await Task.Run(() => {

    //            dynamic doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();

    //            if (doc != null)
    //            {
    //                var entity = (T)doc;
    //                return entity;
    //            }
    //            else
    //                return default(T);
    //        });

    //    }

    //    public async Task<T> UpdateEntity(string id, T entity)
    //    {
    //        if (entity.TaskId == id)
    //        {
    //            //dynamic docEntity = new { id = entity.TaskID, Data = entity };
    //            Document doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();
    //            if (doc != null)
    //            {
    //                Document updated = await _client.ReplaceDocumentAsync(doc.SelfLink, entity);
    //                if (updated != null)
    //                    return entity;
    //            }
    //        }
    //        return default(T);            
    //    }

    //    public async Task DeleteEntity(string id)
    //    {
    //        Document doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();
    //        if (doc != null)
    //            await _client.DeleteDocumentAsync(doc.SelfLink);
    //    }
 

    //    public string HostName
    //    {
    //        get;
    //        private set;
    //    }

    //    public string AuthKey
    //    {
    //        get;
    //        private set;
    //    }

    //    public string DatabaseId
    //    {
    //        get;
    //        private set;
    //    }

    //    public string CollectionId
    //    {
    //        get;
    //        private set;
    //    }

    //    DocumentClient _client;
    //    Database _database;
    //    DocumentCollection _collection;

    //    public void Dispose()
    //    {
    //        _client.Dispose();
    //    }


    //    private async Task<Database> GetOrCreateDatabaseAsync(string id)
    //    {
    //        Database database = _client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
    //        if (database == null)
    //        {
    //            database = await _client.CreateDatabaseAsync(new Database { Id = id });
    //        }

    //        return database;
    //    }

    //    private async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
    //    {
    //        DocumentCollection collection = _client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
    //        if (collection == null)
    //        {
    //            collection = await _client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
    //        }

    //        return collection;
    //    }

    //    public async Task DeleteCollectionAsync(string collectionId)
    //    {
    //        DocumentCollection collection = _client.CreateDocumentCollectionQuery(_database.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();
    //        if (collection != null)
    //        {
    //            await _client.DeleteDocumentCollectionAsync(collection.SelfLink);
    //        }
    //    }
    //}
}


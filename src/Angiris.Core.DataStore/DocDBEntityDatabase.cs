namespace Angiris.Core.DataStore
{
    using Angiris.Core.Models;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Client.TransientFaultHandling;
    using Microsoft.Azure.Documents.Client.TransientFaultHandling.Strategies;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

	 
    public class DocDBFlightEntityDatabase  
	{
        public DocDBFlightEntityDatabase(string serviceEndpoint, string authKey, string databaseId, string collectionId)
        {
            this.HostName = serviceEndpoint;
            this.AuthKey = authKey;
            this.DatabaseId = databaseId;
            this.CollectionId = collectionId;
        }


        IReliableReadWriteDocumentClient _client;
        Database _database;
        DocumentCollection _collection;
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            //perf tips: http://azure.microsoft.com/blog/2015/01/20/performance-tips-for-azure-documentdb-part-1-2/
            if(_client == null)
            {
                var increRetry = new Incremental(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200));
                var retryStrategy = new DocumentDbRetryStrategy(increRetry);

                _client = new DocumentClient(new Uri(this.HostName), this.AuthKey, new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                }).AsReliable(retryStrategy);

                _client.OpenAsync().Wait();
            }

            if (_database == null)
                _database = GetOrCreateDatabaseAsync(this.DatabaseId).GetAwaiter().GetResult();

            if (_collection == null)
                _collection = GetOrCreateCollectionAsync(_database.SelfLink, this.CollectionId).GetAwaiter().GetResult();

            IsInitialized = true;
        }

        public async Task<FlightResponse> CreateEntity(FlightResponse entity)
        {
            try
            {
                Document created = await _client.CreateDocumentAsync(_collection.SelfLink, entity);
                if (created != null)
                    return entity;
            }
            catch(DocumentClientException ex)
            {
                Trace.TraceError("CreateEntity:{0}, Error:{1}", entity.Id, ex.Message + "," + ex.StatusCode + "," + ex.ActivityId);
            }
            catch(Exception ex)
            {
                Trace.TraceError("CreateEntity:{0}, Error:{1}", entity.Id, ex.Message);
            }
            return default(FlightResponse);
            
        }

        public async Task<FlightResponse> ReadEntity(string id)
        {
            return await Task.Run(() => {

                dynamic doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();

                if (doc != null)
                {
                    var entity = (FlightResponse)doc;
                    return entity;
                }
                else
                    return default(FlightResponse);
            });

        }

        public async Task<FlightResponse> CreateOrUpdateEntity(FlightResponse entity)
        {
            Document doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == entity.Id).AsEnumerable().FirstOrDefault();
            if (doc != null)
            {
                try
                {
                    Document updated = await _client.ReplaceDocumentAsync(doc.SelfLink, entity);
                    if (updated != null)
                        return entity;
                    else
                        return default(FlightResponse);
                }
                catch (DocumentClientException ex)
                {
                    Trace.TraceError("CreateEntity:{0}, Error:{1}", entity.Id, ex.Message + "," + ex.StatusCode + "," + ex.ActivityId);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("CreateEntity:{0}, Error:{1}", entity.Id, ex.Message);
                }
                return default(FlightResponse);
            }
            else
            {
                return await this.CreateEntity(entity);
            }
         }

        public async Task DeleteEntity(string id)
        {
            Document doc = _client.CreateDocumentQuery(_collection.SelfLink).Where(d => d.Id == id).AsEnumerable().FirstOrDefault();
            if (doc != null)
                await _client.DeleteDocumentAsync(doc.SelfLink);
        }

        public async Task<IEnumerable<FlightResponse>> QueryEntities(string flightNumber, DateTime flightDate)
        {
            List<FlightResponse> results = new List<FlightResponse>();
            await Task.Run(() =>
            {

                var docId = FlightResponse.ToDistinctHash(flightNumber, flightDate);

                results = _client.CreateDocumentQuery<FlightResponse>(_collection.SelfLink).Where(d => d.Id == docId).ToList();
                
            });

            return results;
        }

        public async Task<IEnumerable<FlightResponse>> QueryEntities(string departure, string arrival, DateEpoch flightDate)
        {
            flightDate = flightDate.Date.Date;

            List<FlightResponse> results = new List<FlightResponse>();
            await Task.Run(() =>
            {
                results = _client.CreateDocumentQuery<FlightResponse>(_collection.SelfLink)
                    .Where(d => d.DepartureCity == departure && d.ArrivalCity == arrival && d.FlightDate.Epoch == flightDate.Epoch).ToList();

            });

            return results;
        }

        //        public string Company { get; set; }
        //// 航班号
        //public string FlightNumber { get; set; }
        //// 起飞日期
        //public DateEpoch FlightDate { get; set; }
        ////出发城市
        //public string DepartureCity { get; set; }
        //// 到达城市
        //public string ArrivalCity { get; set; }

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


        public void Dispose()
        {
            _client.Dispose();
        }


        private async Task<Database> GetOrCreateDatabaseAsync(string id)
        {
            Database database = _client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await _client.CreateDatabaseAsync(new Database { Id = id });
                //Trace.TraceError("")
            }

            return database;
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
        {
            DocumentCollection collection = _client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await _client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            DocumentCollection collection = _client.CreateDocumentCollectionQuery(_database.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();
            if (collection != null)
            {
                collection = await _client.DeleteDocumentCollectionAsync(collection.SelfLink);
            }
        }
    }
}


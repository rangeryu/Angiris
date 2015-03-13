namespace Angiris.Core.DataStore
{
	using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public class DataProviderFactory
	{
        public INoSQLStoreProvider<T> GetRedisQueuedTaskStore<T>() where T : IQueuedTask
		{
            string host = "Angiris-Demo-Cache.redis.cache.windows.net";
            string key = "kYi4cUJPM4o/jDEfWiXR89994u0xG9AMHbL/AyVMczw=";
            TimeSpan expiry = TimeSpan.FromMinutes(5);

            RedisQueuedTaskStoreProvider<T> provider = new RedisQueuedTaskStoreProvider<T>(host, key, expiry);
            return provider;

		}

        public INoSQLStoreProvider<T> GetDocDBQueuedTaskStore<T>() where T : IQueuedTask
        {
            string host = "	https://angiris-demo.documents.azure.com:443";
            string key = "dCvlAX1QGxPnjSqpcDsH0DdKu7zuOxvwAv9q1Zb9bQOnGcqyBQJheNAoQTz8YarSG+/Y0I6iCCVSdjz6IVV6Mw==";
            string databaseId = "EntityTaskResults";
            string collectionId = "QueuedTask";

            DocDBQueuedTaskStoreProvider<T> provider = new DocDBQueuedTaskStoreProvider<T>(host, key, databaseId, collectionId);
            return provider;

        }

	}
}


namespace Angiris.Core.DataStore
{
	using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public class DataProviderFactory
	{
        public static RedisQueuedTaskStoreProvider<T> GetRedisQueuedTaskStore<T>() where T : IQueuedTask
		{
            string host = "Angiris-Demo-Cache.redis.cache.windows.net";
            string key = "kYi4cUJPM4o/jDEfWiXR89994u0xG9AMHbL/AyVMczw=";
            TimeSpan expiry = TimeSpan.FromMinutes(30);
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            int dbIndex = 0;
            RedisQueuedTaskStoreProvider<T> provider = new RedisQueuedTaskStoreProvider<T>(connString,  expiry, dbIndex);
            return provider;
		}

        public static RedisFlightEntityDatabase GetRedisFlightEntityDatabase()
        {
            string host = "Angiris-Demo-Cache.redis.cache.windows.net";
            string key = "kYi4cUJPM4o/jDEfWiXR89994u0xG9AMHbL/AyVMczw=";
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            int dbIndex = 1;
            TimeSpan expiryAfterLight = TimeSpan.FromDays(2);

            RedisFlightEntityDatabase database = new RedisFlightEntityDatabase(connString, dbIndex, expiryAfterLight);
            return database;

        }

        public static RedisDaemonStatusProvider GetRedisDaemonStatusProvider()
        {
            string host = "Angiris-Demo-Cache.redis.cache.windows.net";
            string key = "kYi4cUJPM4o/jDEfWiXR89994u0xG9AMHbL/AyVMczw=";
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            string keyname = "Telemetry-DaemonStatus";
            int dbIndex = 2;
            RedisDaemonStatusProvider provider = new RedisDaemonStatusProvider(connString, dbIndex, keyname);
            return provider;
        }

        public static DocDBQueuedTaskStoreProvider<T> GetDocDBQueuedTaskStore<T>() where T : IQueuedTask
        {
            string host = "https://angiris-demo.documents.azure.com:443";
            string key = "dCvlAX1QGxPnjSqpcDsH0DdKu7zuOxvwAv9q1Zb9bQOnGcqyBQJheNAoQTz8YarSG+/Y0I6iCCVSdjz6IVV6Mw==";
            string databaseId = "EntityTaskResults";
            string collectionId = "QueuedTask";

            DocDBQueuedTaskStoreProvider<T> provider = new DocDBQueuedTaskStoreProvider<T>(host, key, databaseId, collectionId);
            return provider;

        }


	}
}


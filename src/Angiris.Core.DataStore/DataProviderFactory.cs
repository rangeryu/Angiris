namespace Angiris.Core.DataStore
{
using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

	public class DataProviderFactory
	{
        public static RedisQueuedTaskStoreProvider<T> GetRedisQueuedTaskStore<T>() where T : IQueuedTask
		{
            dynamic cfg = Config.ConfigMgr.ReleaseCfg.RedisQueuedTaskStore;

            string host = cfg.Host;
            string key = cfg.Key;
            TimeSpan expiry = TimeSpan.FromMinutes(cfg.ExpiryMins);
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            int dbIndex = cfg.DbIndex;
            RedisQueuedTaskStoreProvider<T> provider = new RedisQueuedTaskStoreProvider<T>(connString,  expiry, dbIndex);
            return provider;
		}

        public static RedisFlightEntityDatabase GetRedisFlightEntityDatabase()
        {
            dynamic cfg = Config.ConfigMgr.ReleaseCfg.RedisFlightEntityDatabase;

            string host = cfg.Host;
            string key = cfg.Key;
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            int dbIndex = cfg.DbIndex;
            TimeSpan expiryAfterFlight = TimeSpan.FromDays(cfg.ExpiryDays);

            RedisFlightEntityDatabase database = new RedisFlightEntityDatabase(connString, expiryAfterFlight, dbIndex);
            return database;

            
        }



        public static RedisDaemonStatusProvider GetRedisDaemonStatusProvider()
        {
            dynamic cfg = Config.ConfigMgr.ReleaseCfg.RedisDaemonStatus;

            string host = cfg.Host;
            string key = cfg.Key;
            string connString = string.Format("{0},ssl=true,password={1}", host, key);
            string keyname = cfg.KeyName;
            int dbIndex = cfg.DbIndex;
            RedisDaemonStatusProvider provider = new RedisDaemonStatusProvider(connString, dbIndex, keyname);
            return provider;
        }

        public static DocDbFlightEntityDatabase GetDocDBFlightEntityDatabase()
        {
            dynamic cfg = Config.ConfigMgr.ReleaseCfg.DocDbFlightEntityDatabase;

            string host = cfg.Host;
            string key = cfg.Key;
            string databaseId = cfg.DatabaseId;
            string collectionId = cfg.CollectionId;

            DocDbFlightEntityDatabase provider = new DocDbFlightEntityDatabase(host, key, databaseId, collectionId);
            return provider;

        }

        //public static DocDbQueuedTaskStoreProvider<T> GetDocDBQueuedTaskStore<T>() where T : IQueuedTask
        //{
        //    string host = "https://angiris-demo.documents.azure.com:443";
        //    string key = "dCvlAX1QGxPnjSqpcDsH0DdKu7zuOxvwAv9q1Zb9bQOnGcqyBQJheNAoQTz8YarSG+/Y0I6iCCVSdjz6IVV6Mw==";
        //    string databaseId = "EntityTaskResults";
        //    string collectionId = "QueuedTask";

        //    DocDbQueuedTaskStoreProvider<T> provider = new DocDbQueuedTaskStoreProvider<T>(host, key, databaseId, collectionId);
        //    return provider;

        //}


        private static readonly object syncRoot_docDBFlightEntityDatabase = new Object();

        private static DocDbFlightEntityDatabase _docDbFlightEntityDatabase;
        public static DocDbFlightEntityDatabase SingletonDocDbFlightEntityDatabase
        {
            get
            {
                if (_docDbFlightEntityDatabase == null || !_docDbFlightEntityDatabase.IsInitialized)
                {
                    lock (syncRoot_docDBFlightEntityDatabase)
                    {
                        if (_docDbFlightEntityDatabase == null || !_docDbFlightEntityDatabase.IsInitialized)
                        {
                            _docDbFlightEntityDatabase = GetDocDBFlightEntityDatabase();
                            _docDbFlightEntityDatabase.Initialize();
                        }
                    }
                }
                return _docDbFlightEntityDatabase;
            }
        }


        private static readonly object syncRoot_flightEntityDatabase = new Object();

        private static RedisFlightEntityDatabase _flightEntityDatabase;
        public static RedisFlightEntityDatabase SingletonFlightEntityDatabase
        {
            get
            {
                if (_flightEntityDatabase == null || !_flightEntityDatabase.IsInitialized )
                {
                    lock (syncRoot_flightEntityDatabase)
                    {
                        if (_flightEntityDatabase == null || !_flightEntityDatabase.IsInitialized)
                        {
                            _flightEntityDatabase = GetRedisFlightEntityDatabase();
                            _flightEntityDatabase.Initialize();
                        }
                    }
                }
                return _flightEntityDatabase;
            }
        }


        //private static object syncRoot_docDBQueuedTaskStore = new Object();

        //private static DocDBQueuedTaskStoreProvider<FlightCrawlEntity> _docDBQueuedTaskStore;
        //public static DocDBQueuedTaskStoreProvider<FlightCrawlEntity> SingletonDocDBQueuedTaskStore
        //{
        //    get
        //    {
        //        if (_docDBQueuedTaskStore == null)
        //        {
        //            lock (syncRoot_docDBQueuedTaskStore)
        //            {
        //                if (_docDBQueuedTaskStore == null)
        //                {
        //                    _docDBQueuedTaskStore = DataProviderFactory.GetDocDBQueuedTaskStore<FlightCrawlEntity>();
        //                    _docDBQueuedTaskStore.Initialize();
        //                }
        //            }
        //        }
        //        return _docDBQueuedTaskStore;
        //    }
        //}

        private static readonly object syncRoot_RedisQueuedTaskStore = new Object();
        //INoSQLStoreProvider<FlightCrawlEntity> cacheStore;

        private static RedisQueuedTaskStoreProvider<FlightCrawlEntity> _redisQueuedTaskStore;
        public static RedisQueuedTaskStoreProvider<FlightCrawlEntity> SingletonRedisQueuedTaskStore
        {
            get
            {
                if (_redisQueuedTaskStore == null || !_redisQueuedTaskStore.IsInitialized)
                {
                    lock (syncRoot_RedisQueuedTaskStore)
                    {
                        if (_redisQueuedTaskStore == null || !_redisQueuedTaskStore.IsInitialized)
                        {
                            _redisQueuedTaskStore = DataProviderFactory.GetRedisQueuedTaskStore<FlightCrawlEntity>();
                            _redisQueuedTaskStore.Initialize();
                        }
                    }
                }
                return _redisQueuedTaskStore;
            }
        }


        private static readonly object syncRoot_RedisDaemonStatusProvider = new Object();

        private static RedisDaemonStatusProvider _redisDaemonStatusProvider;
        public static RedisDaemonStatusProvider SingletonRedisDaemonStatusProvider
        {
            get
            {
                if (_redisDaemonStatusProvider == null || !_redisDaemonStatusProvider.IsInitialized)
                {
                    lock (syncRoot_RedisDaemonStatusProvider)
                    {
                        if (_redisDaemonStatusProvider == null || !_redisDaemonStatusProvider.IsInitialized)
                        {
                            _redisDaemonStatusProvider = DataProviderFactory.GetRedisDaemonStatusProvider();
                            _redisDaemonStatusProvider.Initialize();
                        }
                    }
                }
                return _redisDaemonStatusProvider;
            }
        }
	}
}


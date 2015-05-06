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
            int expiryMins = cfg.ExpiryMins;
            TimeSpan expiry = TimeSpan.FromMinutes(expiryMins);
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
            int expiryDays = cfg.ExpiryDays;
            TimeSpan expiryAfterFlight = TimeSpan.FromDays(expiryDays);

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

        public static DocDbFlightEntityDatabase GetDocDbFlightEntityDatabase()
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

	    public static void InitializeAll()
	    {

	        InitSingletonDocDbFlightEntityDatabase();
            InitSingletonRedisFlightEntityDatabase();

            InitSingletonRedisQueuedTaskStore();
	        InitSingletonRedisDaemonStatusProvider();
	    }


        

        private static DocDbFlightEntityDatabase _docDbFlightEntityDatabase;
        public static DocDbFlightEntityDatabase SingletonDocDbFlightEntityDatabase
        {
            get
            {
                InitSingletonDocDbFlightEntityDatabase();
                return _docDbFlightEntityDatabase;
            }
        }

        private static readonly object syncRoot_docDBFlightEntityDatabase = new Object();
	    private static void InitSingletonDocDbFlightEntityDatabase()
	    {
            if (_docDbFlightEntityDatabase == null || !_docDbFlightEntityDatabase.IsInitialized)
            {
                lock (syncRoot_docDBFlightEntityDatabase)
                {
                    if (_docDbFlightEntityDatabase == null || !_docDbFlightEntityDatabase.IsInitialized)
                    {
                        _docDbFlightEntityDatabase = GetDocDbFlightEntityDatabase();
                        _docDbFlightEntityDatabase.Initialize();
                    }
                }
            }
	    }


        

        private static RedisFlightEntityDatabase _redisFlightEntityDatabase;
        public static RedisFlightEntityDatabase SingletonRedisFlightEntityDatabase
        {
            get
            {
                InitSingletonRedisFlightEntityDatabase();
                return _redisFlightEntityDatabase;
            }
        }

        private static readonly object syncRoot_RedisflightEntityDatabase = new Object();
        private static void InitSingletonRedisFlightEntityDatabase()
	    {
            if (_redisFlightEntityDatabase == null || !_redisFlightEntityDatabase.IsInitialized)
            {
                lock (syncRoot_RedisflightEntityDatabase)
                {
                    if (_redisFlightEntityDatabase == null || !_redisFlightEntityDatabase.IsInitialized)
                    {
                        _redisFlightEntityDatabase = GetRedisFlightEntityDatabase();
                        _redisFlightEntityDatabase.Initialize();
                    }
                }
            }
	    }
 

        private static RedisQueuedTaskStoreProvider<FlightCrawlEntity> _redisQueuedTaskStore;
        public static RedisQueuedTaskStoreProvider<FlightCrawlEntity> SingletonRedisQueuedTaskStore
        {
            get
            {
                InitSingletonRedisQueuedTaskStore();
                return _redisQueuedTaskStore;
            }
        }

        private static readonly object syncRoot_RedisQueuedTaskStore = new Object();
	    private static void InitSingletonRedisQueuedTaskStore()
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
	    }




        private static RedisDaemonStatusProvider _redisDaemonStatusProvider;
        public static RedisDaemonStatusProvider SingletonRedisDaemonStatusProvider
        {
            get
            {
                InitSingletonRedisDaemonStatusProvider();
                return _redisDaemonStatusProvider;
            }
        }

        private static readonly object syncRoot_RedisDaemonStatusProvider = new Object();
	    private static void InitSingletonRedisDaemonStatusProvider()
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
	    }


	}
}


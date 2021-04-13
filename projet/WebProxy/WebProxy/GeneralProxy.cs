using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebProxy
{
    class GeneralProxy<T> where T : class, new()
    {
        ObjectCache cache = MemoryCache.Default;
        CacheItemPolicy policy = new CacheItemPolicy();
        DateTimeOffset dt_default = ObjectCache.InfiniteAbsoluteExpiration;

        public GeneralProxy() {}

        public async Task<string> Initialize(string link)
        {
            cache = MemoryCache.Default;
            HttpClient client = new HttpClient();
            try
            {
                return await client.GetStringAsync(link + "?apiKey=496571058607f1ccae3899d63be166f0058bca6e");                                     
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find link : " + link);
            }
            return null;
        }

        public DateTimeOffset Dt_default
        {
            get => dt_default;
            set
            {
                dt_default = value;
            }
        }

        public T Get(string cacheItem)
        {
            T item = (T)cache.Get(cacheItem);

            if (item == null)
                item = generateNewItem(cacheItem, dt_default);
            
            return item;
        }

        public T Get(string cacheItem, double dt_seconds)
        {
            T item = (T)cache.Get(cacheItem);

            if (item == null)
                item = generateNewItem(cacheItem, DateTimeOffset.Now.AddSeconds(dt_seconds));
            
            return item;
        }

        public T Get(string cacheItem, DateTimeOffset dt)
        {
            T item = (T)cache.Get(cacheItem);

            if (item == null)
                item = generateNewItem(cacheItem, dt);

            return item;

        }

        public T generateNewItem(string cacheItem, DateTimeOffset dt)
        {
            T item = new T();

            policy = new CacheItemPolicy
            {
                AbsoluteExpiration = dt,
            };
            cache.Add(cacheItem, item, policy);
            return item;
        }

        public void Set(string cacheItem, T item)
        {
            policy = new CacheItemPolicy
            {
                AbsoluteExpiration = dt_default,
            };
            cache.Add(cacheItem, item, policy);
        }

        public void Set(string cacheItem, T item, double dt_seconds)
        {
            policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(dt_seconds),
            };
            cache.Add(cacheItem, item, policy);
        }

        public void Set(string cacheItem, T item, DateTimeOffset dt)
        {
            policy = new CacheItemPolicy
            {
                AbsoluteExpiration = dt,
            };
            cache.Add(cacheItem, item, policy);
        }

        public List<T> getAllCache()
        {
            List<T> cacheList = new List<T>();
            foreach (var item in cache)
            {
                cacheList.Add((T)item.Value);
            }
            return cacheList;
        }

    }
}

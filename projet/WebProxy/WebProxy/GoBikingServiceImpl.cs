using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebProxy
{
    public class GoBikingServiceImpl : IGoBikingService
    {

        GeneralProxy<JCDecauxItem> proxy = new GeneralProxy<JCDecauxItem>();
      
        public GoBikingServiceImpl()
        {
            
            Task<string> init = Task.Run(() => proxy.Initialize("https://api.jcdecaux.com/vls/v3/stations"));
            init.Wait();

            List<JCDecauxItem> items = JsonConvert.DeserializeObject<List<JCDecauxItem>>(init.Result);

            foreach(JCDecauxItem item in items)
            {
                proxy.Set(item.name, item, ObjectCache.InfiniteAbsoluteExpiration);
            }
        }

        public List<JCDecauxItem> getStationByCityName(string name)
        {
            List<JCDecauxItem> items = proxy.getAllCache();
            List<JCDecauxItem> results = new List<JCDecauxItem>();

            foreach (JCDecauxItem item in items)
            {
                if (item.contractName.Equals(name.ToLower()))
                {
                    results.Add(item);
                }              
            }
            return results;
        }
    }
}

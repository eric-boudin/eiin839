using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace myFirstRest
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static string responseBody = "";
        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    responseBody = await client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations?contract="+ args[0] +"&apiKey=496571058607f1ccae3899d63be166f0058bca6e");
                }
                else
                {
                    responseBody = await client.GetStringAsync("https://api.jcdecaux.com/vls/v3/contracts?&apiKey=496571058607f1ccae3899d63be166f0058bca6e");
                }
                
                //string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below

                dynamic obj = JsonConvert.DeserializeObject(responseBody);

                Console.WriteLine(obj);
                Console.ReadKey();
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("C'est cassé !");
            }
        }
    }
}

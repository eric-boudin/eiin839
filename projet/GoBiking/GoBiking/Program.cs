using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using GoBiking.RoutingService;

namespace GoBiking
{
    class Program
    {
        static IRoutingService api = new RoutingServiceClient();
        static bool debug = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenue sur Let's Go Biking !");

            string start = "487 Rue Jean Queillau";
            string destination = "140 avenue Viton";
            string ville = "Marseille";

            if(!debug)
            {
                Console.WriteLine("Dans quelle ville êtes-vous ?");
                ville = Console.ReadLine();
                Console.WriteLine("Quelle est votre addresse de départ ? (sans le code postal et la ville)");
                start = Console.ReadLine();
                Console.WriteLine("Quelle est votre addresse de destination ? (sans le code postal et la ville)");
                destination = Console.ReadLine();
            }           

            string path = api.getDirection(start , destination, ville);

            Console.WriteLine(path);

            Console.ReadLine();
        }
    }
}

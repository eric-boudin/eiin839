using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoBiking.ServiceReference1;

namespace GoBiking
{
    class Program
    {
        static void Main(string[] args)
        {
            IService1 serv = new Service1Client();

            

        }

        void central()
        {
            Console.WriteLine("Bienvenue sur Let's Go Biking");
        }

    }
}

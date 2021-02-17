using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exo5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(mymethod(args));
        }

        static string mymethod(string[] args)  
        {   if (args.Length >= 2)
                return "<HTML><BODY> Hello " + args[0] + " et " + args[1] + ". Generer depuis un programme externe.</BODY></HTML>";
            else
                return "Not enough args";
        }
    }
}

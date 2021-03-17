using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice
{
    class Program
    {
        static void Main(string[] args)
        {
            //calculator.CalculatorSoap c = new calculator.CalculatorSoapClient();
            CMONSERVICE.MathsOperationsClient c = new CMONSERVICE.MathsOperationsClient();
            Console.WriteLine(c.Add(11, 11));
            Console.WriteLine(c.Substract(11, 11));
            Console.WriteLine(c.Multiply(11,11));
            Console.WriteLine(c.Divide(4,2));
            Console.ReadLine();
        }
    }
}

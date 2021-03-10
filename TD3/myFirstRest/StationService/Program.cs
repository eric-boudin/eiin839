using System;
using System.Diagnostics;
using System.IO;

namespace StationService
{
    class Program
    {
        static void Main(string[] args)
        {
           
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"../../../myFirstRest/bin/Debug/myFirstRest.exe"; // Specify exe name.
            start.Arguments = args[0]; // Specify arguments.
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                    Console.ReadLine();
                }
            }
        }
    }
}

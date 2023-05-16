using System;
using System.ServiceModel;


namespace Wcf.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(PingService));
            serviceHost.Open();

            Console.WriteLine("Service is running. Press any key to exit...");
            Console.ReadKey();
        }
    }
}

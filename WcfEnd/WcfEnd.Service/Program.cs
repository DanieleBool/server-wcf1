using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfEnd.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceHost = new WebServiceHost(typeof(GestoreService));
            serviceHost.Open();

            Console.WriteLine("Service is running. Press any key to exit...");
            Console.ReadKey();
        }
    }
}

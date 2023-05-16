using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Xml.Serialization;
using WcfEnd.Common;
using WcfEnd.Common.DTO;

namespace WcfEnd.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Method 1
            using (var channelFactory = new ChannelFactory<IGestore>("RestEndpoint"))
            {
                var proxy = channelFactory.CreateChannel();
                Console.WriteLine($"Calling {nameof(IGestore.Ping)} over http via ChannelFactory.");
                var reply = proxy.Ping(Guid.NewGuid().ToString());
                Console.WriteLine($"\tService replied with: '{reply.Message}'");
                Console.WriteLine($"Calling {nameof(IGestore.Ping2)} over http via ChannelFactory.");
                reply = proxy.Ping2(Guid.NewGuid().ToString());
                Console.WriteLine($"\tService replied with: '{reply.Message}'");
            }

            // Method 2 - HTTP GET
            var ser = new XmlSerializer(typeof(CustomData));
            var req = WebRequest.Create($"http://localhost:9999/GestoreService/Ping?message={Guid.NewGuid().ToString()}");

            Console.WriteLine($"Calling {nameof(IGestore.Ping)} over http via HttpWebRequest (GET)");

            using (var resp = req.GetResponse())
            {
                var ret = (CustomData)ser.Deserialize(resp.GetResponseStream());
                Console.WriteLine($"\tService replied with: '{ret.Message}'");
            }

            // HTTP POST
            ser = new XmlSerializer(typeof(string));
            req = WebRequest.Create($"http://localhost:9999/GestoreService/Ping2");
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = "text/xml";

            ser.Serialize(req.GetRequestStream(), Guid.NewGuid().ToString());

            Console.WriteLine($"Calling {nameof(IGestore.Ping)} over http via HttpWebRequest (POST)");

            using (var resp = req.GetResponse())
            {
                ser = new XmlSerializer(typeof(CustomData));
                var ret = (CustomData)ser.Deserialize(resp.GetResponseStream());
                Console.WriteLine($"\tService replied with: '{ret.Message}'");
            }

            // Method 3 - HTTP GET
            string uri = $"http://localhost:9999/GestoreService/Ping?message={Guid.NewGuid().ToString()}";
            Console.WriteLine("Opening the following URI with the default browser:");
            Console.WriteLine($"\t{uri}");

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(uri);
            p.Start();


        }
    }
}

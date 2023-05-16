using System;

using System.ServiceModel;
using Wcf.Common;

namespace Wcf.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {


            using (var channelFactory = new ChannelFactory<Iestore>("NetTcpEndpoint"))
            {
                var proxy = channelFactory.CreateChannel();
                string msg = Guid.NewGuid().ToString();
                Console.WriteLine($"Calling '{nameof(Iestore.Ping)}' over TCP with message '{msg}' via ChannelFactory.");
                proxy.Ping(msg);
            }

            using (var channelFactory = new ChannelFactory<Iestore>("NamedPipeEndpoint"))
            {
                var proxy = channelFactory.CreateChannel();
                string msg = Guid.NewGuid().ToString();
                Console.WriteLine($"Calling '{nameof(Iestore.Ping)}' over named pipe with message '{msg}' via ChannelFactory.");
                proxy.Ping(msg);
            }
        }
    }
}

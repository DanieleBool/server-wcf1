using System;
using Wcf.Common;


namespace Wcf.Service
{
    internal class PingService : Iestore
    {


        public void Ping(string message)
        {
            Console.WriteLine($"Operation '{nameof(Ping)}' was called with argument '{message}'.");
        }
    }
}
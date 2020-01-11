using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ip = "127.0.0.1";
            var port = 3000;

            var server = new Server(IPAddress.Parse(ip), port);
            server.Run();
        }
    }
}
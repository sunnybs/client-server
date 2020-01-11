using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 3000;
        private static readonly ManualResetEvent mainThreadState = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen(100);

            while (true)
            {
                mainThreadState.Reset();
                listener.BeginAccept(ListenClient, listener);
                mainThreadState.WaitOne();
            }
        }

        private static void ListenClient(IAsyncResult state)
        {
            var connection = ((Socket) state.AsyncState).EndAccept(state);
            mainThreadState.Set();
            var worker = new Connection(connection);

            while (true)
            {
                if (worker.ReceiveMessage(out string result))
                {
                    result =  
                }
                else
                {

                }



            }
        }
    }
}
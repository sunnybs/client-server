using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 3000;
        private static readonly ManualResetEvent mainThreadState = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen(10);

            while (true)
            {
                mainThreadState.Reset();
                listener.BeginAccept(ListenClient, listener);
                mainThreadState.WaitOne();
            }
        }

        private static void ListenClient(IAsyncResult state)
        {
            var client = ((Socket) state.AsyncState).EndAccept(state);
            mainThreadState.Set();
            
            while (true)
            {
                var buffer = new byte[256];
                var data = new StringBuilder();
                do
                {
                    var size = client.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                } while (client.Available > 0);

                Console.WriteLine($"Принято от клиента: {data}");
                client.Send(Encoding.UTF8.GetBytes($"Отправлено {DateTime.Now}"));
            }

            //listener.Shutdown(SocketShutdown.Both);
            //listener.Close();
        }
    }
}
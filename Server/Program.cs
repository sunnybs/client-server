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
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 3000;
        private static readonly ManualResetEvent mainThreadState = new ManualResetEvent(false);
        private static readonly List<ClientConnection> _connectedClients = new List<ClientConnection>();

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
            ClientConnection currentClient = null;
            try
            {
                var connection = ((Socket)state.AsyncState).EndAccept(state);
                mainThreadState.Set();
                currentClient = new ClientConnection(connection);
                lock (_connectedClients)
                    _connectedClients.Add(currentClient);
                
                while (true)
                {
                    var receivedData = currentClient.GetMessage();
                    Console.WriteLine($"Принято от клиента: {receivedData}");
                    
                    foreach (var client in _connectedClients)
                    {
                        client.Send(receivedData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Клиент отключился.");
                mainThreadState.Set();
                currentClient?.Disconnect();

                lock (_connectedClients)
                {
                    if (_connectedClients.Contains(currentClient))
                        _connectedClients.Remove(currentClient);
                }
            }
        }
    }
}
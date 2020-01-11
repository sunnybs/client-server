using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Connection;

namespace Server
{
    public class Server
    {
        private readonly TcpListener server;
        private readonly ManualResetEvent mainThreadState = new ManualResetEvent(false);

        private readonly ConcurrentDictionary<Guid, ClientConnection> connectedClients =
            new ConcurrentDictionary<Guid, ClientConnection>();

        public Server(IPAddress ipAddress, int port)
        {
            server = new TcpListener(ipAddress, port);
        }

        public void Run()
        {
            server.Start();

            while (true)
            {
                mainThreadState.Reset();
                server.BeginAcceptTcpClient(ListenClient, server);
                mainThreadState.WaitOne();
            }
        }

        private void ListenClient(IAsyncResult state)
        {
            Console.WriteLine("Клиент подключился.");
            ClientConnection currentClient = null;
            try
            {
                var connection = ((TcpListener) state.AsyncState).EndAcceptTcpClient(state);
                mainThreadState.Set();
                currentClient = new ClientConnection(connection);
                connectedClients.TryAdd(currentClient.Id, currentClient);

                while (true)
                {
                    var receiveTask = Task.Run(async () => await currentClient.GetMessageAsync());
                    receiveTask.Wait();
                    var receivedData = receiveTask.Result;

                    Console.WriteLine($"Принято от клиента: {receivedData}");

                    foreach (var client in connectedClients.Values)
                    {
                        Task.Run(async () => await client.SendAsync(receivedData));
                    }
                }
            }
            catch (Exception e)
            {
                var logMessage = e.Message
                    .Contains("Удаленный хост принудительно разорвал существующее подключение")
                    ? "Клиент отключился."
                    : $"Ошибка при обработке клиента: {e.Message}";

                Console.WriteLine(logMessage);
            }
            finally
            {
                mainThreadState.Set();
                if (currentClient != null)
                    DisposeClient(currentClient);
            }
        }

        private void DisposeClient(ClientConnection client)
        {
            client.Disconnect();
            connectedClients.TryRemove(client.Id, out client);
        }
    }
}
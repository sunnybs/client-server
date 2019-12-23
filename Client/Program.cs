using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 3000;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите сообщение:");
                var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var message = Console.ReadLine();
                var data = Encoding.UTF8.GetBytes(message);

                if (!clientSocket.Connected)
                    clientSocket.Connect(endPoint);

                clientSocket.Send(data);
                
                var buffer = new byte[256];
                var response = new StringBuilder();

                do
                {
                    var size = clientSocket.Receive(buffer);
                    response.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (clientSocket.Available > 0);

                Console.WriteLine(response.ToString());
            }
        }
    }
}

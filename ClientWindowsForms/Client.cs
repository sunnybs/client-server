using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Connection;

namespace ClientWindowsForms
{
    public class Client
    {
        private readonly TcpClient server = new TcpClient();

        public void Connect(string ip, int port) => server.Connect(IPAddress.Parse(ip), port);

        public string Receive()
        { 
            var connection = new ClientConnection(server);
            return connection.GetMessageAsync().Result;
        }

        public void SendMessage(string message)
        {
            var connection = new ClientConnection(server);
            connection.SendAsync(message);
        }
        

    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientConnection
    {
        public Guid Id { get; }
        private readonly Socket _connection;

        public ClientConnection(Socket connection)
        {
            _connection = connection;
            Id = Guid.NewGuid();
        }

        public void Send(string message) => _connection.Send(Encoding.UTF8.GetBytes(message));

        public string GetMessage()
        {
            var buffer = new byte[256];
            var data = new StringBuilder();
            do
            {
                var size = _connection.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (_connection.Available > 0);

            return data.ToString();
        }

        public void Disconnect() => _connection.Close();
    }
}

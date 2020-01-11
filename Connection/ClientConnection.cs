using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public class ClientConnection
    {
        public Guid Id { get; }
        private readonly TcpClient connection;

        public ClientConnection(TcpClient connection)
        {
            this.connection = connection;
            Id = Guid.NewGuid();
        }

        public async Task SendAsync(string message)
        {
            var stream = connection.GetStream();
            var messageData = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageData, 0, messageData.Length);
        }

        public async Task<string> GetMessageAsync()
        {
            const int bufferSize = 256;
            var buffer = new byte[bufferSize];
            var data = new StringBuilder();
            var stream = connection.GetStream();
            do
            {
                var size = await stream.ReadAsync(buffer, 0, bufferSize);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (stream.DataAvailable);

            return data.ToString();
        }

        public void Disconnect() => connection.Close();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ServerTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private readonly List<string> results = new List<string>();

        [Test]
        public void Test1()
        {
            var testMessages = Enumerable.Range(0, 100).Select(num => num.ToString());
            foreach (var testMessage in testMessages)
            {
                SendAndGetResponse(testMessage);
            }

            var check = testMessages.Except(results);
            var check1 = results.Except(testMessages);

            Assert.True(!testMessages.Except(results).Any() && !results.Except(testMessages).Any());
        }

        private void SendAndGetResponse(string message)
        {
            var _ip = "127.0.0.1";
            var _port = 3000;
            var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
            } while (clientSocket.Available > 0);

            results.Add(response.ToString());
        }
    }
}
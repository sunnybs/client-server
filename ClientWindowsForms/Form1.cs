using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientWindowsForms
{
    public partial class Form1 : Form
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 3000;

        private readonly Socket _server;

        public Form1()
        {
            InitializeComponent();

            var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Connect(endPoint);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            var name = NameTextBox.Text;
            var message = MessageTextBox.Text;

            var data = Encoding.UTF8.GetBytes($"{name}: {message}");
            _server.Send(data);
        }

        public string ReceiveDataFromServer()
        {
            var response = new StringBuilder();
            var buffer = new byte[256];
            do
            {
                var size = _server.Receive(buffer);
                response.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (_server.Available > 0);

            return response.ToString();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                var result = await Task.Factory.StartNew<string>(ReceiveDataFromServer);
                ChatHub.Text += "\n" + result;
            }
        }
    }
}

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
        private readonly Client client;

        public Form1()
        {
            InitializeComponent();

            client = new Client();
            client.Connect("127.0.0.1",3000);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            var name = NameTextBox.Text;
            var message = MessageTextBox.Text;

            client.SendMessage($"{name}: {message}");
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                var result = await Task.Factory.StartNew(client.Receive);
                ChatHub.Text += "\n" + result;
            }
        }
    }
}

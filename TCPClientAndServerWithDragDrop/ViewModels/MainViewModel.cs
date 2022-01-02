using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TCPClientAndServerWithDragDrop.Command;

namespace TCPClientAndServerWithDragDrop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainWindow MainView { get; set; }

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }

        public RelayCommand DropCommand { get; set; }

        public string[] Text { get; set; }


        TcpClient client = new TcpClient();

        public MainViewModel()
        {

            Text = new string[] { };
            DropCommand = new RelayCommand((sender) =>
            {
                MainView.DragDropListBx.Drop += DragDrop;
            });

            ConnectCommand = new RelayCommand((sender) =>
            {
                Send();

            }, (pred) =>
            {
                if (MainView.ClientName.Text != string.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            DisconnectCommand = new RelayCommand((sender) =>
            {
                client.Dispose();
                MessageBox.Show("Disconnect . . .");
            });
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                MainView.DragDropListBx.Items.Add(file);
            }

            Text = files;
            e.Effects = DragDropEffects.All;
        }

        private void Send()
        {
            var ipadress = IPAddress.Parse("192.168.1.109");
            var port = 1234;
            var ep = new IPEndPoint(ipadress, port);

            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    foreach (var t in Text)
                    {
                        var clientName = MainView.ClientName.Text;
                        var text = t + " " + clientName;
                        var stream = client.GetStream();
                        var bw = new BinaryWriter(stream);
                        bw.Write(text);
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}

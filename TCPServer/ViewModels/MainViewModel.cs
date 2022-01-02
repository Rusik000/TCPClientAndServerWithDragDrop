using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TCPServer.Command;

namespace TCPServer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainWindow MainView { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand StartServerCommand { get; set; }


        private string _clientName;

        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; OnPropertyChanged(); }
        }

        public string SendedMessage { get; set; }

        static TcpListener listener;

        static BinaryReader br = null;



        public MainViewModel()
        {
            SendedMessage = "";

            StartServerCommand = new RelayCommand((sender) =>
            {
                Thread thread = new Thread(() =>
                {
                    Receive();

                });
                thread.Start();
            });

            PlayCommand = new RelayCommand((sender) =>
            {
                Thread thread = new Thread(() =>
                {
                    ShowFile();
                });
                thread.Start();
            });
        }

        private void ShowFile()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Process.Start($@"{SendedMessage}");
            });
        }

        private void Receive()
        {
            try
            {
                var ip = IPAddress.Parse("192.168.1.109");

                var port = 1234;

                var ep = new IPEndPoint(ip, port);

                listener = new TcpListener(ep);


                listener.Start();


                while (true)
                {
                    var client = listener.AcceptTcpClient();                  
                    var stream = client.GetStream();
                    br = new BinaryReader(stream);

                    var msg = br.ReadString();
                    string[] subMessage = msg.Split(' ');
                    SendedMessage = subMessage[0];
                    ClientName = "From" + " " + subMessage[1];
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MainView.FromClient.Text = ClientName;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}

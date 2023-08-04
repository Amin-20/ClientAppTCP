using ClientAppTCP.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientAppTCP.Models;
using System.Windows;

namespace ClientAppTCP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand ConnectServerCommand { get; set; }

        private string nameTextBlock;

        public string NameTextBlock
        {
            get { return nameTextBlock; }
            set { nameTextBlock = value; OnPropertyChanged(); }
        }


        private string serverStatus;

        public string ServerStatus
        {
            get { return serverStatus; }
            set { serverStatus = value; OnPropertyChanged(); }
        }
        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(); }
        }



        public MainViewModel()
        {
            ConnectServerCommand = new RelayCommand((obj) =>
            {
                if (NameTextBlock != null)
                {
                    Task.Run(() =>
                    {
                        var client = new TcpClient();
                        var ip = IPAddress.Parse("10.1.18.2");
                        var port = 27001;

                        var ep = new IPEndPoint(ip, port);


                        try
                        {
                            client.Connect(ep);
                            ServerStatus = "Connected Server";
                            User user = new User
                            {
                                LocalAdress = client.Client.LocalEndPoint.ToString(),
                                Name = NameTextBlock
                            };
                            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(user);
                            if (client.Connected)
                            {
                                var writer = Task.Run(() =>
                                {
                                    var stream = client.GetStream();
                                    var bw = new BinaryWriter(stream);
                                    bw.Write(jsonString);
                                });

                                var reader = Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        var stream = client.GetStream();
                                        var br = new BinaryReader(stream);
                                        MessageText += br.ReadString() + "\n";
                                    }
                                });

                                Task.WaitAll(writer, reader);

                            }
                        }
                        catch (Exception ex)
                        {
                            ServerStatus = ex.Message;
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Enter the name", "Name Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
        }
    }
}

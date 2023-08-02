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
using ClientAppTCP.Views.UserControls;

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

                        User user = new User
                        {
                            Name = NameTextBlock,
                            IpAdress = "10.1.18.2",
                            Port = 27001
                        };

                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(user);

                        try
                        {
                            client.Connect(ep);
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
                                        var messageUC = new MessageUC();
                                        var messageUCvm = new MessageUCViewModel();
                                        messageUCvm.MessageText = br.ReadString();
                                        App.wrapPanel.Children.Add(messageUC);
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
                    MessageBox.Show("Enter the name", "Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
        }
    }
}

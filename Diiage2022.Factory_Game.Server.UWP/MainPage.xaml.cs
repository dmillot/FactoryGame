using Diiage2022.Factory_Games.Client.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Diiage2022.Factory_Game.Server.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public TcpClient Tcpclient { get; set; }
        public MainPage()
        {
            Tcpclient = new TcpClient();
            ConnectToServer();
            this.InitializeComponent();
        }

        public bool ConnectToServer()
        {
            bool result = false;//Variable for test the connection
            try
            {
                Communication com = new Communication();//intantiate the objet communication.      
                Tcpclient.Connect(IPAddress.Parse("127.0.0.1"), 7777);
                result = true;
            }
            catch (Exception e)
            {
                //exception
            }
            return result;//Send the variable for the test of connection.
        }

        public void sendData(Communication communication)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                string msg = JsonConvert.SerializeObject(communication);//Create a string message and serialise com to json 
                byte[] buffer = new byte[1000];
                buffer = encoding.GetBytes(msg);// I transform my stfing into byte to send it in TCP.
                Tcpclient.GetStream().Write(buffer, 0, buffer.Length);// I get the connection flow between the client and the server
            }
            catch(Exception e)
            {

            }
        }

        private void ButtonLaunchGame_Click(object sender, RoutedEventArgs e)
        {
            int money = int.Parse(TextBoxMoneyStart.Text);
            int numberTurns = int.Parse(TextBoxNumberOfTurns.Text);

            Configuration config = new Configuration();
            config.PlayerMoney = money;
            config.NumberOfTurns = numberTurns;

            Communication communication = new Communication();
            communication.RequestType = RequestType.ADMIN_START_GAME; // start game type
            communication.Configuration = config;

            if (Tcpclient.Connected)
            {
                sendData(communication);
            }
        }
    }
}

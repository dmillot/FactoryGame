using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Sockets;
using System.Text;
using System.Net;
using Diiage2022.Factory_Games.Client.Services;
using Windows.Storage;
using System.Threading;
using System.Diagnostics;

namespace Diiage2022.Factory_Games.Client.UWP
{
    public sealed partial class MainPage : Page, MainPageInterface
    {
        public string reponse;
        


        public MainPage()
        {
            this.InitializeComponent();
            SynchronizationContext context = SynchronizationContext.Current;
            ((App)App.Current).servicesClass.SetMainPageInterface(this, context);
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {

            //Send the contains of TextBox(username).
            if (((App)App.Current).servicesClass.Connection(textBox_Username.Text, textBox_AddressIP.Text))
            {
                //this.Frame.Navigate(typeof(RoomScreen));
            }
            else
            {
                Debug.Write("erreur");
                ErreurName.Text = "Server close, please try again.";
                if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }

            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            StandardPopup.IsOpen = false;
            this.Frame.Navigate(typeof(MainPage));
        }

        public void ServerAcceptConnexion()
        {
            this.Frame.Navigate(typeof(RoomScreen));
        }

        public void PlayerWin()
        {
            this.Frame.Navigate(typeof(WinPage));
        }

        public void PlayerLose()
        {
            this.Frame.Navigate(typeof(LosePage));
        }
    }
}

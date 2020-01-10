using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Diiage2022.Factory_Games.Client.Services;
using System.Diagnostics;
using System.Threading;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diiage2022.Factory_Games.Client.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class RoomScreen : Page, RoomScreenInterface
    {


        public RoomScreen()
        {
            this.InitializeComponent();
            Debug.WriteLine(((App)App.Current).servicesClass.Tcpclient.Available);
            SynchronizationContext context = SynchronizationContext.Current;
            ((App)App.Current).servicesClass.SetRoomScreenInterface(this, context);
        }

        public void StartGame()
        {
            this.Frame.Navigate(typeof(GameScreen));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           // ((App)App.Current).servicesClass = e.Parameter as ServicesClass;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e) //////////////// REPLACE SERVER ORDER TO START GAME
        {
            //((App)App.Current).servicesClass.SendTestMessage("test");
            //this.Frame.Navigate(typeof(PageTestCommunication));

        }
    }
}

using Diiage2022.Factory_Games.Client.Entities;
using Diiage2022.Factory_Games.Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Diiage2022.Factory_Games.Client.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class GameScreen : Page, GameScreenInterface
    {
        Game game;
        public GameScreen()
        {
            this.InitializeComponent();
            game = ((App)App.Current).servicesClass.game;
            SynchronizationContext originalContext = SynchronizationContext.Current;
            ((App)App.Current).servicesClass.SetGameScreenInterface(this, originalContext);

            DisplayCompanies();
            RefreshDisplay(game.LocalPlayer.CompanyId);
            maxRound.Text = game.MaxRound.ToString();
        }

        public void DisplayDeveloppers()
        {
            availableDevs.ItemsSource = null;
            availableDevs.ItemsSource = game.Developers.Where(d=>!d.Hired).ToList(); // devs not hired
        }

        public void DisplayProjects()
        {
            availableProjects.ItemsSource = null;
            availableProjects.ItemsSource = game.Projects.Where(p=> !game.Companies.SelectMany(c=>c.Projects).Select(cp=>cp.ProjectID).Contains(p.ProjectID)).ToList();
        }

        public void DisplayTrainningSession()
        {
            availableSchools.ItemsSource = null;
            availableSchools.ItemsSource = game.Schools;
        }

        public void RefreshDisplay(int playerId)
        {
            DisplayDeveloppers();
            DisplayProjects();
            DisplayTrainningSession();
            FocusOnPlayer(playerId);
        }

        public void ShowPlayerDetails(object sender, RoutedEventArgs e)
        {
            Button btnPlayer = (Button)sender;
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == int.Parse(btnPlayer.Name));
            FocusOnPlayer(player.CompanyId);
        }

        public void FocusOnPlayer(int companyId)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == companyId);
            if (player != null)
            {
                playerName.Text = player.Username;
                playerMoney.Text = "Money : " + player.Funds + "€";
                myDevs.ItemsSource = null;
                myDevs.ItemsSource = player.Developers;
                myProjects.ItemsSource = null;
                myProjects.ItemsSource = player.Projects;
            }
        }

        public void DisplayCompanies()
        {
            List<Company> lstCompanies = game.Companies;
            Grid playersGrid = new Grid(); // grid who contains players button for see details
            playersGrid.Background = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
            TextBlock infos = new TextBlock(); // text infos
            infos.Text = "Players :";
            infos.FontSize = 30;
            playersGrid.Children.Add(infos);
            for (int index=0; index< lstCompanies.Count;index++)
            {
                ColumnDefinition col1 = new ColumnDefinition(); // new col per player
                playersGrid.ColumnDefinitions.Add(col1); // add col to grid
                Button btnPlayer = new Button(); // create player button
                btnPlayer.Name = lstCompanies[index].CompanyId.ToString(); // set button name to id of company (+cast)
                btnPlayer.Content = lstCompanies[index].Username; // set button text
                btnPlayer.Click += ShowPlayerDetails; // set event on clic (to display player details)
                btnPlayer.HorizontalAlignment = HorizontalAlignment.Center;
                btnPlayer.VerticalAlignment = VerticalAlignment.Center;
                playersGrid.Children.Add(btnPlayer); // add player button to grid
                Grid.SetColumn(btnPlayer, index); // set col of button
            }
            board.Children.Add(playersGrid); // add grid to main grid
            Grid.SetRow(playersGrid, 2); // and set position
            Grid.SetColumnSpan(playersGrid, 2);
        }

        public void HireDevelopper(object sender, DoubleTappedRoutedEventArgs e)
        {
            //test local
            if (!game.IsInAction && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                game.IsInAction = true;
                Developer devClicked = (Developer)availableDevs.SelectedItem; //(Developper)((ListView)sender).SelectedItem;
                if(devClicked is Developer)
                {
                    ((App)App.Current).servicesClass.SelectDeveloper(devClicked.DeveloperId);
                }
            }
            game.IsInAction = false;
        }

        public void FireDevelopper(object sender, DoubleTappedRoutedEventArgs e)
        {
            //test local
            if (!game.IsInAction && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                game.IsInAction = true;
                Developer devClicked = (Developer)myDevs.SelectedItem; //(Developper)((ListView)sender).SelectedItem;
                if (devClicked is Developer && game.LocalPlayer.Developers.Contains(devClicked))
                {
                    ((App)App.Current).servicesClass.FireDeveloper(devClicked.DeveloperId);
                }
            }
            game.IsInAction = false;
        }

        public async void SelectProject(object sender, DoubleTappedRoutedEventArgs e)
        {
            //test local
            if (!game.IsInAction && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                game.IsInAction = true;
                Project projectClicked = (Project)availableProjects.SelectedItem; //(Project)((ListView)sender).SelectedItem;
                if (projectClicked is Project)
                {
                    ((App)App.Current).servicesClass.SelectProjet(projectClicked.ProjectID);
                }
            }
            game.IsInAction = false;
        }

        public void ClickEndTurn(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).servicesClass.FinishTurn();
        }

        private void ClickTrainingSession(object sender, DoubleTappedRoutedEventArgs e)
        {
            TrainingSession trainingSession = (TrainingSession)((ListView)sender).SelectedItem;
            if (!trainingSessionPopup.IsOpen && trainingSession is TrainingSession) {
                game.SelectedTrainingSession = trainingSession;
                developpersInThisTrainingSession.ItemsSource = null;
                developpersInThisTrainingSession.ItemsSource = trainingSession.TrainingSessionDevelopers;
                developpersAvailableForSchool.ItemsSource = null;
                developpersAvailableForSchool.ItemsSource = developpersAvailableForMyProject.ItemsSource = game.LocalPlayer.Developers.Where(d => !game.Projects.SelectMany(p => p.Developers).Select(p => p.DeveloperId).Contains(d.DeveloperId) && !game.Schools.SelectMany(s => s.SchoolTrainingSessions).SelectMany(ts => ts.TrainingSessionDevelopers).Select(tsd => tsd.DeveloperId).Contains(d.DeveloperId));
                trainingSessionPopup.IsOpen = true;
            }
        }

        private void ClickSendDevelopperInTrainingSession(object sender, RoutedEventArgs e)
        {
            // multiple selection
            List<Developer> lstDevs = (List<Developer>)developpersAvailableForSchool.SelectedItems.ToList().Select(d=>(Developer)d).ToList();
            if(lstDevs.Count > 0 && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                int schoolId = game.Schools.FirstOrDefault(s => s.SchoolTrainingSessions.Contains(game.SelectedTrainingSession)).SchoolId;
                foreach (var dev in lstDevs)
                {
                    
                    ((App)App.Current).servicesClass.AddDeveloperToSchool(schoolId, dev.DeveloperId, game.SelectedTrainingSession.TrainingSessionId);
                }
            }

            // selection 1 per 1
            //Developper developper = (Developper)((ListView)developpersAvailableForSchool).SelectedItem;
            //if(developper is Developper)
            //{
            //    developper.TrainingSession = game.SelectedTrainingSession;
            //    game.SelectedTrainingSession.TrainingSessionDeveloppers.Add(developper); // attente du srv
            //}
            if (trainingSessionPopup.IsOpen) { trainingSessionPopup.IsOpen = false; } // close popup
        }

        private void ClickRemoveDevelopperFromTrainingSession(object sender, RoutedEventArgs e)
        {
            // multiple selection
            List<Developer> lstDevs = (List<Developer>)developpersInThisTrainingSession.SelectedItems.ToList().Select(d => (Developer)d).ToList();
            if (lstDevs.Count > 0 && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                foreach (var dev in lstDevs)
                {
                    // no method for that
                }
            }
            if (trainingSessionPopup.IsOpen) { trainingSessionPopup.IsOpen = false; } // close popup
        }

        private void ClickCloseTrainingSessionPopup(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it 
            if (trainingSessionPopup.IsOpen) { trainingSessionPopup.IsOpen = false; }
        }

        private void ClickSendDevelopperInMyProject(object sender, RoutedEventArgs e)
        {
            // multiple selection
            List<Developer> lstDevs = (List<Developer>)developpersAvailableForMyProject.SelectedItems.ToList().Select(d => (Developer)d).ToList();
            if (lstDevs.Count > 0 && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                foreach (var dev in lstDevs)
                {
                    ((App)App.Current).servicesClass.AddDeveloperToProject(game.SelectedMyProject.ProjectID, dev.DeveloperId);
                }
            }
            if (myProjectPopup.IsOpen) { myProjectPopup.IsOpen = false; } // close popup
        }

        private void ClickRemoveDevelopperFromMyProject(object sender, RoutedEventArgs e)
        {
            // multiple selection
            List<Developer> lstDevs = (List<Developer>)developpersInThisProject.SelectedItems.ToList().Select(d => (Developer)d).ToList();
            if (lstDevs.Count > 0 && game.PlayerTurn == game.LocalPlayer.CompanyId)
            {
                foreach (var dev in lstDevs)
                {
                    // no method for that
                }
            }
            if (myProjectPopup.IsOpen) { myProjectPopup.IsOpen = false; } // close popup
        }

        private void ClickCloseMyProjectPopup(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it 
            if (myProjectPopup.IsOpen) { myProjectPopup.IsOpen = false; }
        }

        private void DbClickMyProjects(object sender, DoubleTappedRoutedEventArgs e)
        {
            Project project = (Project)((ListView)sender).SelectedItem;
            if (!myProjectPopup.IsOpen && project is Project && game.LocalPlayer.Projects.Contains(project))
            {
                game.SelectedMyProject = project;
                developpersAvailableForMyProject.ItemsSource = null;
                developpersAvailableForMyProject.ItemsSource = game.LocalPlayer.Developers.Where(d => !game.Projects.SelectMany(p => p.Developers).Select(p => p.DeveloperId).Contains(d.DeveloperId) && !game.Schools.SelectMany(s => s.SchoolTrainingSessions).SelectMany(ts => ts.TrainingSessionDevelopers).Select(tsd=>tsd.DeveloperId).Contains(d.DeveloperId));
                developpersInThisProject.ItemsSource = null;
                developpersInThisProject.ItemsSource = project.Developers;
                myProjectPopup.IsOpen = true;
            }
        }

        public void DisplayPlayerHireDeveloper(Developer developer, Company company)
        {
            //add action in chatbox
            string text = company.Username + " get the developper " + developer.DeveloperName;
            DisplayPlayerActionInChatBox(text);

            //update lists
            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerActionInChatBox(string text)
        {
            TextBlock playerAction = new TextBlock();
            playerAction.Text = text;
            playersActionsBox.Children.Add(playerAction);
        }

        public void DisplayPlayerFireDeveloper(Developer developer, Company company)
        {
            string text = company.Username + " fire the developper " + developer.DeveloperName;
            DisplayPlayerActionInChatBox(text);

            //update lists
            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerChooseProject(Project project, Company company)
        {
            string text = company.Username + " select project " + project.ProjectName;
            DisplayPlayerActionInChatBox(text);

            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerAssignDeveloperToProject(Project project, Developer developer, Company company)
        {
            string text = company.Username + " set developer " + developer.DeveloperName + " in project " + project.ProjectName;
            DisplayPlayerActionInChatBox(text);

            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerRemoveDeveloperFromProject(Project project, Developer developer, Company company)
        {
            string text = company.Username + " remove developer " + developer.DeveloperName + " from project " + project.ProjectName;
            DisplayPlayerActionInChatBox(text);

            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerAssignDeveloperToTrainingSession(TrainingSession trainingSession, Developer developer, Company company)
        {
            string text = company.Username + " set developer " + developer.DeveloperName + " in training session " + trainingSession.Name;
            DisplayPlayerActionInChatBox(text);

            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerRemoveDeveloperFromTrainingSession(TrainingSession trainingSession, Developer developer, Company company)
        {
            string text = company.Username + " remove developer " + developer.DeveloperName + " from training session " + trainingSession.Name;
            DisplayPlayerActionInChatBox(text);

            RefreshDisplay(company.CompanyId);
        }

        public void DisplayPlayerTurn(Company company)
        {
            if (game.LocalPlayer.CompanyId == game.PlayerTurn && !myTurnPopup.IsOpen) { myTurnPopup.IsOpen = true; }
            string text = "Turn of player " + company.Username;
            actualRound.Text = game.Round.ToString();
            RefreshDisplay(company.CompanyId);
            DisplayPlayerActionInChatBox(text);

        }

        private void ClickClosePopupMyTurn(object sender, RoutedEventArgs e)
        {
            if (myTurnPopup.IsOpen) { myTurnPopup.IsOpen = false; } // close popup
        }
    }
}

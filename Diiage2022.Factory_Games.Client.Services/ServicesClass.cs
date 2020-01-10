using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Diiage2022.Factory_Games.Client.Entities;

namespace Diiage2022.Factory_Games.Client.Services
{
    public class ServicesClass
    {
        public TcpClient Tcpclient { get; set; }//Define a TCPClient object that I could initialize or retrieve.
        public GameScreenInterface GSI { get; set; }
        public MainPageInterface MPI { get; set; }
        public RoomScreenInterface RSI { get; set; }
        public SynchronizationContext Context { get; set; }

        public Game game;        
        Thread t;


        public ServicesClass()
        {
            Tcpclient = new TcpClient();//Instantiate my clientTcp.
            game = new Game();

        }

        public void SetGameScreenInterface(GameScreenInterface gsi, SynchronizationContext context)
        {
            if(!(GSI is GameScreenInterface))
            {
                GSI = gsi;
                Context = context;
            }
        }

        public void SetMainPageInterface(MainPageInterface mpi, SynchronizationContext context)
        {
            if (!(MPI is MainPageInterface))
            {
                MPI = mpi;
                Context = context;
            }
        }

        public void SetRoomScreenInterface(RoomScreenInterface rsi, SynchronizationContext context)
        {
            if (!(RSI is RoomScreenInterface))
            {
                RSI = rsi;
                Context = context;
            }
        }

        public void SendData(Communication com)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                com.PlayerId = game.LocalPlayerId;
                string msg = JsonConvert.SerializeObject(com);//Create a string message and serialise com to json 
                byte[] buffer = new byte[1000];
                buffer = encoding.GetBytes(msg);// I transform my stfing into byte to send it in TCP.
                Tcpclient.GetStream().Write(buffer, 0, buffer.Length);// I get the connection flow between the client and the server
            }
            catch
            {

            }

        }
        public void SelectDeveloper(int IdDevelopper)
        {
            try
            {
                Communication com = new Communication();
                com.DeveloperId = IdDevelopper;
                com.RequestType = RequestType.CHOOSE_DEVELOPER;
                SendData(com);
            }
            catch
            {

            }
        }

        public void SelectProjet(int IdProject)
        {
            try
            {
                Communication com = new Communication();
                com.ProjectId = IdProject;
                com.RequestType = RequestType.CHOOSE_PROJECT;
                SendData(com);
            }
            catch
            {

            }
        }

        public void AddDeveloperToProject(int projectId, int IdDevelopper)
        {
            try
            {
                Communication com = new Communication();
                com.ProjectId = projectId;
                com.DeveloperId = IdDevelopper;
                com.RequestType = RequestType.ASSOCIATE_DEV_PROJECT;
                SendData(com);
            }
            catch
            {

            }
        }

        public void SendTestMessage(string msg)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Communication com = new Communication();//intantiate the objet communication.      
                string message = JsonConvert.SerializeObject(msg);//Create a string message and serialise com to json 
                byte[] buffer = new byte[1000]; //Create a table where we store what we send.
                buffer = encoding.GetBytes(message);// I transform my stfing into byte to send it in TCP.
                Tcpclient.GetStream().Write(buffer, 0, buffer.Length);//I get the connection flow between the client and the server
            }
            catch
            {

            }
        }

        public void FireDeveloper(int IdDevelopper)
        {
            try
            {
                Communication com = new Communication();
                com.DeveloperId = IdDevelopper;
                com.RequestType = RequestType.FIRE_DEVELOPER;
                SendData(com);
            }
            catch
            {

            }
        }

        public void AddDeveloperToSchool(int IdSchool, int IdDevelopper, int trainingSessionId)
        {
            try
            {
                School school = new School();
                Communication com = new Communication();
                com.DeveloperId = IdDevelopper;
                com.SchoolID = IdSchool;
                com.TrainingSessionId = trainingSessionId;
                com.RequestType = RequestType.CHOOSE_TRAINING_SESSION;
                SendData(com);
            }
            catch
            {

            }
        }

        public void FinishTurn()
        {
            try
            {
                Communication com = new Communication();
                com.RequestType = RequestType.FINISH_TURN;
                SendData(com);
            }
            catch
            {

            }
        }

        public bool Connection(string username, string addressIP)
        {
            bool test = false;//Variable for test the connection
            try
            {
                Communication com = new Communication();//intantiate the objet communication.      
                Tcpclient.Connect(IPAddress.Parse(addressIP), 7777);
                //Connect to the ip address give with the port 7777.
                byte[] buffer = new byte[1000]; //Create a table where we store what we send.
                ASCIIEncoding encoding = new ASCIIEncoding();//Instantiates the object that allows to encode my data in byte.

                com.RequestType = RequestType.CONNEXION_RESPONSE;
                com.NamePlayer = username;

                string msg = JsonConvert.SerializeObject(com);//Create a string message and serialise com to json 
                buffer = encoding.GetBytes(msg);// I transform my stfing into byte to send it in TCP.
                Tcpclient.GetStream().Write(buffer, 0, buffer.Length);//I get the connection flow between the client and the server
                                                                      // .whrite allows to write on the flow of data my byte tables.

                t = new Thread(Read);//Instantiate a Thread to keep the connection(function READ).
                t.Start();//Start the Thread.
                test = true;//The connection is established.
            }
            catch(Exception e)
            {
                //exception
            }
            return test;//Send the variable for the test of connection.
        }
        public void Read()
        {
            try
            {
                while (true)//Create an infinite loop to keep the connection.
                {
                    byte[] bytes = new byte[10000];//Create a table where we store what we receve.
                    StringBuilder MyCompleteMessage = new StringBuilder();
                    do
                    {
                        int NbrOfBytesRead = Tcpclient.GetStream().Read(bytes, 0, bytes.Length);
                        MyCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(bytes, 0, NbrOfBytesRead));
                    }
                    while (Tcpclient.GetStream().DataAvailable);
                    Communication com = JsonConvert.DeserializeObject<Communication>(MyCompleteMessage.ToString());
                    RedirectReceiveData(com);
                }
            }
            catch
            {

            }

        }

        public void RedirectReceiveData(Communication com)
        {
            switch (com.RequestType)
            {
                case RequestType.CONNEXION_RESPONSE:
                    ConnexionResponse(com);
                    return;
                case RequestType.INITIALIZE_GAME:
                    GameInitialization(com);
                    return;
                case RequestType.CHOOSE_DEVELOPER:// other player Choose a dev
                    PlayerChooseDeveloper(com);
                    return;
                case RequestType.CHOOSE_TRAINING_SESSION:// Choose a training Session
                    PlayerAssignDeveloperToTrainingSession(com);
                    return;
                case RequestType.CHOOSE_PROJECT:// Choose a project
                    PlayerChooseProject(com);
                    return;
                case RequestType.ASSOCIATE_DEV_PROJECT:// Associate dev and project
                    PlayerAssignDeveloperToProject(com);
                    return;
                case RequestType.ANALYSE_MARKET:// Analyse market
                    return;
                case RequestType.FINISH_TURN:// Finish Turn
                    return;
                case RequestType.FIRE_DEVELOPER:
                    PlayerFireDeveloper(com);
                    return;
                case RequestType.PLAYER_TURN:
                    PlayerTurn(com);
                    return;
                case RequestType.END_GAME:
                    EndGameResponse(com);
                    return;
                default:// Something go wrong
                    return;
            }
        }

        public void ConnexionResponse(Communication com)
        {
            if(com.PlayerId != 0)
            {
                game.LocalPlayerId = com.PlayerId;
                ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { MPI.ServerAcceptConnexion(); }, null); });
            }
        }

        public void EndGameResponse(Communication com)
        {
            if (com.PlayerId == game.LocalPlayerId)
            {
                ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { MPI.PlayerWin(); }, null); });
            }
            else
            {
                ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { MPI.PlayerLose(); }, null); });
            }
        }

        public void GameInitialization(Communication com)
        {
            //set players
            game.Companies = com.Companies;
            // set local player
            game.LocalPlayer = com.Companies.FirstOrDefault(p => p.CompanyId == game.LocalPlayerId);
            game.PlayerTurn = game.LocalPlayerId;
            //set developers
            game.Developers = com.Developers;
            game.Round = 0;
            game.MaxRound = com.RoundMax;

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { RSI.StartGame(); }, null); });
            
        }

        public void PlayerChooseDeveloper(Communication com)
        {
            // set changes
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);

            // call ui
            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerHireDeveloper(developer, player); }, null); });
            
        }

        void PlayerFireDeveloper(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);
            // call ui
            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerFireDeveloper(developer, player); }, null); });
        }

        public void PlayerChooseProject(Communication com)
        {
            //set changes
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            Project project = game.Projects.FirstOrDefault(p => p.ProjectID == com.ProjectId);
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerChooseProject(project, player); }, null); });
        }

        void PlayerAssignDeveloperToProject(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            Project project = game.Projects.FirstOrDefault(p => p.ProjectID == com.ProjectId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerAssignDeveloperToProject(project, developer, player); }, null); });
        }
        void PlayerRemoveDeveloperFromProject(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            Project project = game.Projects.FirstOrDefault(p => p.ProjectID == com.ProjectId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerRemoveDeveloperFromProject(project, developer, player); }, null); });
        }
        void PlayerAssignDeveloperToTrainingSession(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            TrainingSession trainingSession = game.Schools.SelectMany(s=>s.SchoolTrainingSessions).FirstOrDefault(ts => ts.TrainingSessionId == com.TrainingSessionId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerAssignDeveloperToTrainingSession(trainingSession, developer, player); }, null); });
        }
        void PlayerRemoveDeveloperFromTrainingSession(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            TrainingSession trainingSession = game.Schools.SelectMany(s => s.SchoolTrainingSessions).FirstOrDefault(ts => ts.TrainingSessionId == com.TrainingSessionId);
            Developer developer = game.Developers.FirstOrDefault(d => d.DeveloperId == com.DeveloperId);
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerRemoveDeveloperFromTrainingSession(trainingSession, developer, player); }, null); });
        }
        void PlayerTurn(Communication com)
        {
            Company player = game.Companies.FirstOrDefault(c => c.CompanyId == com.PlayerId);
            game.PlayerTurn = player.CompanyId;
            game.Round = com.ActualRound;
            UpdateGameDatas(com);

            ThreadPool.QueueUserWorkItem(delegate { Context.Post(delegate { GSI.DisplayPlayerTurn(player); }, null); });
        }

        void UpdateGameDatas(Communication com)
        {
            game.Projects = com.Projects;
            game.Companies = com.Companies;
            game.Developers = com.Developers;
            game.Schools = com.Schools;

            game.LocalPlayer = com.Companies.FirstOrDefault(c => c.CompanyId == game.LocalPlayerId);
        }
    }
}

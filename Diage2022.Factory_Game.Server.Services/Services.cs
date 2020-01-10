using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Diiage2022.Factory_Games.Entities;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Diage2022.Factory_Game.Server.Services
{
    public partial class Services
    {
        public Game Game;
        public bool TurnFinished { set; get; }
        public List<TcpClient> Clients { set; get; }

        public List<string> developersNames;
        public List<string> developersImages;
        public List<string> projectsNames;
        public List<string> projectsImages;
        public List<string> schoolsNames;
        public List<string> schoolsImages;
        public List<string> skillsNames;
        public List<string> skillsImages;
        public bool adminStartGame = false;

        public Services()
        {
            Clients = new List<TcpClient>();
            LoadData();
        }
        /// <summary>
        /// Load the data contained in the json files to put them in lists
        /// </summary>
        public void LoadData()
        {
            var pathDevelopersNames = Path.Combine(Directory.GetCurrentDirectory(), "Data/developersNames.json");
            FillList(out developersNames, pathDevelopersNames);
            var pathDevelopersImages = Path.Combine(Directory.GetCurrentDirectory(), "Data/developersImages.json");
            FillList(out developersImages, pathDevelopersImages);


            var pathProjectsNames = Path.Combine(Directory.GetCurrentDirectory(), "Data/projectsNames.json");
            FillList(out projectsNames, pathProjectsNames);
            var pathProjectsImages = Path.Combine(Directory.GetCurrentDirectory(), "Data/projectsImages.json");
            FillList(out projectsImages, pathProjectsImages);

            var pathSchoolsNames = Path.Combine(Directory.GetCurrentDirectory(), "Data/schoolsNames.json");
            FillList(out schoolsNames, pathSchoolsNames);
            var pathSchoolsImages = Path.Combine(Directory.GetCurrentDirectory(), "Data/schoolsImages.json");
            FillList(out schoolsImages, pathSchoolsImages);

            var pathSkillsNames = Path.Combine(Directory.GetCurrentDirectory(), "Data/skillsNames.json");
            FillList(out skillsNames, pathSkillsNames);
            var pathSkillsImages = Path.Combine(Directory.GetCurrentDirectory(), "Data/skillsImages.json");
            FillList(out skillsImages, pathSkillsImages);

        }
        /// <summary>
        /// Fill the list with data contained in a file
        /// </summary>
        /// <param name="list"></param>
        /// <param name="path"></param>
        public void FillList(out List<string> list, string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                list = JsonConvert.DeserializeObject<List<string>>(json);
            }
        }


        /// <summary>
        /// Initialize a Game Object which contains all the informations for the game
        /// </summary>
        /// <param name="gamedifficulty"></param>
        /// <param name="maxround"></param>
        public int CreateGame(int gamedifficulty, int maxround, double treasuryStart)
        {
            try
            {
                Game = new Game(gamedifficulty, maxround, treasuryStart);//Initialize the game settings
                GenerateSkills();//Generate Skills
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
        /// <summary>
        /// Send the Data of the game to all the players
        /// </summary>
        /// <param name="tcpClients"></param>
        /// <returns></returns>
        public int InitializeGame(List<TcpClient> tcpClients)
        {
            try
            {
                Clients = tcpClients;//Set the list of TcpClient
                Game.Round = 0;//Set the Round to 0
                Game.Companies.ForEach(c => c.Funds = Game.GameTreasuryStart);// Set the treasury of all players
                Communication communication = new Communication();//Create a communication object
                communication.Developers = Game.Developers;//Set the Developers
                communication.Companies = Game.Companies;//Set the Companies
                communication.RequestType = RequestType.INITIALIZE_GAME;//Set the Request Type
                communication.RoundMax = Game.RoundMax;//Set the Max Round
                SendToAll(communication);//Send the data to all players
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }

        /// <summary>
        /// Create an object Company which correspond to a player
        /// </summary>
        /// <param name="username"></param>
        /// <param name="idcompany"></param>
        /// <returns>Return the number of companies</returns>
        public int AddCompany(string username, int idcompany)
        {
            try
            {
                Company company = new Company();//Create the company object
                company.Username = username;//Set the username
                company.CompanyId = idcompany;//Set the company id
                company.Funds = Game.GameTreasuryStart;//Set the Treasury Start
                Game.Companies.Add(company);// Add the company Object to the Game object
                return Game.Companies.Count;//return the number of companies
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
        /// <summary>
        /// This method send the object communication to all the clients connected
        /// </summary>
        /// <param name="communication"></param>
        public int SendToAll(Communication communication)
        {
            try
            {
                communication.Developers = Game.Developers;//Set the Developers by the Game data
                communication.Projects = Game.Projects;//Set the Projects by the Game data
                communication.Companies = Game.Companies;//Set the Companies by the Game data
                communication.Schools = Game.Schools;//Set the Schools by Game Data
                string query = JsonConvert.SerializeObject(communication, new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore });//Serialize the data
                byte[] response = Encoding.UTF8.GetBytes(query);//Get the bytes of the string

                Clients.ForEach(c => c.GetStream().Write(response, 0, response.Length));// Send the bytes to all clients
                Console.WriteLine(communication.PlayerId + " " + communication.NamePlayer + " " + communication.RequestType);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
        /// <summary>
        /// Send Data to one client
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <param name="communication"></param>
        /// <returns></returns>
        public int SendToOneClient(TcpClient tcpClient, Communication communication)
        {
            try
            {
                communication.PlayerId = Thread.CurrentThread.ManagedThreadId;//Set the player id by the id of the thread
                string query = JsonConvert.SerializeObject(communication, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });//Serialize data
                byte[] response = Encoding.UTF8.GetBytes(query);//Get bytes of the string
                tcpClient.GetStream().Write(response, 0, response.Length);// Send the bytes to the string
                Console.WriteLine(communication.PlayerId + " " + communication.NamePlayer);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }

        /// <summary>
        /// Analyse the data sent by the clients and call the appropriate method
        /// </summary>
        /// <param name="communication"></param>
        /// <returns></returns>
        public int QueryTreatment(Communication communication)
        {
            try
            {
                switch (communication.RequestType)//Test the Request Type
                {
                    case RequestType.CREATE_PLAYER:// Create the player
                        int result = AddCompany(communication.NamePlayer, Thread.CurrentThread.ManagedThreadId);
                        return 1;
                    case RequestType.CHOOSE_DEVELOPER:// Choose a dev
                        AddDeveloperToCompany(communication.DeveloperId, communication.PlayerId);
                        SendToAll(communication);
                        return 0;
                        
                    case RequestType.CHOOSE_TRAINING_SESSION:// Choose a training Session
                        AddDeveloperToTrainingSession(communication.PlayerId, communication.DeveloperId, communication.SchoolID, communication.TrainingSessionId);
                        SendToAll(communication);
                        return 0;

                    case RequestType.CHOOSE_PROJECT:// Choose a project
                        AddProjectToCompany(communication.ProjectId, communication.PlayerId);
                        SendToAll(communication);
                        return 0;

                    case RequestType.ASSOCIATE_DEV_PROJECT:// Associate dev and project
                        AddDeveloperToProject(communication.DeveloperId, communication.ProjectId, communication.PlayerId);
                        SendToAll(communication);
                        return 0;

                    case RequestType.ANALYSE_MARKET:// Analyse market
                        return 0;

                    case RequestType.FINISH_TURN:// Finish Turn
                        FinishTurn();
                        return 0;

                    case RequestType.FIRE_DEVELOPER://Fire a developer
                        FireDeveloper(communication.DeveloperId, communication.PlayerId);
                        SendToAll(communication);
                        return 0;

                    case RequestType.ADMIN_START_GAME:
                        AdminStartGame(communication);
                        return 0;
                    default:// Something go wrong
                        return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
    }
}

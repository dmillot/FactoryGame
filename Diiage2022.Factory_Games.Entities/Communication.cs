using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Entities
{
    public enum RequestType { CREATE_PLAYER, CHOOSE_DEVELOPER, CHOOSE_TRAINING_SESSION, CHOOSE_PROJECT, ASSOCIATE_DEV_PROJECT, ANALYSE_MARKET, FINISH_TURN, FIRE_DEVELOPER, GAME_START, PLAYER_TURN, ADMIN_START_GAME, INITIALIZE_GAME, END_GAME };

    public class Communication
    {
        public int CommunicationId { set; get; }
        public int PlayerId { set; get; }
        public RequestType RequestType { set; get; }
        public int DeveloperId { set; get; }
        public int ProjectId { set; get; }
        public string NamePlayer { set; get; }
        public int TrainingSessionId { set; get; }
        public int SchoolID { set; get; }
        public List<School> Schools { set; get; }
        public List<Developer> Developers { set; get; }
        public List<Project> Projects { set; get; }
        public List<Company> Companies { set; get; }
        public int ErrorChoice { set; get; }
        public string ErrorMessage { set; get; }
        public Configuration Configuration { get; set; }
        public int ActualRound { get; set; }
        public int RoundMax { get; set; }

        public Communication()
        {
            Schools = new List<School>();
            Developers = new List<Developer>();
            Projects = new List<Project>();
        }
    }
}

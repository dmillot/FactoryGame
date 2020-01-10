using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Client.Entities
{
    public class Game
    {
        public int Round { set; get; }
        public int MaxRound { set; get; }
        public List<Company> Companies { set; get; }
        public List<Project> Projects { set; get; }
        public List<School> Schools { set; get; }
        public List<Developer> Developers { set; get; }
        public List<Skill> Skills { set; get; }
        public bool IsInAction { get; set; }
        public Company LocalPlayer { get; set; }
        public int LocalPlayerId { get; set; }
        public TrainingSession SelectedTrainingSession { get; set; }
        public Project SelectedMyProject { get; set; }
        public List<string> PlayersActions { get; set; }

        public int PlayerTurn { get; set; }

        public Game()
        {
            Projects = new List<Project>();
            Companies = new List<Company>();
            Schools = new List<School>();
            Developers = new List<Developer>();
            Skills = new List<Skill>();
            PlayersActions = new List<string>();
            IsInAction = false;
            PlayerTurn = 0; // set to 0 for nothing
        }

    }
}

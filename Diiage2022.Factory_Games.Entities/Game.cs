using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Entities
{
    public class Game
    {
        public int GameDifficulty { set; get; }
        public int GamePlayerNumber { set; get; }
        public double GameTreasuryStart { set; get; }
        public int Round { set; get; }
        public int RoundMax { set; get; }
        public int PlayerHaveToPlay { set; get; }
        public int CurrentPlayer { set; get; }
        public List<Company> Companies { set; get; }
        public List<Project> Projects { set; get; }
        public List<School> Schools { set; get; }
        public List<Developer> Developers { set; get; }
        public List<Skill> Skills { set; get; }

        public Game(int gamedifficulty, int roundmax, double treasuryStart)
        {
            Projects = new List<Project>();
            Companies = new List<Company>();
            Schools = new List<School>();
            Developers = new List<Developer>();
            Skills = new List<Skill>();
            GameDifficulty = gamedifficulty;
            RoundMax = roundmax;
            GameTreasuryStart = treasuryStart;
        }
    }
}

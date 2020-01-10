using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Client.Entities
{
    public class TrainingSession
    {
        public int TrainingSessionId { set; get; }
        public Skill TrainingSessionSkill { set; get; }
        public int TrainingSessionDuration { set; get; }
        public bool Accessibility { set; get; }
        public List<Developer> TrainingSessionDevelopers { set; get; }
        public string Name { get; set; }
        public School School { get; set; }

        public TrainingSession()
        {
            TrainingSessionDevelopers = new List<Developer>();
        }
    }
}

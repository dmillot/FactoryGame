using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Client.Entities
{
    public class Developer
    {
        public int DeveloperId { set; get; }
        public bool Hired { set; get; }
        public string DeveloperName { set; get; }
        public List<Skill> DeveloperSkills { set; get; }
        public double DeveloperSalary { set; get; }
        public TrainingSession TrainingSession { get; set; }
        public bool InATrainingSession { get; set; }
        public bool OnAProject { get; set; }
        public Project Project { get; set; }
        public Company Company { get; set; }

        public string ImageUrl { get; set; }

        public Developer()
        {
            DeveloperSkills = new List<Skill>();
        }
    }
}

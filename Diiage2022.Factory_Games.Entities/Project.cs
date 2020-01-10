using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Entities
{
    public class Project
    {
        public int ProjectID { set; get; }
        public string ProjectName { set; get; }
        public List<Skill> Skills { set; get; }
        public List<Developer> Developers { set; get; }
        public int ProjectDuration { set; get; }
        public double ProjectPay { set; get; }
        public int ProjectPenality { set; get; }
        public Company Company { get; set; }
        public string ImageUrl { get; set; }
        public Project()
        {
            Skills = new List<Skill>();
            Developers = new List<Developer>();
        }
    }
}

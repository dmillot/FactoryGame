using System;
using System.Collections.Generic;

namespace Diiage2022.Factory_Games.Entities
{
    public class Company
    {
        public int CompanyId { set; get; }
        public string Username { set; get; }
        public List<Developer> Developers { set; get; }
        public List<Project> Projects { set; get; }
        public double Funds { set; get; }

        public Company()
        {
            Developers = new List<Developer>();
            Projects = new List<Project>();
        }
    }
}

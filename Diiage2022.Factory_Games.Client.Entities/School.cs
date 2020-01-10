using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Client.Entities
{
    public class School
    {
        public int SchoolId { set; get; }
        public string SchoolName { set; get; }
        public List<TrainingSession> SchoolTrainingSessions { set; get; }
        public string ImageUrl { get; set; }

        public School()
        {

        }
    }
}

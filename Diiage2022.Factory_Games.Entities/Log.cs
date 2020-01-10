using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Entities
{
    public class Log
    {
        public DateTime Date { get; set; }
        public Communication Communication { get; set; }
        public Log(DateTime date, Communication com)
        {
            Date = date;
            Communication = com;
        }
    }
}

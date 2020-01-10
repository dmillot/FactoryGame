using Diiage2022.Factory_Games.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Diage2022.Factory_Game.Server.Services
{
    partial class Services
    {
        public void AdminStartGame(Communication com)
        {
            Configuration config = com.Configuration;
            Game.GameTreasuryStart = config.PlayerMoney;
            Game.RoundMax = config.NumberOfTurns;
            adminStartGame = true;
        }
    }
}

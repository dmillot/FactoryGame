using Diiage2022.Factory_Games.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Diage2022.Factory_Game.Server.Services
{
    public partial class Services
    {
        /// <summary>
        /// Initialize A new turn:
        /// Shuffle the List of the Player
        /// Generate the developers, the projects, the training sessions
        /// </summary>
        /// <returns>
        /// Return the list of Id of the player shuffled 
        /// </returns>
        public List<int> StartTurn()
        {
            try
            {
                Game.Round += 1;//Increase the value of the round by 1
                Game.PlayerHaveToPlay = Game.Companies.Count;// Set the value of the number of player who have to play by the number of player in the object game
                List<Company> companiesToShuffle = Game.Companies.ToList();// Copy the list of the companies
                List<Company> ShuffledPlayerList = ShuffleList(companiesToShuffle);//Shuffle the list of player
                if(Game.Round>=2)
                {
                    GenerateDevelopers(3, 3000);
                    GenerateProjects(3);
                    GenerateSchoolTrainingSessions();
                }
                return ShuffledPlayerList.Select(p=> p.CompanyId).ToList();//return the list of the id of the companies shuffled
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                List<int> error = new List<int> { -1 };
                return error;
            }
        }
        /// <summary>
        /// Return a list which is the list gave shuffled 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="inputList"></param>
        /// <returns>List of Elements</returns>
        public List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
        /// <summary>
        /// Create a communication Object with the request type PLAYER_TURN
        /// And call the method SendToAll with this communication object as data
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public int SetPlayerTurn(int playerId)
        {
            Game.CurrentPlayer = playerId;//Set the value of the current player by the player id
            Communication communication = new Communication();//Create a communication object
            communication.RequestType = RequestType.PLAYER_TURN;//Set the request type of the communication
            communication.PlayerId = playerId;//Set the player id
            communication.ActualRound = Game.Round;//Set the Actual Round of the game
            SendToAll(communication);//Send the data to all the players
            return 1;
        }
        /// <summary>
        /// Update the Funds of all the company by subtracting all developers salary
        /// </summary>
        /// <returns></returns>
        public int UpdatingCompanies()
        {
            try
            {
                //Subtract the total salary of all developers of a company foreach companies
                Game.Companies.ForEach(c=>
                {
                    double salarySpending = c.Developers.Sum(d=> d.DeveloperSalary);
                    c.Funds -= salarySpending;
                }
                );
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
        /// <summary>
        /// Finish the turn of a player
        /// if all the player have played update the training sessions, projects, and the amount of money which the players have
        /// </summary>
        /// <returns></returns>
        public int FinishTurn()
        {
            try
            {
                if (Thread.CurrentThread.ManagedThreadId == Game.CurrentPlayer)
                {
                    Game.PlayerHaveToPlay -= 1;//Subtrack 1 to the number of player who have to play
                    
                    TurnFinished = true;
                    if (Game.PlayerHaveToPlay <= 0)//Test if all player have played
                    {
                        UpdateTrainingSessions();
                        UpdatingProject();
                        UpdatingCompanies();
                        if(Game.Round==Game.RoundMax)//Test if the current round is the last round
                        {
                            Communication communication = new Communication();//Create a communication object
                            communication.RequestType = RequestType.END_GAME;// Set the request type by END_GAME
                            double companyFunds = Game.Companies.Max(c => c.Funds);//Get the funds maximum of all the players
                            Company company = Game.Companies.FirstOrDefault(c => c.Funds == companyFunds);//get the company with this max amount
                            communication.PlayerId = company.CompanyId;// Set the player Id of the winner
                            SendToAll(communication);//Send the data to all players
                            return 0;
                        }
                        return 0;
                    }
                    else
                        return 1;                    
                }
                else
                    return -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }
    }
}
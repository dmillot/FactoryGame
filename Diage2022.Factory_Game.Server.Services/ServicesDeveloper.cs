using Diiage2022.Factory_Games.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diage2022.Factory_Game.Server.Services
{
    public partial class Services
    {
        /// <summary>
        /// Methods which Generate a number of developer
        /// </summary>
        /// <param name="numberDeveloper"></param>
        /// <param name="baseSalary"></param>
        public int GenerateDevelopers(int numberDeveloper, int baseSalary)
        {
            try
            {
                for (int i = 1; i <= numberDeveloper; i++)
                {
                    Random random = new Random();
                    Developer developer = new Developer
                    {
                        DeveloperId = Game.Developers.Count + 1,
                        DeveloperName = developersNames[random.Next(developersNames.Count)],
                        ImageUrl = developersImages[random.Next(developersImages.Count)]
                    };
                    for(int k= 0;k<2;k++)
                    {
                        int skillIndex = random.Next(Game.Skills.Count);
                        developer.DeveloperSkills.Add(Game.Skills[skillIndex]);
                    }
                    developer.DeveloperSalary = baseSalary + developer.DeveloperSkills.Sum(d => d.SkillLevel)*200;
                    developer.Hired = false;
                    Game.Developers.Add(developer);
                }
                return Game.Developers.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }
        }

        /// <summary>
        /// Add a developer to the player company
        /// </summary>
        /// <param name="developerId"></param>
        /// <param name="companyId"></param>
        public int AddDeveloperToCompany(int developerId, int companyId)
        {
            try
            {
                if (companyId == Game.CurrentPlayer)//Verify if the player who sent the request is the correct player
                {
                    Company currentCompany = Game.Companies.FirstOrDefault(c => c.CompanyId == companyId);//Select the right company
                    Developer developer = Game.Developers.FirstOrDefault(d => d.DeveloperId == developerId);//Select the right developer
                    if (developer.Hired == false)//Check if the developer isn't hired
                    {
                        developer.Hired = true;//Set the developer as hired
                        developer.Company = currentCompany;
                        currentCompany.Developers.Add(developer);//Add the developer to the company
                        return currentCompany.Developers.Count;//Return the number of developer
                    }
                    else
                        return -1;
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

        /// <summary>
        /// This Method delete a developer from the company list and send it back to the free developers list
        /// </summary>
        /// <param name="developerId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public int FireDeveloper(int developerId, int companyId)
        {
            if (companyId == Game.CurrentPlayer)
            {
                Company currentCompany = Game.Companies.FirstOrDefault(c => c.CompanyId == companyId);
                Developer developer = Game.Developers.FirstOrDefault(d => d.DeveloperId == developerId);
                if (developer.OnAProject == true && developer.InATrainingSession == true)
                {
                    developer.Hired = false;
                    developer.Company = null;
                    currentCompany.Developers.Remove(developer);
                }
                return currentCompany.Developers.Count;
            }
            else
                return -1;
        }
    }
}

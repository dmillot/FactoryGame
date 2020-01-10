using Diage2022.Factory_Game.Server.Services;
using Diiage2022.Factory_Games.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Diiage2022.Factory_Games.Server.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        Services services = new Services();
        [TestMethod]
        public void TestMethodGenerateSchools()
        {
            services.CreateGame(1, 2, 3);
            Assert.AreEqual(1, services.GenerateSchoolTrainingSessions());
            Assert.AreEqual(2, services.GenerateSchoolTrainingSessions());
            Assert.AreEqual(3, services.GenerateSchoolTrainingSessions());
        }

        [TestMethod]
        public void TestMethodGenerateDevelopers()
        {
            services.CreateGame(1, 2, 3);
            Assert.AreEqual(3, services.GenerateDevelopers(3, 10000));
            
        }
        [TestMethod]
        public void TestMethodStartTurn()
        {
            services.CreateGame(1, 2, 400);
            services.Game.Round = 1;
            Company company = new Company();
            company.CompanyId = 1;
            company.Funds = 1000;
            company.Username = "Je suis un test";
            services.Game.Companies.Add(company);
            services.StartTurn();
            Assert.AreEqual(3, services.Game.Developers.Count);
            Assert.AreEqual(3, services.Game.Projects.Count);
        }
        [TestMethod]
        public void TestMethodFireDeveloper()
        {
            services.CreateGame(1, 2, 3);
            services.GenerateDevelopers(3, 10000);
            services.AddCompany("toto", 1);
            services.AddDeveloperToCompany(1, 1);
            services.SetPlayerTurn(1);
            Assert.AreEqual(1, services.AddDeveloperToCompany(1, 1));
            Assert.AreEqual(0, services.FireDeveloper(1,1));
            
        }

        [TestMethod]
        public void TestMethodAddDeveloperToTrainingSession()
        {
            services.CreateGame(1, 2, 3);
            services.GenerateDevelopers(3, 10000);
            services.GenerateSchoolTrainingSessions();
            services.AddCompany("toto", 1);
            services.AddDeveloperToCompany(1, 1);
            services.SetPlayerTurn(1);
            Assert.AreEqual(1, services.AddDeveloperToTrainingSession(1, 1, 1, 1));

        }

        [TestMethod]
        public void TestGenerateProjects()
        {
            services.CreateGame(1, 2, 3);
            Assert.AreEqual(2, services.GenerateProjects(2));
        }

        [TestMethod]
        public void TestUpdateProjects()
        {
            services.CreateGame(1, 10, 100000);
            services.GenerateDevelopers(1000, 0);
            services.AddCompany("toto", 1);
            services.SetPlayerTurn(1);
            services.GenerateProjects(1);
            services.AddProjectToCompany(services.Game.Projects[0].ProjectID, 1);
            foreach(var developer in services.Game.Developers)
            {
                services.AddDeveloperToCompany(developer.DeveloperId, 1);
                services.AddDeveloperToProject(developer.DeveloperId, services.Game.Projects[0].ProjectID, 1);
            }
            int duration = services.Game.Projects[0].ProjectDuration;
            for (int i=0;i<= duration; i++)
            {
                services.UpdatingProject();
            }
            Assert.AreEqual(0, services.Game.Companies[0].Projects.Count);

        }


        [TestMethod]
        public void TestUpdateSchools()
        {
            services.CreateGame(1, 10, 100000);
            services.GenerateDevelopers(1, 0);
            int nbSkills = services.Game.Developers[0].DeveloperSkills.Count;
            services.GenerateSchoolTrainingSessions();
            services.AddCompany("toto", 1);
            services.SetPlayerTurn(1);
            services.AddDeveloperToCompany(1, 1);
            services.AddDeveloperToTrainingSession(1, 1, 1, 1);

            int duration = services.Game.Schools[0].SchoolTrainingSessions[0].TrainingSessionDuration;
            for (int i = 0; i <= duration; i++)
            {
                services.UpdateTrainingSessions();
            }
            int nbSkillsAfterTrainingSession = services.Game.Developers[0].DeveloperSkills.Count;

            Assert.AreEqual(nbSkills + 1, nbSkillsAfterTrainingSession);

        }
    }
}

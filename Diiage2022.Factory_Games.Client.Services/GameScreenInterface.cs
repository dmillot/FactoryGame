using Diiage2022.Factory_Games.Client.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diiage2022.Factory_Games.Client.Services
{
    public interface GameScreenInterface
    {
        
        void DisplayPlayerHireDeveloper(Developer developer, Company company);
        void DisplayPlayerFireDeveloper(Developer developer, Company company);
        void DisplayPlayerChooseProject(Project project, Company company);
        void DisplayPlayerAssignDeveloperToProject(Project project, Developer developer, Company company);
        void DisplayPlayerRemoveDeveloperFromProject(Project project, Developer developer, Company company);
        void DisplayPlayerAssignDeveloperToTrainingSession(TrainingSession trainingSession, Developer developer, Company company);
        void DisplayPlayerRemoveDeveloperFromTrainingSession(TrainingSession trainingSession, Developer developer, Company company);
        void DisplayPlayerTurn(Company company);
    }
}

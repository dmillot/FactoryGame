using Diiage2022.Factory_Games.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diage2022.Factory_Game.Server.Services
{
    public partial class Services
    {
        /// <summary>
        /// this method allow to Generate Projects
        /// </summary>
        /// <param name="numberProjects"></param>
        /// <returns></returns>
        public int GenerateProjects(int numberProjects)
        {
            try
            {
                for (int i = 1; i <= numberProjects; i++)
                {
                    Random random = new Random();
                    Project project = new Project()
                    {
                        ProjectID = Game.Projects.Count + 1,
                        ProjectName = projectsNames[random.Next(projectsNames.Count)],
                        ProjectPenality = 0,
                        ImageUrl = projectsImages[random.Next(projectsImages.Count)]
                    };

                    int skillsToGenerate = random.Next(1, 4);

                    for (int x = 1; x <= skillsToGenerate; x++)
                    {
                        List<Skill> currentSkills = Game.Skills.Intersect(project.Skills).ToList();
                        int skillIndex = random.Next(Game.Skills.Count);

                        while (currentSkills.Contains(Game.Skills[skillIndex]))
                        {
                            skillIndex = random.Next(Game.Skills.Count);
                        }

                        project.Skills.Add(Game.Skills[skillIndex]);
                    }

                    int variable = 0;
                    foreach (Skill oneSkill in project.Skills)
                    {
                        variable += (project.Skills.Count * oneSkill.SkillLevel);
                    }

                    int maxProjectDuration = 8;
                    if (Game.RoundMax < 8)
                        maxProjectDuration = Game.RoundMax;

                    double customDuration = Math.Floor(maxProjectDuration * ((double)variable / 27d));
                    if (Convert.ToInt16(customDuration) <= 0)
                        project.ProjectDuration = 1;
                    else
                        project.ProjectDuration = Convert.ToInt16(customDuration);

                    project.ProjectPay = 4000 * project.ProjectDuration;
                    Game.Projects.Add(project);
                }
                return Game.Projects.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return -1;
            }

        }

        /// <summary>
        /// Add a project to a company
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        public int AddProjectToCompany(int projectid, int companyid)
        {
            try
            {
                if (companyid == Game.CurrentPlayer)
                {
                    var companies = Game.Companies;
                    Company currentCompany = companies.FirstOrDefault(c => c.CompanyId == companyid);
                    var projects = Game.Projects;
                    Project project = projects.FirstOrDefault(p => p.ProjectID == projectid);
                    currentCompany.Projects.Add(project);
                    return currentCompany.Projects.Count;
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
        /// Assign a developer to a project
        /// </summary>
        /// <param name="developerid"></param>
        /// <param name="projectid"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        public int AddDeveloperToProject(int developerid, int projectid, int companyid)
        {
            try
            {
                if (companyid == Game.CurrentPlayer)
                {
                    var companies = Game.Companies;
                    Company currentCompany = companies.FirstOrDefault(c => c.CompanyId == companyid);
                    var projects = currentCompany.Projects;
                    Project project = projects.FirstOrDefault(p => p.ProjectID == projectid);
                    var developers = currentCompany.Developers;
                    Developer developer = developers.FirstOrDefault(d => d.DeveloperId == developerid);
                    if (developer.OnAProject == false && developer.InATrainingSession == false)
                    {
                        developer.OnAProject = true;
                        developer.Project = project;
                        project.Developers.Add(developer);
                    }
                    return project.Developers.Count;
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

        public int UpdatingProject()
        {
            List<Project> projects = Game.Projects.Where(p=>p.Developers.Count > 0).ToList();
            projects.ForEach(p =>
            {
                List<Skill> developerSkills = p.Developers.SelectMany(d => d.DeveloperSkills).ToList();
                int skillRequired = 0;
                p.Skills.ForEach(s =>
                {
                    if (developerSkills.Contains(s))
                        skillRequired += 1;
                }
                );
                if (skillRequired == p.Skills.Count())
                    p.ProjectDuration -= 1;
            }
            );
            //check if project is finished
            //projects.RemoveAll(p => p.ProjectDuration == 0);
            projects.Where(p=>p.ProjectDuration == 0).ToList().ForEach(p=> {
                Company company = Game.Companies.FirstOrDefault(c => c.Projects.Select(cp => cp.ProjectID).Contains(p.ProjectID) );
                company.Funds += p.ProjectPay;
                company.Projects.RemoveAll(cp=>cp.ProjectID == p.ProjectID);
                Game.Projects.RemoveAll(gp => gp.ProjectID == p.ProjectID);
            });

            return projects.Count;
        }
    }
}

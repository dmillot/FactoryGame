using Diiage2022.Factory_Games.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diage2022.Factory_Game.Server.Services
{
    public partial class Services
    {
        public int GenerateSkills()
        {
            int nbLvl = 3;
            for(int index=0; index < skillsNames.Count; index++)
            {
                for (int k = 1; k <= nbLvl; k++)
                {
                    Skill skill = new Skill();
                    skill.SkillId = index * nbLvl + k;
                    skill.SkillName = skillsNames[index];
                    skill.SkillLevel = k;
                    Game.Skills.Add(skill);
                }
            }
            return Game.Skills.Count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPGGame.Dtos.Character;
using RPGGame.Dtos.Skill;
using RPGGame.Models;

namespace RPGGame.Services.SkillService
{
    public interface ISkillService
    {
        Task<ServiceResponse<GetCharacterDto>> AddSkill(AddSkillDto newSkill);
    }
}
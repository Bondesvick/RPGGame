using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RPGGame.Dtos.Character;
using RPGGame.Dtos.Skill;
using RPGGame.Dtos.Weapon;
using RPGGame.Models;

namespace RPGGame
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>().ForMember(dto => dto.Skills, c => c.MapFrom(ch => ch.CharacterSkills.Select(cs => cs.Skill)));
            CreateMap<AddCharacterDTo, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }
    }
}
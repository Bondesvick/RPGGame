using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGGame.Dtos.Skill
{
    public class AddCharacterSkillDto
    {
        public int SkillId { get; set; }
        public int CharacterId { get; set; }
    }
}
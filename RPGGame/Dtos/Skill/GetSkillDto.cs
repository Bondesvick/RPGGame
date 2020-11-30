using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RPGGame.Dtos.Skill
{
    public class GetSkillDto
    {
        public string Name { get; set; }
        public string Damage { get; set; }
    }
}
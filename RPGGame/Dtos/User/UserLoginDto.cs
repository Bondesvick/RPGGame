using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGGame.Dtos.User
{
    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
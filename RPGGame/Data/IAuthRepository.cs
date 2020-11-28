using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPGGame.Models;

namespace RPGGame.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string passWord);

        Task<ServiceResponse<string>> Login(string userName, string passWord);

        Task<bool> UserExist(string userName);
    }
}
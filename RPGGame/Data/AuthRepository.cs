using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RPGGame.Models;

namespace RPGGame.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="user">the user to </param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<int>> Register(User user, string passWord)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (await UserExist(user.UserName))
            {
                response.Success = false;
                response.Message = "User already Exist!";
                return response;
            }

            CreatePasswordHash(passWord, out byte[] passwordHash, out byte[] passwordSalt);

            user.PassWordHash = passwordHash;
            user.PassWordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;

            return response;
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="userName">userName of user to login</param>
        /// <param name="passWord">password of user to login</param>
        /// <returns>Returned response ofter user Login</returns>
        public async Task<ServiceResponse<string>> Login(string userName, string passWord)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(userName.ToLower()));

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found!";
            }
            else if (!VerifyPasswordHash(passWord, user.PassWordHash, user.PassWordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password!";
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }

        /// <summary>
        /// Checks if the user exist
        /// </summary>
        /// <param name="userName">userName of the user to check</param>
        /// <returns>confirms if the user exist or not</returns>
        public async Task<bool> UserExist(string userName)
        {
            if (await _context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates Password Hash
        /// </summary>
        ///<param name="password">password</param>
        /// <param name="passwordHash">password Hash</param>
        /// <param name="passwordSalt">password Salt</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Verifies Password Hash
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="passwordHash">password Hash</param>
        /// <param name="passwordSalt">password Salt</param>
        /// <returns>confirms if the password is correct or not</returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Create JWT Token
        /// </summary>
        /// <param name="user">user whose token is to be created for</param>
        /// <returns>the generated token</returns>
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
                );

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
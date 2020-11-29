using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RPGGame.Data;
using RPGGame.Dtos.Character;
using RPGGame.Dtos.Skill;
using RPGGame.Models;

namespace RPGGame.Services.SkillService
{
    public class SkillService : ISkillService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SkillService(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddSkill(AddSkillDto newSkill)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character character = await _dataContext.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c =>
                    c.Id == newSkill.CharacterId && c.User.Id == GetUserId());

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found!";
                    return response;
                }

                Skill skill = new Skill
                {
                    Name = newSkill.Name,
                    Damage = newSkill.Damage
                };

                await _dataContext.Skills.AddAsync(skill);
                await _dataContext.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Get Id of logged in User
        /// </summary>
        /// <returns>Id of logged in User</returns>
        private int GetUserId() =>
            int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
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
using RPGGame.Models;

namespace RPGGame.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get Id of logged in User
        /// </summary>
        /// <returns>Id of logged in User</returns>
        private int GetUserId() =>
            int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        /// <summary>
        ///
        /// </summary>
        /// <param name="newCharacter"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDTo newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = _context.Characters.Where(c => c.User.Id == GetUserId()).Select(val => _mapper.Map<GetCharacterDto>(val)).ToList();

            return serviceResponse;
        }

        /// <summary>
        /// Updates a particular character
        /// </summary>
        /// <param name="updatedCharacter">character details to update</param>
        /// <returns>updated character details</returns>
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDTo updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character character = _context.Characters.Include(c => c.User).FirstOrDefault(c => c.Id == updatedCharacter.Id);

                if (character.User.Id == GetUserId())
                {
                    character.Name = updatedCharacter.Name;
                    character.Class = updatedCharacter.Class;
                    character.Defense = updatedCharacter.Defense;
                    character.HitPoints = updatedCharacter.HitPoints;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.Strength = updatedCharacter.Strength;

                    await Task.Run(() => _context.Characters.Update(character));
                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found!";
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        /// <summary>
        /// Deletes a particular Character
        /// </summary>
        /// <param name="id">Id of character to delete</param>
        /// <returns>response containing list remaining character after successful delete</returns>
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character != null)
                {
                    await Task.Run(() => _context.Characters.Remove(character));
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = _context.Characters.Where(c => c.User.Id == GetUserId())
                        .Select(val => _mapper.Map<GetCharacterDto>(val)).ToList();
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found!";
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        /// <summary>
        /// Fetches all the available character added by the logged in user
        /// </summary>
        /// <returns>returned response with the list of characters</returns>
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacters = await _context.Characters
                .Where(c => c.User.Id == GetUserId()).ToListAsync();

            serviceResponse.Data = dbCharacters.Select(val => _mapper.Map<GetCharacterDto>(val)).ToList();
            return serviceResponse;
        }

        /// <summary>
        /// Fetches a character by its Id
        /// </summary>
        /// <param name="id">Id if character</param>
        /// <returns>the returned response with the character</returns>
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            Character character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);

            return serviceResponse;
        }
    }
}
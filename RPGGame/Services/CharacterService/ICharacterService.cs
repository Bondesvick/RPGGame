using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPGGame.Dtos.Character;
using RPGGame.Models;

namespace RPGGame.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();

        public Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);

        public Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDTo newCharacter);

        public Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDTo updateCharacter);

        public Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
    }
}
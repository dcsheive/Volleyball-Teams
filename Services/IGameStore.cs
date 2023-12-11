using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volleyball_Teams.Models;

namespace Volleyball_Teams.Services
{
    public interface IGameStore
    {
        Task Init();
        Task<bool> AddGameAsync(GameDB item);
        Task<bool> UpdateGameAsync(GameDB item);
        Task<bool> UpdateGamesAsync(List<GameDB> item);
        Task<bool> DeleteGameAsync(GameDB item);
        Task DeleteGameByIdAsync (int id);
        Task DeleteAllGamesAsync();
        Task<List<GameDB>> GetGamesAsync();
        Task<GameDB> GetGameAsync(int id);
    }
}

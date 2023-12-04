using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volleyball_Teams.Models;

namespace Volleyball_Teams.Services
{
    public interface IPlayerStore
    {
        Task<bool> AddPlayerAsync(Player item);
        Task<bool> UpdatePlayerAsync(Player item);
        Task<bool> UpdatePlayersAsync(List<Player> item);
        Task<bool> DeletePlayerAsync(Player item);
        Task DeleteAllPlayersAsync();
        Task<Player?> GetPlayerAsync(int id);
        Task<Player?> GetPlayerByNameAsync(string name);
        Task<List<Player>> GetPlayersAsync();
        Task<List<Player>> GetPlayersHereAsync();
        Task SetPlayerRanksByRatio();
    }
}


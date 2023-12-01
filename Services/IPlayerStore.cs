using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Volleyball_Teams.Services
{
    public interface IPlayerStore<T>
    {
        Task<bool> AddPlayerAsync(T item);
        Task<bool> UpdatePlayerAsync(T item);
        Task<bool> UpdatePlayersAsync(List<T> item);
        Task<bool> DeletePlayerAsync(T item);
        Task DeleteAllPlayersAsync();
        Task<T?> GetPlayerAsync(int id);
        Task<T?> GetPlayerByNameAsync(string name);
        Task<List<T>> GetPlayersAsync();
        Task<List<T>> GetPlayersHereAsync();
        Task SetPlayerRanksByRatio();
    }
}

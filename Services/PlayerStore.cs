using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volleyball_Teams.Models;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.Services
{
    public class PlayerStore : IPlayerStore<Player>
    {
        SQLiteAsyncConnection Database;
        async Task Init()
        {
            if (Database is not null)
                return;

            Database = GetConnection();
            var result = await Database.CreateTableAsync<Player>();
        }

        private SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task<bool> AddPlayerAsync(Player item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            await Init();
            int rows = await Database.InsertAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdatePlayerAsync(Player item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdatePlayersAsync(List<Player> item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAllAsync(item);
            return rows > 0;
        }

        public async Task<bool> DeletePlayerAsync(Player item)
        {
            int rows = await Database.DeleteAsync(item);
            return rows > 0;
        }

        public async Task DeleteAllPlayersAsync()
        {
            int rows = await Database.DeleteAllAsync<Player>();
        }

        public async Task<Player?> GetPlayerAsync(int id)
        {
            return await Database.Table<Player>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Player?> GetPlayerByNameAsync(string name)
        {
            return await Database.Table<Player>().Where(s => s.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            await Init();
            return await Database.Table<Player>().ToListAsync();
        }
        public async Task<List<Player>> GetPlayersHereAsync()
        {
            await Init();
            return await Database.Table<Player>().Where(i => i.IsHere).ToListAsync();
        }
    }
}
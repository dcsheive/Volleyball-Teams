using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volleyball_Teams.Models;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.Services
{
    public class GameStore : IGameStore
    {
        SQLiteAsyncConnection Database;
        public async Task Init()
        {
            if (Database is not null)
                return;

            Database = GetConnection();
            var result = await Database.CreateTableAsync<GameDB>();
        }

        private SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task<bool> AddGameAsync(GameDB item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.InsertAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdateGameAsync(GameDB item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdateGamesAsync(List<GameDB> item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAllAsync(item);
            return rows > 0;
        }

        public async Task<bool> DeleteGameAsync(GameDB item)
        {
            int rows = await Database.DeleteAsync(item);
            return rows > 0;
        }

        public async Task DeleteAllGamesAsync()
        {
            await Database.DeleteAllAsync<GameDB>();
        }

        public async Task<List<GameDB>> GetGamesAsync()
        {
            return await Database.Table<GameDB>().ToListAsync();
        }
        public async Task<GameDB?> GetGameAsync(int id)
        {
            return await Database.Table<GameDB>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteGameByIdAsync(int id)
        {
            await Database.DeleteAsync(await GetGameAsync(id));
        }
    }
}
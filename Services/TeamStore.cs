using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volleyball_Teams.Models;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.Services
{
    public class TeamStore : ITeamStore
    {
        SQLiteAsyncConnection Database;
        async Task Init()
        {
            if (Database is not null)
                return;

            Database = GetConnection();
            var result = await Database.CreateTableAsync<TeamDB>();
        }

        private SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task<bool> AddTeamAsync(TeamDB item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            await Init();
            int rows = await Database.InsertAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdateTeamAsync(TeamDB item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAsync(item);
            return rows > 0;
        }

        public async Task<bool> UpdateTeamsAsync(List<TeamDB> item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            int rows = await Database.UpdateAllAsync(item);
            return rows > 0;
        }

        public async Task<bool> DeleteTeamAsync(TeamDB item)
        {
            int rows = await Database.DeleteAsync(item);
            return rows > 0;
        }

        public async Task DeleteAllTeamsAsync()
        {
            await Init();
            await Database.DeleteAllAsync<TeamDB>();
        }

        public async Task<List<TeamDB>> GetTeamsAsync()
        {
            await Init();
            return await Database.Table<TeamDB>().ToListAsync();
        }
        public async Task<TeamDB?> GetTeamAsync(int id)
        {
            return await Database.Table<TeamDB>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteTeamByIdAsync(int id)
        {
            await Database.DeleteAsync(await GetTeamAsync(id));
        }

        public async Task<bool> CheckIfExists(string idstr)
        {
            await Init();
            return await Database.Table<TeamDB>().Where(e => e.IDStr == idstr).FirstOrDefaultAsync() != null;
        }
    }
}
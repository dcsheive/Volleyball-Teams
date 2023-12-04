using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volleyball_Teams.Models;

namespace Volleyball_Teams.Services
{
    public interface ITeamStore
    {
        Task<bool> AddTeamAsync(TeamDB item);
        Task<bool> UpdateTeamAsync(TeamDB item);
        Task<bool> UpdateTeamsAsync(List<TeamDB> item);
        Task<bool> DeleteTeamAsync(TeamDB item);
        Task DeleteTeamByIdAsync (int id);
        Task DeleteAllTeamsAsync();
        Task<List<TeamDB>> GetTeamsAsync();
        Task<TeamDB> GetTeamAsync(int id);
        Task<bool> CheckIfExists(string idstr);

    }
}

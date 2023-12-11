using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class Game: List<Team>
    {
        public int Id { get; set; }
        public bool LeftWins { get; set; }
        public bool HasWinner { get; set; }
        public string Winner { get; set; } = "";
        public Team LeftTeam { get; set; }
        public Team RightTeam { get; set; }
        public int LeftTeamScore { get; set; }
        public int RightTeamScore { get; set; }
    }
}

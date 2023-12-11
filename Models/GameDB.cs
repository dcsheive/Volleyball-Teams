using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class GameDB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LeftTeamId { get; set; }
        public int RightTeamId { get; set; }
        public bool LeftWins { get; set; }
        public bool HasWinner { get; set; }
        public int LeftTeamScore { get; set; }
        public int RightTeamScore { get; set; }

    }
}

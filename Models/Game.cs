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
        public int Winner { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
    }
}

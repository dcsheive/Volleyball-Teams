using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class Team : List<Player>
    {
        public int Number { get; private set; }
        public string NumberText { get; private set; }

        public Team(int number, List<Player> players) : base(players)
        {
            Number = number;
            NumberText = (number + 1).ToString();
        }
    }
}

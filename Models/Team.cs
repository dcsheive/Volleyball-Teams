using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class Team : List<Player>
    {
        public int Number { get; set; }
        public string NumberText
        {
            get
            {
                return (Number + 1).ToString();
            }
        }
        public int Power { get; set; }

        public Team(int number, List<Player> players) : base(players)
        {
            Number = number;
        }
    }
}

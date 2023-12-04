using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class TeamList : List<Team>
    {
        public int Id { get; set; }

        public TeamList(int number, List<Team> teams) : base(teams)
        {
            Id = number;
        }
    }
}

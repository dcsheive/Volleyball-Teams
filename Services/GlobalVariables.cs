using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volleyball_Teams.Models;

namespace Volleyball_Teams.Services
{
    public class GlobalVariables : IGlobalVariables
    {
        public Team? LeftTeam { get; set; } = null;
        public Team? RightTeam { get; set; } = null;
        public int LeftScore { get; set; }
        public int RightScore { get; set; }
        public bool NewGame { get; set; }
    }
}

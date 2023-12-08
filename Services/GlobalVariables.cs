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
        public Team LeftTeam { get; set; }
        public Team RightTeam { get; set; }
    }
}

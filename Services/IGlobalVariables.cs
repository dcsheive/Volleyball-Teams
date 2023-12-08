using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volleyball_Teams.Models;

namespace Volleyball_Teams.Services
{
    public interface IGlobalVariables
    {
        Team LeftTeam { get; set; }
        Team RightTeam { get; set; }
    }
}

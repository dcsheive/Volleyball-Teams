using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class TeamDB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PlayerIdStr { get; set; } = "";
        public int Power { get; set; }
        public string Name { get; set; } = "";
        public int NumWins { get; set; } = 0;
        public int NumLosses { get; set; } = 0;
        public bool IsRandom { get; set; } = false;

    }
}

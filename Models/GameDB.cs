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
        public int Team1 { get; set; }
        public int Team2 { get; set; }
        public int Winner { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

    }
}

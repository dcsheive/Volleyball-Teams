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
        public string IDStr { get; set; } = "";
    }
}

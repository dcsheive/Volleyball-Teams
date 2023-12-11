using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Models
{
    public class Team : List<Player>, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Power { get; set; }
        public string Name { get; set; }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }
        public string NameDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return "Team " + (Number + 1).ToString();
                else
                    return Name;
            }
        }
        public string WinsDisplay
        {
            get
            {
                return NumWins + " - " + NumLosses;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public Team(List<Player> players) : base(players)
        {

        }

        public Team(int number, List<Player> players) : base(players)
        {
            Number = number;
        }

        public Team(int number, string name, List<Player> players) : base(players)
        {
            Number = number;
            Name = name;
        }
    }
}

using SQLite;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.Models
{
    public class Player : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsHere { get; set; } = true;
        public string NumStars { get; set; } = "1";
        public string NumStarsRatio { get; set; } = "1";
        public string NumStarsDisplay
        {
            get
            {
                return Preferences.Get(Constants.Settings.UseScore, false) ? NumStarsRatio : NumStars;
            }
        }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }
        public string WinsDisplay
        {
            get
            {
                return NumWins + " - " + NumLosses;
            }
        }
        [Ignore]
        public bool IsBeingDragged { get; set; } = false;
        [Ignore]
        public bool IsBeingDraggedOver { get; set; } = false;
        [Ignore]
        public Team? Team { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
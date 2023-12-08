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
        public int NumWins { get; set; } = 0;
        public int NumLosses { get; set; } = 0;
        [Ignore]
        public bool IsChecked { get; set; } = false;
        [Ignore]
        public string NumStarsDisplay
        {
            get
            {
                return Settings.UseScore ? NumStarsRatio : NumStars;
            }
        }
        [Ignore]
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
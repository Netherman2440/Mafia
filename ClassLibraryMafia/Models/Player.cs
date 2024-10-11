using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;


namespace Mafia.Models
{
    public class Player : INotifyPropertyChanged
    {
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Score
        {
            get => score;
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged();
                }
            }
        }

        public Role Role
        {
            get => role;
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged();
                }
            }
        }

        private string name;
        private int score;
        private Role role;

        public bool IsPaired { get; set; }
        public bool IsAlive { get; set; }
        public bool CantVote { get; set; }

        public int succesRoleActivites;

        public int secondVotingCount;   //how many times were you in second round of voting

        public int currentDayPoints;    //points collected during this day

        public int[] pointHistory = new int[40];

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Player()
        {
            IsAlive = true;
        }
        public Player(string name)
        {
            this.Name = name;
            IsAlive = true;
        }
    }
}

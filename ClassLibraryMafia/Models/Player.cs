namespace Mafia.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public Role Role { get; set; }
        public bool IsPaired { get; set; }
        public bool IsAlive { get; set; }
        public bool CantVote { get; set; }

        public int secondVotingCount;   //how many times were you in second round of voting

        public int currentDayPoints;    //points collected during this day

        public List<int> pointHistory = new();

        public Player()
        {
            
        }
        public Player(string name)
        {
            this.Name = name;
            IsAlive = true;
        }
    }
}

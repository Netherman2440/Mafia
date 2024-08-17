namespace Mafia.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public Role Role { get; set; }

        public bool IsPaired { get; set; }

        public List<int> pointHistory = new();

    }
}

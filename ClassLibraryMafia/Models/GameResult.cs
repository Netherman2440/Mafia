using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    public class GameResult
    {
        public string winners;
        public List<Player>[] deathHistory;

        public List<Player> players;
        public int days;

        public bool dealerWins;
        public bool pairWins;
        public bool renegatWins;
        public int renegatKills;
        public GameResult(string winners, List<Player>[] deathHistory, List<Player> players, int days, int renegatKills)
        {
            this.winners = winners;
            this.deathHistory = deathHistory;
            this.players = players;

            this.days = days;

            dealerWins = players.FindAll(p =>p.IsAlive && p.Role == Role.Dealer).Count() > 0;
            pairWins = players.FindAll(p =>p.IsAlive && p.IsPaired).Count() == 2;
            renegatWins = players.FindAll(p =>p.IsAlive && p.Role == Role.Renegat).Count() > 0;
            this.renegatKills = renegatKills;

        }
    }
}

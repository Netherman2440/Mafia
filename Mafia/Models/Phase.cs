using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    internal class Phase
    {
        public string Name { get; set; }
        public string Description { get; set; } //displayed description of 

        public List<Player> selectedPlayers = new();

        public Role phaseRole;  //if set to "Pair" then instead of setting role, we set bool 'IsPaired'

        public bool isInitial;  //if true, then role will aplayed to phase player


        public void ChangeSelection(Player player)
        {
            if(selectedPlayers.Contains(player))
            {
                selectedPlayers.Remove(player);
            }
            else
            { selectedPlayers.Add(player); }
        }
    }
}

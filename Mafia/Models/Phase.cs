using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    public class Phase
    {
        public string Name { get; set; }
        public string Description { get; set; } //displayed description of 
        public PhaseEnum PhaseEnum { get; set; }

        public List<Player> selectedPlayers = new();

        public Role Role;  

        public bool isInitial;  //if true, then role will aplayed to phase player

        public int limitOfSelections = -1;

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

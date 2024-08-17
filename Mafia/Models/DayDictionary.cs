using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    internal class DayDictionary
    {
        public Day GetFirstDay()
        {
            List<Phase> phases = new()
            {
                GetPhase(Role.Amor, true),
                GetPhase(Role.Pair, true),
                GetPhase(Role.Jester,true),
                GetPhase(Role.Vendetta,true),

                GetPhase(Role.Barman, true),
                GetPhase(Role.Barman),
                GetPhase(Role.Bodyguard, true),
                GetPhase(Role.Bodyguard),
                GetPhase(Role.Dealer, true),
                GetPhase(Role.Dealer),
                GetPhase(Role.Mafia, true),
                GetPhase(Role.Mafia),
                GetPhase(Role.Agent, true),
                GetPhase(Role.Agent),

            };


            return new Day(phases);
        }

        public Day GetNextDay()
        {
            List<Phase> phases = new()
            {
                GetPhase(Role.Barman),
                GetPhase(Role.Bodyguard),
                GetPhase(Role.Dealer),
                GetPhase(Role.Mafia),
                GetPhase(Role.Agent),

            };


            return new Day(phases);
        }

        private Phase GetPhase(Role role, bool initialRound = false)
        {
            Phase phase = new()
            {
                Name = role.ToString(),
                phaseRole = role,
                isInitial = initialRound,
            };


            switch (role)
            {
                case Role.None:
                    break;
                case Role.Citizen:
                    phase.Description = initialRound ? "Who is citizien" : "Choose 2 persons to be voted out.";
                    break;
                case Role.Mafia:
                    phase.Description = initialRound ? "Who is Mafia" : "Choose 1 person to be killed.";
                    break;
                case Role.Agent:
                    phase.Description = initialRound ? "Who is agent" : "Choose 1 people to know his identity.";
                    break;
                case Role.Bodyguard:
                    phase.Description = initialRound ? "Who is bodyguard" : "Choose 1 people to be protected this night.";
                    break;
                case Role.Barman:
                    phase.Description = initialRound ? "Who is Barman" : "Choose 1 people to be protected this night.";
                    break;
                case Role.Dealer:
                    phase.Description = initialRound ? "Who is Dealer" : "Choose 1 people to be silent next day.";
                    break;
                case Role.Amor:
                    phase.Description = initialRound ? "Who is Amor" : "";
                    break;
                case Role.Pair:
                    phase.Description = initialRound ? "Who was picked by Amor" : "";
                    break;
                case Role.Jester:
                    phase.Description = initialRound ? "Who is the jester" : "";
                    break;
                case Role.Vendetta:
                    phase.Description = initialRound ? "Who is Vendetta" : "";
                    break;
            }
            return phase;
        }
    }

    public enum Role
    {
        None,
        Citizen,
        Mafia,
        Agent,
        Bodyguard,
        Amor,
        Barman,
        Dealer,
        Jester, //Suicide
        Vendetta,
        Pair    //
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    internal static class DayDictionary
    {
        private static readonly Dictionary<PhaseEnum, Role> _phaseToRoleMap = new Dictionary<PhaseEnum, Role>
{
    { PhaseEnum.Amor, Role.Amor },
    { PhaseEnum.Jester, Role.Jester },
    { PhaseEnum.Renegat, Role.Renegat },
    { PhaseEnum.Barman, Role.Barman },
    { PhaseEnum.Bodyguard, Role.Bodyguard },
    { PhaseEnum.Dealer, Role.Dealer },
    { PhaseEnum.Mafia, Role.Mafia },
    { PhaseEnum.Agent, Role.Agent },
};

        private static Role GetRole(PhaseEnum phaseEnum)
        {
            return _phaseToRoleMap.TryGetValue(phaseEnum, out var role) ? role : Role.None;
        }
        public static Day GetFirstDay()
        {
            List<Phase> phases = new()
            {
                GetPhase(PhaseEnum.Jester,true),
                GetPhase(PhaseEnum.Renegat,true),
                GetPhase(PhaseEnum.Amor, true),
                GetPhase(PhaseEnum.Amor),

                GetPhase(PhaseEnum.Barman, true),
                GetPhase(PhaseEnum.Barman),
                GetPhase(PhaseEnum.Bodyguard, true),
                GetPhase(PhaseEnum.Bodyguard),
                GetPhase(PhaseEnum.Dealer, true),
                GetPhase(PhaseEnum.Dealer),
                GetPhase(PhaseEnum.Mafia, true),
                GetPhase(PhaseEnum.Mafia),
                GetPhase(PhaseEnum.Agent, true),
                GetPhase(PhaseEnum.Agent),
                 GetPhase(PhaseEnum.NightSumUp),
                GetPhase(PhaseEnum.SecondVoting),
                GetPhase(PhaseEnum.WhoDied),
                GetPhase(PhaseEnum.WhoVoted),
                 GetPhase(PhaseEnum.DaySumUp)
            };


            return new Day(phases);
        }

        public static Day GetNextDay(bool amorCreatedPair)
        {
            List<Phase> phases = new()
            {
                GetPhase(PhaseEnum.Barman),
                GetPhase(PhaseEnum.Bodyguard),
                GetPhase(PhaseEnum.Dealer),
                GetPhase(PhaseEnum.Mafia),
                GetPhase(PhaseEnum.Agent),
                GetPhase(PhaseEnum.NightSumUp),
                 GetPhase(PhaseEnum.SecondVoting),
                GetPhase(PhaseEnum.WhoDied),
                GetPhase(PhaseEnum.WhoVoted),
                GetPhase(PhaseEnum.DaySumUp),
            };

            if (!amorCreatedPair)
            {
                phases.Insert(0, GetPhase(PhaseEnum.Amor));
            }


            return new Day(phases);
        }

        private static Phase GetPhase(PhaseEnum phaseEnum, bool initialRound = false)
        {
            Phase phase = new()
            {
                Name = phaseEnum.ToString(),
                Role = GetRole(phaseEnum),
                isInitial = initialRound,
            };


            switch (phaseEnum)
            {
                case PhaseEnum.Amor:
                    phase.Description = initialRound ? "Who is Amor" : "Who is picked by Amor";
                    phase.limitOfSelections = initialRound ? 1 : 2;

                    break;

                case PhaseEnum.Jester:
                    phase.Description = "Who is Jester?";
                    phase.limitOfSelections = initialRound ? 1 : 0;


                    break;
                case PhaseEnum.Renegat:
                    phase.Description = "Who is Renegat?";
                    phase.limitOfSelections = initialRound ? 1 : 0;

                    break;
                case PhaseEnum.Barman:
                    phase.Description = initialRound ? "Who is Barman" : "Who is picked by Barman?";
                    phase.limitOfSelections = 1;

                    break;
                case PhaseEnum.Bodyguard:
                    phase.Description = initialRound ? "Who is Bodyguard" : "Who is picked by Bodyguard?";
                    phase.limitOfSelections = initialRound ? -1 : 1;

                    break;
                case PhaseEnum.Dealer:
                    phase.Description = initialRound ? "Who is Dealer" : "Who is picked by Dealer?";
                    phase.limitOfSelections = initialRound ? 1 : 1;


                    break;
                case PhaseEnum.Mafia:
                    phase.Description = initialRound ? "Who is Mafia" : "Who is picked by Mafia?";
                    phase.limitOfSelections = initialRound ? -1 : 1;

                    break;
                case PhaseEnum.Agent:
                    phase.Description = initialRound ? "Who is Agent" : "Who is picked by Agent?";
                    phase.limitOfSelections = initialRound ? -1 : 1;

                    break;
                case PhaseEnum.NightSumUp:
                    phase.Description = "Night points summed up";
                    phase.limitOfSelections = 0;

                    break;
                case PhaseEnum.SecondVoting:
                    phase.Description = "Who is picked to second voting?";
                    phase.limitOfSelections = 2;

                    break;
                case PhaseEnum.WhoDied:
                    phase.Description = "Who died?";
                    phase.limitOfSelections = -1;

                    break;
                case PhaseEnum.WhoVoted:
                    phase.Description = "Who voted for dead person?";
                    break;
                case PhaseEnum.DaySumUp:
                    phase.Description = "Day points summed up";
                    phase.limitOfSelections = 0;

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
        Renegat,

    }

    public enum PhaseEnum
    {
        Amor,
        Jester,
        Renegat,
        Barman,
        Bodyguard,
        Dealer,
        Mafia,
        Agent,
        NightSumUp,
        SecondVoting,
        WhoDied,
        WhoVoted,
        DaySumUp
    }
}

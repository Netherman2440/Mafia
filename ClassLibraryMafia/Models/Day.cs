using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    public class Day
    {
        public List<Phase> Phases { get; private set; }

        public Phase CurrentPhase { get; private set; }

        public int Index;

        public Day(List<Phase> phases)
        {
                this.Phases = phases;

            CurrentPhase = Phases[0];
        }

        public void NextPhase()
        {
            Index++;

            if (Index > Phases.Count - 1)
            {
                //sum up points
                Index = Phases.Count - 1;
            }
               

            CurrentPhase = Phases[Index];
        }

        public void PrevPhase()
        {
            Index--;
            if (Index < 0)
                Index = 0;

            CurrentPhase = Phases[Index];
        }

        public Phase GetPhase(PhaseEnum phaseEnum)
        {
            return Phases.FirstOrDefault(p => p.PhaseEnum == phaseEnum);

        }
    }
}

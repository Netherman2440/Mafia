using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Models
{
    internal class Day
    {
        public List<Phase> Phases { get; private set; }

        public Phase CurrentPhase { get; private set; }

        public int Index;

        public Day(List<Phase> phases)
        {
                this.Phases = phases;
        }

        public void NextPhase()
        {
            Index++;
            if (Index > Phases.Count - 1)
            {
                //sum up points
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
    }
}

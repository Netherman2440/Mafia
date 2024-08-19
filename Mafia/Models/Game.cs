

namespace Mafia.Models
{
    internal class Game
    {
        public int DayIndex;

        public List<Day> Days = new();

        public List<Player> Players = new();

        private bool amorCreatedPair = false;

        public void StartGame()
        {
            var firstDay = DayDictionary.GetFirstDay();

            Days.Add(firstDay);
        }

        public void NextDay()
        {
            DayIndex++;

            if( DayIndex >= Days.Count - 1)
            {
                var nextDay = DayDictionary.GetNextDay(amorCreatedPair);

                Days.Add(nextDay);
            }
        }

        public void NextPhase()
        {
            Days[DayIndex].NextPhase();

            if(Days[DayIndex].CurrentPhase.PhaseEnum == PhaseEnum.NightSumUp)
            {
                //sum up night
                SumUpNight();
            }

            if (Days[DayIndex].CurrentPhase.PhaseEnum == PhaseEnum.DaySumUp)
            {
                //sum up day
                SumUpDay();

                //next phase means get next day
            }
        }
        private void SumUpNight()
        {
            foreach (Phase currentPhase in Days[DayIndex].Phases)
            {
                if (currentPhase.isInitial)
                {
                    SetRole(currentPhase);
                }
            }

            var amorPhase = Days[DayIndex].GetPhase(PhaseEnum.Amor);
            var bodyguardPhase = Days[DayIndex].GetPhase(PhaseEnum.Bodyguard);
            var dealerPhase = Days[DayIndex].GetPhase(PhaseEnum.Dealer);
            var mafiaPhase = Days[DayIndex].GetPhase(PhaseEnum.Mafia);

            if (amorPhase != null && !amorPhase.isInitial && amorPhase.selectedPlayers.Count > 0)    //Amor phase result
            {
                foreach (var player in amorPhase.selectedPlayers)
                {
                    player.IsPaired = true;
                }
            }

            if (dealerPhase != null && !dealerPhase.isInitial && dealerPhase.selectedPlayers.Count > 0)  //dealer effect
            {
                if (!BodyguardProtected(bodyguardPhase, dealerPhase.selectedPlayers))
                {
                    foreach (var player in dealerPhase.selectedPlayers)
                    {
                        player.CantVote = true;
                    }
                }
            }

            if (mafiaPhase != null && !mafiaPhase.isInitial && mafiaPhase.selectedPlayers.Count > 0)  //mafia effect
            {
                if (!BodyguardProtected(bodyguardPhase, mafiaPhase.selectedPlayers))
                {
                    foreach (var player in mafiaPhase.selectedPlayers)
                    {
                        player.IsAlive = false;
                    }
                }
            }

        }


        private void SumUpDay()
        {
            foreach (Phase currentPhase in Days[DayIndex].Phases)
            {
                if (currentPhase.isInitial) continue;

                else
                {
                    switch (currentPhase.PhaseEnum)
                    {
                        case PhaseEnum.Amor:
                            AmorPoints();
                            break;
                        case PhaseEnum.Jester:

                            break;
                        case PhaseEnum.Renegat:
                            RenegatPoints(currentPhase.selectedPlayers, Days[DayIndex].GetPhase(PhaseEnum.WhoDied));    //points for killing man selected during night
                            break;
                        case PhaseEnum.Barman:
                            BarmanPoints(currentPhase.selectedPlayers); //points for selecting mafia
                            break;
                        case PhaseEnum.Bodyguard:
                            BodyguardPoints(currentPhase, Days[DayIndex].GetPhase(PhaseEnum.Dealer), Days[DayIndex].GetPhase(PhaseEnum.Mafia)); //points for saving from mafia and dealer
                            break;
                        case PhaseEnum.Dealer:
                            DealerPoints(currentPhase.selectedPlayers, Days[DayIndex].GetPhase(PhaseEnum.Bodyguard));   //points for giving drugs to mafia, agent, or bodyguard
                            break;
                        case PhaseEnum.Mafia:
                            MafiaPoints(currentPhase.selectedPlayers, Days[DayIndex].GetPhase(PhaseEnum.Bodyguard));    //points for killing agent and bodyguard
                            break;
                        case PhaseEnum.Agent:
                            AgentPoints(currentPhase.selectedPlayers);  //points for selecting mafia
                            break;
                        case PhaseEnum.NightSumUp:
                            break;
                        case PhaseEnum.SecondVoting:
                            SecondVotingPoints(currentPhase.selectedPlayers);
                            break;
                        case PhaseEnum.WhoDied:
                            PlayerDied(currentPhase.selectedPlayers);
                            break;
                        case PhaseEnum.WhoVoted:
                            VotingForPoints(currentPhase.selectedPlayers, Days[DayIndex].GetPhase(PhaseEnum.WhoDied));
                            break;
                        case PhaseEnum.DaySumUp:
                            break;
                    }
                }
            }

            //moreover

            JesterPoints();

            

        }




        private void VotingForPoints(List<Player> selectedPlayers, Phase whoDiedPhase)
        {
            throw new NotImplementedException();
        }

        private void SecondVotingPoints(List<Player> selectedPlayers)
        {
            foreach(Player player in selectedPlayers)
            {
                player.secondVotingCount++;
            }

            //todo give points

        }

        private void DealerPoints(List<Player> selectedPlayers, Phase bodyguardPhase)
        {
            if(!BodyguardProtected(bodyguardPhase, selectedPlayers))
            {
                foreach (Player player in selectedPlayers)
                {
                    //give points if it is mafia, agent or bodyguard
                }
            }
        }

        private void RenegatPoints(List<Player> selectedPlayers, Phase whoDiedPhase)
        {
            foreach(var player in selectedPlayers)
            {
                if (whoDiedPhase.selectedPlayers.Contains(player))
                {
                    //give points
                }
            }
        }

        private void JesterPoints()
        {
            //give -1 to jester
            throw new NotImplementedException();

        }

        private void AmorPoints()
        {
            //give 1 additional point if pair is alive
            throw new NotImplementedException();
        }
        private void MafiaPoints(List<Player> selectedPlayers, Phase bodyguardPhase)
        {
            if(!BodyguardProtected(bodyguardPhase, selectedPlayers))
            {
                //give points if this was agent or bodyguard
            }
        }

        private void WhoVotedPoints(List<Player> selectedPlayers, Phase phase)
        {
            throw new NotImplementedException();
        }

        private void PlayerDied(List<Player> selectedPlayers)
        {
            throw new NotImplementedException();
        }


        private void AgentPoints(List<Player> selectedPlayers)
        {
            throw new NotImplementedException();
        }

        private void BodyguardPoints(Phase currentPhase, Phase phase1, Phase phase2)
        {
            throw new NotImplementedException();
        }

        private void BarmanPoints(List<Player> selectedPlayers)
        {
            throw new NotImplementedException();
        }

        private void SetRole(Phase phase)
        {
            foreach(var player in phase.selectedPlayers)
            {
                player.Role = phase.Role;
            }
        }

        private bool BodyguardProtected(Phase bodyguardPhase, List<Player> currentSelection)
        {
            bool bodyguardProtected = false;
            foreach( var player in currentSelection)
            {
                if (bodyguardPhase.selectedPlayers.Contains(player))
                {
                    bodyguardProtected = true;
                }
            }

            return bodyguardProtected;
        }
    }
}

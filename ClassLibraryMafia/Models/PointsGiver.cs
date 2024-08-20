namespace Mafia.Models
{
    public class PointsGiver
    {
        private List<Player> players;
        private Day day;

        public PointsGiver()
        {
                
        }
        public PointsGiver(Day day)
        {
            this.day = day;
        }

        public void GiveWinPoints(ref List<Player> players)
        {
            var mafia = GetAlivePlayers(Role.Mafia);

            var city = GetAlivePlayers(new List<Role>() { Role.Citizen, Role.Barman, Role.Agent, Role.Bodyguard });

            var neutral = GetAlivePlayers(new List<Role>() { Role.Renegat, Role.Jester, Role.Amor, Role.Dealer });
            var stayAliveNeutral = GetAlivePlayers(new List<Role>() { Role.Renegat,  Role.Amor, Role.Dealer });

            if (mafia.Count == 0)
            {
                //City wins

                foreach(var player in city)
                {
                    if(!(player.IsPaired && player.IsAlive))
                    player.currentDayPoints += 5;
                }

                //neutral wins
                foreach(var player in stayAliveNeutral)
                {
                    if (!(player.IsPaired && player.IsAlive))
                        player.currentDayPoints += 5;
                }

                //pair wins
                foreach (var player in players)
                {
                    if(  player.IsPaired && player.IsAlive) 
                        player.currentDayPoints += 5;
                }

            }

            if (mafia.Count > city.Count + neutral.Count)
            {
                //Mafia wins

                foreach (var player in mafia)
                {
                    if (!(player.IsPaired && player.IsAlive))
                        player.currentDayPoints += 5;
                }

                //neutral wins
                foreach (var player in stayAliveNeutral)
                {
                    if (!(player.IsPaired && player.IsAlive))
                        player.currentDayPoints += 5;
                }

                //pair wins
                foreach (var player in players)
                {
                    if (player.IsPaired && player.IsAlive)
                        player.currentDayPoints += 5;
                }
            }
        }

        public void GiveNightPoints(ref List<Player> players)
        {
            this.players = players;

            foreach (Phase currentPhase in day.Phases)
            {
                if (currentPhase.isInitial) continue;

                else
                {
                    switch (currentPhase.PhaseEnum)
                    {
                        case PhaseEnum.Barman:
                            BarmanPoints(currentPhase.selectedPlayers); //points for selecting mafia 
                            break;
                        case PhaseEnum.Bodyguard:
                            BodyguardPoints(currentPhase, day.GetPhase(PhaseEnum.Dealer), day.GetPhase(PhaseEnum.Mafia)); //points for saving from mafia and dealer
                            break;
                        case PhaseEnum.Dealer:
                            DealerPoints(currentPhase.selectedPlayers, day.GetPhase(PhaseEnum.Bodyguard));   //points for giving drugs to mafia, agent, or bodyguard
                            break;
                        case PhaseEnum.Mafia:
                            MafiaPoints(currentPhase.selectedPlayers, day.GetPhase(PhaseEnum.Bodyguard));    //points for killing agent and bodyguard
                            break;
                        case PhaseEnum.Agent:
                            AgentPoints(currentPhase.selectedPlayers);  //points for selecting mafia
                            break;
                    }
                }
            }
        }

        public void GiveDayPoints(ref List<Player> players)
        {
            this.players = players;

            foreach (Phase currentPhase in day.Phases)
            {
                if (currentPhase.isInitial) continue;

                else
                {
                    switch (currentPhase.PhaseEnum)
                    {
                        case PhaseEnum.Renegat:
                            RenegatPoints(currentPhase.selectedPlayers, day.GetPhase(PhaseEnum.WhoDied));    //points for killing man selected during night
                            break;
                        case PhaseEnum.SecondVoting:
                            SecondVotingPoints(currentPhase.selectedPlayers);   //points for second round of voting
                            break;
                        case PhaseEnum.WhoVoted:
                            VotingForPoints(currentPhase.selectedPlayers, day.GetPhase(PhaseEnum.WhoDied));
                            break;
                        case PhaseEnum.DaySumUp:
                            PointsForLiving();
                            break;
                    }
                }
            }

            AmorPoints();
        }

        public int JesterPoints(int playersCount, int dayIndex)
        {
            int jesterPoints = playersCount / 2 - dayIndex;

            return jesterPoints;
        }

        private void PointsForLiving()
        {
            foreach(var player in players)
            {
                if (player.IsAlive)
                {
                    player.currentDayPoints += 1;

                    if (player.IsPaired)
                    {
                        player.currentDayPoints += 1;
                    }
                }
            }
        }


        private void VotingForPoints(List<Player> selectedPlayers, Phase whoDiedPhase)
        {
            Player deadMan = whoDiedPhase.selectedPlayers[0];

            var citiziens = GetAlivePlayers(Role.Citizen, selectedPlayers);
            var otherCity = GetAlivePlayers(new List<Role> { Role.Agent, Role.Bodyguard, Role.Barman }, selectedPlayers);


            var mafia = GetAlivePlayers(Role.Mafia, selectedPlayers);
            
            var neutrals = GetAlivePlayers(new List<Role> { Role.Dealer, Role.Amor, Role.Jester, Role.Renegat }, selectedPlayers);

            if (deadMan == null) return;

            if(deadMan.Role == Role.Mafia)
            {
                // 3 points for obyvatels
                GiveRolePoints(citiziens, 3);

                GiveRolePoints(otherCity, 1);

                GiveRolePoints(neutrals, 1);
            }

            if(deadMan.Role == Role.Citizen)
            {
                GivePoints(neutrals, 1);
                GivePoints(mafia, 1);
            }
            
            if(deadMan.Role == Role.Agent || deadMan.Role == Role.Bodyguard)
            {
                GivePoints(neutrals, 1);
                GivePoints(mafia, 3);
            }


        }

        private void SecondVotingPoints(List<Player> selectedPlayers)   // 1/3/5/7 points for being in second voting round
        {
            foreach (Player player in selectedPlayers)
            {
                player.currentDayPoints += Math.Min(player.secondVotingCount * 2 + 1, 7);

                player.secondVotingCount++;
            }

        }

        private void DealerPoints(List<Player> selectedPlayers, Phase bodyguardPhase)   //dealer points
        {
            var dealers = GetAlivePlayers(Role.Dealer);

            if (!BodyguardProtected(bodyguardPhase, selectedPlayers))
            {
                foreach (Player player in selectedPlayers)
                {
                    if (player.Role == Role.Mafia || player.Role == Role.Agent || player.Role == Role.Bodyguard)  //dealer picked mafia, agent or bodyguard
                    {
                        GiveRolePoints(dealers, 3);
                    }
                }
            }
        }


        private void RenegatPoints(List<Player> selectedPlayers, Phase whoDiedPhase)    //renegat points
        {
            var renegats = GetPlayers(Role.Renegat);    //renegat will get points even after death

            foreach (var player in selectedPlayers)
            {
                if (whoDiedPhase.selectedPlayers.Contains(player))
                {
                    //give points
                    GiveRolePoints(renegats, 3);
                }
            }
        }

        private void AmorPoints()   //amor points
        {
            //give 1 additional point if pair is alive
            var amors = GetPlayers(Role.Amor);

            bool pairAlive = false;

            foreach (var player in players)
            {
                if (player.IsPaired && player.IsAlive)
                {
                    pairAlive = true;
                }
            }

            if (pairAlive)
                GiveRolePoints(amors, 1);
        }
        private void MafiaPoints(List<Player> selectedPlayers, Phase bodyguardPhase)    //3 points for agent or bodyguard
        {
            var mafia = GetAlivePlayers(Role.Mafia);

            if (!BodyguardProtected(bodyguardPhase, selectedPlayers))
            {
                //give points if this was agent or bodyguard

                foreach (var player in selectedPlayers)
                {
                    if (player.Role == Role.Agent || player.Role == Role.Bodyguard)
                    {
                        GiveRolePoints(mafia, 3);
                    }
                }

            }
        }

        private void AgentPoints(List<Player> selectedPlayers)  // 3 points for choosing mafia
        {
            var agents = GetAlivePlayers(Role.Agent);

            foreach (var player in selectedPlayers)
            {
                if (player.Role == Role.Mafia)
                {
                    GiveRolePoints(agents, 3);
                }
            }
        }

        private void BodyguardPoints(Phase bodyguardPhase, Phase dealerPhase, Phase mafiaPhase) // 3 points for saving from dealer or mafia
        {
            var bodyguards = GetAlivePlayers(Role.Bodyguard);

            foreach (var player in bodyguardPhase.selectedPlayers)
            {
                if (dealerPhase.selectedPlayers.Contains(player))
                {
                    GiveRolePoints(bodyguards, 3);
                    break;
                }

                if (mafiaPhase.selectedPlayers.Contains(player))
                {
                    GiveRolePoints(bodyguards, 3);
                    break;
                }
            }
        }

        private void BarmanPoints(List<Player> selectedPlayers) // 3 points for choosing mafia
        {
            var barmans = GetAlivePlayers(Role.Barman);

            foreach (var player in selectedPlayers)
            {
                //give points if player is mafia
                if (player.Role == Role.Mafia)
                {

                    GiveRolePoints(barmans, 3);
                }
            }
        }

        private bool BodyguardProtected(Phase bodyguardPhase, List<Player> currentSelection)
        {
            bool bodyguardProtected = false;
            foreach (var player in currentSelection)
            {
                if (bodyguardPhase.selectedPlayers.Contains(player))
                {
                    bodyguardProtected = true;
                }
            }

            return bodyguardProtected;
        }

        private void GiveRolePoints(List<Player> players, int v)
        {
            foreach (Player player in players)
            {
                if(!player.IsPaired)
                player.currentDayPoints += v;
            }
        }

        private void GivePoints(List<Player> players, int v)    //points for 
        {
            foreach (Player player in players)
            {
                    player.currentDayPoints += v;
            }
        }


        private List<Player> GetPlayers(Role role)
        {
            return GetPlayers(role, players);
        }

        private List<Player> GetPlayers(Role role, List<Player> players)
        {
            var result = players.FindAll((p) => p.Role == role);

            return result;
        }

        private List<Player> GetAlivePlayers(Role role)
        {
            return GetPlayers(role, players);
        }

        private List<Player> GetAlivePlayers(List<Role> roles)
        {
            return GetAlivePlayers(roles, players);
        }

        private List<Player> GetAlivePlayers(Role role, List<Player> players)
        {
            return GetAlivePlayers(new List<Role> { role }, players);
        }

        private List<Player> GetAlivePlayers(List<Role> roles, List<Player> Players)
        {
            var list = new List<Player>();

            foreach (var role in roles)
            {
                var results = Players.FindAll((p) => p.IsAlive && p.Role == role);

                list.AddRange(results);
            }

            return list;
        }
    }
}

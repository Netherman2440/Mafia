

namespace Mafia.Models
{
    public class Game
    {
        public int DayIndex;

        public Day CurrentDay => Days[DayIndex];

        public List<Day> Days = new();

        public List<Player> Players = new();

        private bool amorCreatedPair = false;

        public void StartGame()
        {
            var firstDay = DayDictionary.GetFirstDay();

            Days.Add(firstDay);
        }

        public void SelectPlayers(List<Player> players)
        {
            CurrentDay.CurrentPhase.selectedPlayers = players;
        }

        public void NextPhase()
        {
            if (CurrentDay.CurrentPhase.PhaseEnum == PhaseEnum.DaySumUp)
            {
                SavePoints();

                NextDay();
                return;
            }


                CurrentDay.NextPhase();

            if(CurrentDay.CurrentPhase.PhaseEnum == PhaseEnum.NightSumUp)
            {
                //sum up night
                SumUpNight();
            }

            if (CurrentDay.CurrentPhase.PhaseEnum == PhaseEnum.DaySumUp)
            {
                //sum up day
                SumUpDay();

                //next phase means get next day
            }
        }

        private void NextDay()
        {
            DayIndex++;

            if (DayIndex >= Days.Count - 1)
            {
                var nextDay = DayDictionary.GetNextDay(amorCreatedPair);

                Days.Add(nextDay);
            }
        }

        private void SavePoints()
        {
            foreach(var player in Players)
            {
                player.pointHistory[DayIndex] = player.currentDayPoints;

                player.currentDayPoints = 0;
            }
        }

        private void SumUpNight()
        {
            foreach (Phase currentPhase in CurrentDay.Phases)
            {
                if (currentPhase.isInitial)
                {
                    SetRole(currentPhase);
                }
            }

            PointsGiver pointsGiver = new(Days[DayIndex]);

            pointsGiver.GiveNightPoints(ref Players);

            ApllyNightEffects();


        }

        private void ApllyNightEffects()
        {
            var amorPhase = CurrentDay.GetPhase(PhaseEnum.Amor);
            var bodyguardPhase = CurrentDay.GetPhase(PhaseEnum.Bodyguard);
            var dealerPhase = CurrentDay.GetPhase(PhaseEnum.Dealer);
            var mafiaPhase = CurrentDay.GetPhase(PhaseEnum.Mafia);

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
                        PlayerNightDied(player);
                    }
                }
            }
        }

        

        private void SumUpDay()
        {
            PointsGiver pointsGiver = new(Days[DayIndex]);

            pointsGiver.GiveDayPoints(ref Players);

            //who died 

            if(CurrentDay.GetPhase(PhaseEnum.WhoDied).selectedPlayers.Count > 0)
            PlayerDayDied(CurrentDay.GetPhase(PhaseEnum.WhoDied).selectedPlayers[0]);
        }

        private void PlayerNightDied(Player player)
        {
            player.IsAlive = false;

            if (player.IsPaired)
            {
                //Find his pair
                var secondPlayer = Players.Find((p) => p.IsPaired && p != player);

                secondPlayer.IsAlive = false;

                //check if she wasnt protected
            }
            else if (player.Role == Role.Jester)
            {

                PointsGiver pointsGiver = new();

                player.currentDayPoints = pointsGiver.JesterPoints(Players.Count, DayIndex);
            }

            if (ShouldGameEnded())
            {
                PointsGiver pointsGiver = new();

                pointsGiver.GiveWinPoints(ref Players);
            }
        }

        private bool ShouldGameEnded()
        {
            var mafia = GetAlivePlayers(Role.Mafia);

            var city = GetAlivePlayers(new List<Role>() { Role.Citizen, Role.Barman, Role.Agent, Role.Bodyguard });

            var neutral = GetAlivePlayers(new List<Role>() { Role.Renegat, Role.Jester, Role.Amor, Role.Dealer });

            return  mafia.Count == 0 || mafia.Count > city.Count + neutral.Count;
        }

        private void PlayerDayDied(Player player)
        {
            player.IsAlive = false;

            if (player.IsPaired)
            {
                //Find his pair
                var secondPlayer = Players.Find((p) => p.IsPaired && p != player);

                secondPlayer.IsAlive = false;

                //check if she wasnt protected
            }
            else if (player.Role == Role.Jester)
            {
                PointsGiver pointsGiver = new();

                player.currentDayPoints = pointsGiver.JesterPoints(Players.Count, DayIndex);

                //jester wins
                player.currentDayPoints += 5;
                return;
            }

            if (ShouldGameEnded())
            {
                PointsGiver pointsGiver = new();

                pointsGiver.GiveWinPoints(ref Players);

                //show results
            }
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

        private List<Player> GetAlivePlayers(Role role)
        {
            return GetAlivePlayers(new List<Role> { role});
        }

        private List<Player> GetAlivePlayers(List<Role> roles)
        {
            var list = new List<Player>();

            foreach ( var role in roles)
            {
                var results = Players.FindAll((p) => p.IsAlive && p.Role == role);

                list.AddRange(results);
            }

            return list;
        }
    }
}

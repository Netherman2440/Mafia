

namespace Mafia.Models
{
    public class Game
    {
        public event Action<GameResult> OnGameEnd;

        public int DayIndex;

        public Day CurrentDay => Days[DayIndex];

        public List<Day> Days = new();

        public List<Player> Players = new();

        private bool amorCreatedPair = false;

        private bool RenegatKill;

        private int renegatKills;

        private List<Player>[] deathHistory;
        public void StartGame(bool rolesAreSet)
        {
            var firstDay = rolesAreSet ? DayDictionary.GetNextDay(GetLivingRoles(Players), false) : DayDictionary.GetFirstDay();

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

            if (CurrentDay.CurrentPhase.PhaseEnum == PhaseEnum.NightSumUp)
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
                var nextDay = DayDictionary.GetNextDay(GetLivingRoles(Players), amorCreatedPair);

                Days.Add(nextDay);
            }
        }

        private void SavePoints()
        {
            foreach (var player in Players)
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
            var renegatPhase = CurrentDay.GetPhase(PhaseEnum.Renegat);
            if (amorPhase != null && !amorPhase.isInitial && amorPhase.selectedPlayers.Count > 0)    //Amor phase result
            {
                foreach (var player in amorPhase.selectedPlayers)
                {
                    player.IsPaired = true;
                }
                amorCreatedPair = true;
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

            if (RenegatKill && renegatPhase != null)
            {
                foreach(var player in renegatPhase.selectedPlayers)
                {
                    Console.WriteLine($" RENEGAT KILLED {player.Name}!!! WHOSAAH!");
                    renegatKills++;
                    PlayerNightDied(player);
                }
            }
        }



        private void SumUpDay()
        {
            PointsGiver pointsGiver = new(Days[DayIndex]);

            pointsGiver.GiveDayPoints(ref Players);

            //who died 

            if (CurrentDay.GetPhase(PhaseEnum.WhoDied).selectedPlayers.Count > 0)
            {

                PlayerDayDied(CurrentDay.GetPhase(PhaseEnum.WhoDied).selectedPlayers[0]);
            }

            //renegat change effect

            if (!RenegatKill && RenegatKilled(CurrentDay.GetPhase(PhaseEnum.Renegat), CurrentDay.GetPhase(PhaseEnum.WhoDied)))
            {
                RenegatKill = true;
            }
            else
            {
                RenegatKill = false;
            }

            ClearDealerEffects();

        }

        private void ClearDealerEffects()
        {
            foreach(var player in Players)
            {
                player.CantVote = false;
            }
        }

        private void PlayerNightDied(Player player)
        {
            player.IsAlive = false;
            SaveKill(player, false);

            if (player.IsPaired)
            {
                //Find his pair
                var secondPlayer = Players.Find((p) => p.IsPaired && p != player && p.IsAlive);

                if (secondPlayer != null)
                {
                    secondPlayer.IsAlive = false;
                    SaveKill(secondPlayer, false);
                }

            }
            else if (player.Role == Role.Jester)
            {

                PointsGiver pointsGiver = new();

                player.currentDayPoints = pointsGiver.JesterPoints(Players.Count, DayIndex, false);
            }

            if (ShouldGameEnded(out string winners))
            {
                EndGame(winners);
            }
        }



        private void PlayerDayDied(Player player)
        {
            player.IsAlive = false;

            SaveKill(player, true);


                if (player.IsPaired)
                {
                    //Find his pair
                    var secondPlayer = Players.Find((p) => p.IsPaired && p != player);

                    if (secondPlayer != null)
                    {
                        secondPlayer.IsAlive = false;
                        SaveKill(player, true);
                    }

                }
                else if (player.Role == Role.Jester)
                {
                    PointsGiver pointsGiver = new();

                    player.currentDayPoints = pointsGiver.JesterPoints(Players.Count, DayIndex, true);


                    EndGame("Jester");
                    return;
                }

            if (ShouldGameEnded(out string winners))
            {
                EndGame(winners);
            }
        }

        private void SaveKill(Player player, bool dayKill)  //if 
        {
            if (deathHistory == null)
            {
                deathHistory = new List<Player>[Players.Count * 2];
            }

            int dayPhase = dayKill ? 1 : 0;

            int deathIndex = DayIndex * 2 + dayPhase;

            if (deathHistory[deathIndex] == null)
            {
                deathHistory[deathIndex] = new List<Player>();
            }

            deathHistory[deathIndex].Add(player);
        }

        private void EndGame(string winners)
        {
            PointsGiver pointsGiver = new();

            pointsGiver.GiveWinPoints(ref Players);

            SavePoints();

            foreach (var player in Players)
            {
                player.Score = player.pointHistory.Sum();
            }

            GameResult gameResult = new(winners, deathHistory, Players, DayIndex + 1, renegatKills);

            OnGameEnd?.Invoke(gameResult);
        }
        private bool ShouldGameEnded(out string winners)
        {
            var mafia = GetAlivePlayers(Role.Mafia);

            var city = GetAlivePlayers(new List<Role>() { Role.Citizen, Role.Barman, Role.Agent, Role.Bodyguard });

            var neutral = GetAlivePlayers(new List<Role>() { Role.Renegat, Role.Jester, Role.Amor, Role.Dealer });


            winners = string.Empty;

            if (mafia.Count == 0)
            {
                winners = "City";
                return true;
            }

            if (mafia.Count > city.Count + neutral.Count)
            {
                winners = "Mafia";
                return true;
            }
            return false;

        }
        private void SetRole(Phase phase)
        {
            foreach (var player in phase.selectedPlayers)
            {
                player.Role = phase.Role;
            }
        }

        private bool BarmanPickedMafia(List<Player> selectedPlayers)    //mafia is drunk, have to show it in mafia phase
        {
            bool barmanPickedMafia = false;

            foreach (var player in selectedPlayers)
            {
                if (player.Role == Role.Mafia)
                    barmanPickedMafia = true;
            }

            return barmanPickedMafia;
        }

        private bool BodyguardProtected(Phase bodyguardPhase, List<Player> currentSelection)
        {
            bool bodyguardProtected = false;
            if (bodyguardPhase == null) return bodyguardProtected;

            foreach (var player in currentSelection)
            {
                if (bodyguardPhase.selectedPlayers.Contains(player))
                {
                    bodyguardProtected = true;
                }
            }

            return bodyguardProtected;
        }

        private List<Role> GetLivingRoles(List<Player> players)
        {
            return players.Where(player => player.IsAlive)
                            .Select(player => player.Role)
                          .Distinct()
                          .ToList();
        }

        private List<Player> GetAlivePlayers(Role role)
        {
            return GetAlivePlayers(new List<Role> { role });
        }

        private List<Player> GetAlivePlayers(List<Role> roles)
        {
            var list = new List<Player>();

            foreach (var role in roles)
            {
                var results = Players.FindAll((p) => p.IsAlive && p.Role == role);

                list.AddRange(results);
            }

            return list;
        }

        private bool RenegatKilled(Phase renegatPhase, Phase whoDiedPhase)    //renegat points
        {
            //renegat will get points even after death

            if (renegatPhase == null) return false;

            foreach (var player in renegatPhase.selectedPlayers)
            {
                if (whoDiedPhase.selectedPlayers.Contains(player))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

using Mafia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppMafia
{
    public class GameSimulator
    {
        public event Action<GameResult> OnGameEnded;
        bool stopGame = false;
        private Game game;
        private Random random;
        private List<Player> players;
        public GameSimulator()
        {
            Random seedGenerator = new Random();

            int seed = seedGenerator.Next();
            random = new Random(seed);

            Console.WriteLine($"Game simulator seed: {seed}");
        }

        public void Simulate()
        {
            Console.WriteLine("Start simulating...");

            game = new Game();
            game.OnGameEnd += (p) => GameEnd(p);
            game.Players = PlayerDictionary.TestPlayersWithRole();

            game.StartGame(true);

            while (!stopGame)
            {
                List<Player> players = Select(game.CurrentDay.CurrentPhase);

                game.SelectPlayers(players);

                game.NextPhase();
            }
        }

        private List<Player> Select(Phase currentPhase)
        {

            players = new List<Player>(game.Players);

            players.RemoveAll((p) => !p.IsAlive);

            switch (currentPhase.PhaseEnum)
            {
                case PhaseEnum.Amor:
                    return GetRandom(players, 2);

                case PhaseEnum.Renegat:
                    return GetRandomExceptRole(players, 1, Role.Renegat);
                case PhaseEnum.Barman:
                    return GetRandomExceptRole(players, 1, Role.Barman);

                case PhaseEnum.Bodyguard:
                    return GetRandom(players, 1);

                case PhaseEnum.Dealer:
                    return GetRandom(players, 1);

                case PhaseEnum.Mafia:

                    if (BarmanPickedMafia(game.CurrentDay.GetPhase(PhaseEnum.Barman)))
                    {
                        return GetRandom(players, 1);
                    }
                    else
                    {
                        return GetRandomExceptRole(players, 1, Role.Mafia);
                    }

                case PhaseEnum.Agent:
                    return GetRandomExceptRole(players, 1, Role.Agent);

                case PhaseEnum.NightSumUp:
                    break;
                case PhaseEnum.SecondVoting:
                    return GetRandom(players, 2);
                case PhaseEnum.WhoDied:
                    var secondRoundVote = game.CurrentDay.GetPhase(PhaseEnum.SecondVoting).selectedPlayers;
                    return GetRandom(secondRoundVote, 1);
                case PhaseEnum.WhoVoted:

                    return SimulateVoting(game.CurrentDay.GetPhase(PhaseEnum.WhoDied));


                case PhaseEnum.DaySumUp:
                    break;
                default:
                    return new();
            }
            return new();
        }

        private List<Player> SimulateVoting(Phase whoDiedPhase)
        {
           if(whoDiedPhase == null) return new();

            List<Player> voting = new();

            if (whoDiedPhase.selectedPlayers[0].Role == Role.Mafia)
            {
                voting = GetRandomExceptRoles(players, players.Count / 3 + 1, new List<Role> { Role.Mafia });
            }
            else
            {
                voting = GetRandom(players, players.Count / 3 + 1);
            }

            voting.RemoveAll(x => x.CantVote);

            return voting;
        }



        private List<Player> GetRandomExceptRole(List<Player> players, int amount, Role role)
        {
            return GetRandomExceptRoles(players, amount, new List<Role> { role });
        }

        private List<Player> GetRandomExceptRoles(List<Player> players, int amount, List<Role> roles)
        {
            List<Player> copyOfPlayers = new List<Player>(players);
            copyOfPlayers.RemoveAll((p) =>roles.Contains(p.Role));

            return GetRandom(copyOfPlayers, amount);
        }


        private List<Player> GetRandom(List<Player> players, int amount)
        {
            if (amount > players.Count)
            {
                throw new ArgumentException("x cannot be greater than the size of the players list.");
            }

            List<Player> copyOfPlayers = new List<Player>(players);

            Random random = new Random();
            copyOfPlayers = copyOfPlayers.OrderBy(p => random.Next()).ToList();

            return copyOfPlayers.Take(amount).ToList();
        }

        private void GameEnd(GameResult result)
        {
            stopGame = true;

            game.OnGameEnd -= (p) => GameEnd(p);
            OnGameEnded?.Invoke(result);

            Console.WriteLine("Game ended!");

            Console.WriteLine($"Winners: {result.winners.ToUpper()}");

            Console.WriteLine("Another winners:");

            Console.WriteLine($"Dealer: {result.dealerWins}");
            Console.WriteLine($"Renegat: {result.renegatWins}");
            Console.WriteLine($"Pair: {result.pairWins}");

            Console.WriteLine($"Game took {result.days} days!");
            Console.WriteLine("Stats:");

            var sortedPlayers = result.players.OrderBy(player => player.Score).Reverse().ToList();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                Console.WriteLine($"{i}. {sortedPlayers[i].Name} - {sortedPlayers[i].Score} points. Is Alive? {sortedPlayers[i].IsAlive}. Was paired? {sortedPlayers[i].IsPaired}.");
            }

            Console.WriteLine($"Renegat kills: {result.renegatKills}");

        }

        private bool BarmanPickedMafia(Phase barmanPhase)    //mafia is drunk, have to show it in mafia phase
        {
            bool barmanPickedMafia = false;

            if (barmanPhase == null) return false;

            foreach (var player in barmanPhase.selectedPlayers)
            {
                if (player.Role == Role.Mafia)
                    barmanPickedMafia = true;
            }

            return barmanPickedMafia;
        }
    }
}

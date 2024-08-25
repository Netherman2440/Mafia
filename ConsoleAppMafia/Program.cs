// See https://aka.ms/new-console-template for more information


using ConsoleAppMafia;
using Mafia.Models;

Console.WriteLine("Hello, Mafia World!");

int gameCount = 1000;
List<GameResult> gameResults = new List<GameResult>();

List<Player> playerList = PlayerDictionary.TestPlayersWithRole();

for (int i = 0; i < gameCount; i++)
{
    NewGame();
}

Console.WriteLine("SIMULATION ENDED");
Console.WriteLine("");
Console.WriteLine($"    RESULTS AFTER {gameCount} GAMES:");

Console.WriteLine($"    City wins:{gameResults.FindAll(r => r.winners.ToLower() == "city").Count}");
Console.WriteLine($"    Mafia wins:{gameResults.FindAll(r => r.winners.ToLower() == "mafia").Count}");
Console.WriteLine($"    Jester wins:{gameResults.FindAll(r => r.winners.ToLower() == "jester").Count}");
Console.WriteLine($"    Dealer wins:{gameResults.FindAll(r => r.dealerWins).Count}");
Console.WriteLine($"    Renegat wins:{gameResults.FindAll(r => r.renegatWins).Count}");
Console.WriteLine($"    Pair wins:{gameResults.FindAll(r => r.pairWins).Count}");

Console.WriteLine($" Total RenegatKills: {gameResults.Sum(r => r.renegatKills)}");
Console.WriteLine($" Average RenegatKills: { (float)gameResults.Sum(r => r.renegatKills) / gameCount}");

var sortedPlayers = playerList.OrderBy(player => player.Score).Reverse().ToList();

for (int i = 0; i < sortedPlayers.Count; i++)
{
    Console.WriteLine($"    {i}. {sortedPlayers[i].Name} - Total Score: {sortedPlayers[i].Score} points. Average score: {(float)sortedPlayers[i].Score / gameCount}. ");
}

void NewGame()
{
    GameSimulator gameSimulator = new GameSimulator();

    gameSimulator.OnGameEnded += p => CacheResult(p);

    gameSimulator.Simulate();

}

void CacheResult(GameResult result)
{
    foreach (var player in result.players)
    {
        playerList.Find(p => p.Name == player.Name).Score += player.Score;
    }

    gameResults.Add(result);
}
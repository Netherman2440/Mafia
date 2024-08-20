// See https://aka.ms/new-console-template for more information


using Mafia.Models;

Game game = new Game();
game.Players = PlayerDictionary.TestPlayersWithRole(); 

game.StartGame(true);

Console.WriteLine($"Player count:{game.Players.Count}");
foreach(var player in game.Players)
{
    Console.WriteLine(player.Name);
}
Console.WriteLine("");

Console.WriteLine("Hello, Mafia World!");

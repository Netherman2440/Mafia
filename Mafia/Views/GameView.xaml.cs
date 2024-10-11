using Mafia.ViewModels;

namespace Mafia.Views;

public partial class GameView : ContentView
{
	public GameView()
	{
		InitializeComponent();

		BindingContext = new GameViewModel();
	}


}
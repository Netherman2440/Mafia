
using CommunityToolkit.Mvvm.Input;
using Mafia.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Mafia.ViewModels
{
    internal class GameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddPlayerCommand { get; }
        public ICommand NextPhaseCommand { get; }
        public ICommand PrevPhaseCommand { get; }

        private ObservableCollection<PlayerViewModel> players;
        public ObservableCollection<PlayerViewModel> Players
        {
            get => players;
            set
            {
                players = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Players)));
            }
        }

        public string PhaseLabel
        {
            get { return this.phaseLabel; }

            set
            {
                this.phaseLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhaseLabel)));
            }
        }

        private string phaseLabel;

        private string newPlayerName;
        public string NewPlayerName
        {
            get => newPlayerName;
            set
            {
                if (newPlayerName != value)
                {
                    newPlayerName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewPlayerName)));
                }
            }
        }


        public Game game;

        public GameViewModel()
        {
            game = new Game();

            game.Players = PlayerDictionary.TestPlayersWithRole();

            Players = new ObservableCollection<PlayerViewModel>();

            foreach (var player in game.Players)
            {
                Players.Add(new PlayerViewModel(player));
            }


            game.StartGame(true);

            PhaseLabel = game.CurrentDay.CurrentPhase.Description;

            AddPlayerCommand = new RelayCommand(AddPlayer);
            NextPhaseCommand = new RelayCommand(NextPhase);
            PrevPhaseCommand = new RelayCommand(PrevPhase);
        }

        private void PrevPhase()
        {
            game.PrevPhase();

            PhaseLabel = game.CurrentDay.CurrentPhase.Description;

            RefreshSelections();
        }

        public void NextPhase()
        {

            var selected = Players.Where(x => x.IsSelected).Select(p => p.Player).ToList();

            game.SelectPlayers(selected);

            game.NextPhase();

            PhaseLabel = game.CurrentDay.CurrentPhase.Description;

            RefreshSelections();
        }

        private void RefreshSelections()
        {
            foreach (var playerViewModel in Players)
            {
                playerViewModel.IsSelected = game.CurrentDay.CurrentPhase.selectedPlayers.Contains(playerViewModel.Player);
            }
        }

        private void AddPlayer()
        {
            if (!string.IsNullOrWhiteSpace(NewPlayerName))
            {
                var player = new Player { Name = NewPlayerName };
                game.Players.Add(player);
                Players.Add(new PlayerViewModel(player));
                NewPlayerName = string.Empty;
            }
        }

    }
}

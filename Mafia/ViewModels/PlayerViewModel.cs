using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mafia.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace Mafia.ViewModels
{
    internal class PlayerViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ToggleSelectionCommand { get; }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }

                BackgroundColor = IsSelected ? Color.FromRgb(37, 216, 181) : Color.FromRgb(26, 45, 93);
            }
        }


        private Color backgroundColor;
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    OnPropertyChanged(nameof(BackgroundColor));
                }
            }
        }

        private Player player;
        public Player Player
        {
            get => player;
             set
            {
                if (player != value)
                {
                    player = value;
                    OnPropertyChanged(nameof(Player));
                }
            }
        }

        public PlayerViewModel(Player player)
        {
            Player = player;

            Player.PropertyChanged += Player_PropertyChanged;

            ToggleSelectionCommand = new RelayCommand(ToggleSelection);

            BackgroundColor = Color.FromRgb(26, 45, 93);
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Powiadomienie o zmianach w PlayerViewModel
            OnPropertyChanged(e.PropertyName);

           OnPropertyChanged(nameof(Player));
        }

        private void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

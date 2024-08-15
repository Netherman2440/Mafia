using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mafia
{
    public class MainPageViewModel
    {
        public ObservableCollection<string> Buttons { get; set; }
        public ICommand ButtonCommand { get; }

        public MainPageViewModel()
        {
            // Example data - this can be populated dynamically
            Buttons = new ObservableCollection<string>
            {
                "1", "2", "3", "4", "5", "6", "7", "8",
                "1", "2", "3", "4", "5", "6", "7", "8",
                "1", "2", "3", "4", "5", "6", "7", "8",
                "1", "2", "3", "4", "5", "6", "7", "8",
                "1", "2", "3", "4", "5", "6", "7", "8"
            };

        }

        public void LoadScenario(int scenario)
        {
            Buttons.Clear();

            if (scenario == 1)
            {
                Buttons.Add("1");
                Buttons.Add("2");
                Buttons.Add("3");
                Buttons.Add("4");
                Buttons.Add("5");
            }
            else if (scenario == 2)
            {
                Buttons.Add("Button A");
                Buttons.Add("Button B");
                Buttons.Add("Button C");
                Buttons.Add("Button D");
                Buttons.Add("Button E");
                Buttons.Add("Button F");
                Buttons.Add("Button G");
                Buttons.Add("Button H");
            }
        }
    }
}

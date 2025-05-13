using AdvancedChatFeatures.Helpers;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public class MainState : UIState
    {

        // CONSTRUCTOR
        public MainState()
        {
            // Initialize the UI elements
            var configIcon = new DrawConfigIcon(Ass.ButtonModConfig);
            Append(configIcon);
        }
    }
}
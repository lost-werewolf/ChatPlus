using AdvancedChatFeatures.Helpers;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.DrawConfig
{
    public class DrawConfigState : UIState
    {
        public DrawConfigState()
        {
            // Initialize the UI elements
            DrawConfigIcon configIcon = new(Ass.ConfigButton);
            Append(configIcon);
        }
    }
}
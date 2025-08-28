using ChatPlus.Core.Features.Scrollbar.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar;

public class ChatScrollState : UIState
{
    public ChatScrollbarElement chatScrollbar;
    public ChatScrollList chatScrollList;

    public ChatScrollState()
    {
        // Initialize chat scroll UI elements
        chatScrollList = new ChatScrollList();
        chatScrollbar = new ChatScrollbarElement();
        chatScrollList.SetScrollbar(chatScrollbar);
        Append(chatScrollList);
        Append(chatScrollbar);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        // Draw a subtle background behind the chat text area for readability
        ScrollHelper.DrawChatMonitorBackground(spriteBatch);
    }
}

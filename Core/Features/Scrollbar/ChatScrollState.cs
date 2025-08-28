using ChatPlus.Core.Features.Scrollbar.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar
{
    public class ChatScrollState : UIState
    {
        public ChatScrollbarElement chatScrollbar;
        public ChatScrollList chatScrollList;

        public ChatScrollState()
        {
            chatScrollList = [];
            chatScrollbar = new();
            chatScrollList.SetScrollbar(chatScrollbar);

            Append(chatScrollList);
            Append(chatScrollbar);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            ScrollHelper.DrawChatMonitorBackground(sb);
            //ScrollHelper.DrawChatScrollList(sb, chatScrollList);
        }
    }
}

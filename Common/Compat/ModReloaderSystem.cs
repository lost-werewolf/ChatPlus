using System;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace ChatPlus.Common.Compat
{
    public class ModReloaderSystem : ModSystem
    {
        public static float ChatOffsetX = 0;
        public static float ChatOffsetY = 0;
        public static bool Found = false;
        
        public override void Load()
        {
            IL_Main.DrawPlayerChat += InjectChatOffset;
            IL_RemadeChatMonitor.DrawChat += InjectChatOffset;
        }

        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("ModReloader", out var modReloader))
            {
                Found = true;
            }
        }

        public override void Unload()
        {
            IL_Main.DrawPlayerChat -= InjectChatOffset;
            IL_RemadeChatMonitor.DrawChat -= InjectChatOffset;
        }

        private void InjectChatOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Find 3 calls to SpriteBatch.Draw, which is where the chat text is drawn.
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchConvR4(),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate((Vector2 pos) => pos + new Vector2(ChatOffsetX, ChatOffsetY));
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}
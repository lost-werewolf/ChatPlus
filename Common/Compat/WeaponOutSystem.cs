using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo;
using Terraria.GameContent;
using Terraria.UI;

namespace ChatPlus.Common.Compat
{
    internal class WeaponOutSystem : ModSystem
    {
        private Hook weaponLayerHook;

        private delegate void DrawDelegate(object self, ref PlayerDrawSet drawInfo);

        public override void Load()
        {
            if (ModLoader.TryGetMod("WeaponOutLite", out Mod mod))
            {
                HookWeaponLayer(mod);
            }
        }

        public override void Unload()
        {
            weaponLayerHook?.Dispose();
            weaponLayerHook = null;
        }

        private void HookWeaponLayer(Mod mod)
        {
            var type = mod.Code.GetType("WeaponOutLite.Common.WeaponOutItemHeldLayer");
            if (type == null)
            {
                Log.Error("WeaponOutItemHeldLayer not found.");
                return;
            }

            var method = type.GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                Log.Error("WeaponOutItemHeldLayer.Draw not found.");
                return;
            }

            weaponLayerHook = new Hook(
                    method,
                    (DrawDelegate orig, object self, ref PlayerDrawSet drawInfo) =>
                    {
                    orig(self, ref drawInfo);

                    if (!PreviewFullBrightPlayer.ForceFullBrightOnce) return;

                    var held = drawInfo.drawPlayer.HeldItem;
                    if (held != null && !held.IsAir && drawInfo.DrawDataCache != null)
                    {
                        for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
                        {
                            var weapon = drawInfo.DrawDataCache[i];
                            if (weapon.texture == TextureAssets.Item[held.type].Value)
                            {
                                    // Set weapon properties
                                    weapon.scale *= 1.0f;
                                    weapon.color = Color.White;

                                    Vector2 pos = new(0, 0);
                                    if (PlayerInfoState.Active)
                                    {
                                        pos = new(366, 321);
                                        //pos = Main.MouseScreen + new Vector2(130, 126);
                                        Log.Info(pos);
                                    }
                                    else
                                    {
                                        // Desired position at mouse with panel offset
                                        pos = Main.MouseScreen + new Vector2(130, 126);

                                        // Clamp inside panel bounds
                                        pos.X = Math.Clamp(pos.X, 109, 852);
                                        pos.Y = Math.Clamp(pos.Y, 105, 753);
                                    }

                                    // Set position
                                    weapon.position = pos;
                                    drawInfo.DrawDataCache[i] = weapon;
                                }
                            }
                        }
                    }
                );
        }
    }
}

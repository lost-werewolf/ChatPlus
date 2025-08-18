using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdvancedChatFeatures.Helpers;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Hooks
{
    // No-op ChatImprover init so it won’t register its hooks
    public class SkipChatImprover : ModSystem
    {
        private readonly List<Hook> _hooks = new();

        public override void Load()
        {
            if (!ModLoader.TryGetMod("ChatImprover", out Mod ci))
            {
                Log.Info("SkipChatImprover: ChatImprover not found; nothing to skip.");
                return;
            }

            var modType = ci.GetType();
            Log.Info($"SkipChatImprover: found ChatImprover type {modType.FullName}; installing detours.");

            try
            {
                // 1) Skip ChatImprover.Mod.Load()
                MethodInfo miLoadMod = modType.GetMethod("Load", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (miLoadMod != null)
                {
                    Log.Info("SkipChatImprover: detouring ChatImprover.Mod.Load()");
                    _hooks.Add(new Hook(miLoadMod, (Action<Action<object>, object>)((orig, self) =>
                    {
                        Log.Info("SkipChatImprover: Skipping ChatImprover.Mod.Load()");
                        // Do NOT call orig(self)
                    })));
                }

                // 2) Skip all ModSystem.Load() inside ChatImprover assembly
                var asm = ci.Code; // ChatImprover assembly
                if (asm != null)
                {
                    foreach (var type in asm.GetTypes().Where(t => typeof(ModSystem).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        MethodInfo miSysLoad = type.GetMethod("Load", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        if (miSysLoad != null && miSysLoad.DeclaringType == type)
                        {
                            Log.Info($"SkipChatImprover: detouring {type.FullName}.Load()");
                            _hooks.Add(new Hook(miSysLoad, (Action<Action<object>, object>)((orig, self) =>
                            {
                                Log.Info($"SkipChatImprover: Skipping {type.FullName}.Load()");
                                // Do NOT call orig(self)
                            })));
                        }

                        // Optional: also skip PostSetupContent to prevent late registrations
                        MethodInfo miSysPSC = type.GetMethod("PostSetupContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                        if (miSysPSC != null && miSysPSC.DeclaringType == type)
                        {
                            Log.Info($"SkipChatImprover: detouring {type.FullName}.PostSetupContent()");
                            _hooks.Add(new Hook(miSysPSC, (Action<Action<object>, object>)((orig, self) =>
                            {
                                Log.Info($"SkipChatImprover: Skipping {type.FullName}.PostSetupContent()");
                            })));
                        }
                    }
                }
                else
                {
                    Log.Warn("SkipChatImprover: ChatImprover.Code assembly is null; cannot scan systems.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"SkipChatImprover: Failed to install detours: {ex}");
            }
        }

        public override void Unload()
        {
            foreach (var h in _hooks)
                h?.Dispose();
            _hooks.Clear();
        }
    }
}
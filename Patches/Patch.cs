using HarmonyLib;
using SpeedrunManager.UI;

namespace SpeedrunManager.Patches
{
    [HarmonyPatch(typeof(Hud), "Update")]
    public static class Hud_Update_Patch
    {
        static void Postfix(Hud __instance)
        {
            if (!SpeedrunConfigPanel.IsCreated())
                SpeedrunConfigPanel.Create();
            
            if (!SpeedrunTimer.IsCreated())
                SpeedrunTimer.Create(__instance);
            
            SpeedrunTimer.Update();
        }
    }
    
    [HarmonyPatch(typeof(Hud), "Awake")]
    public static class Hud_Awake_Patch
    {
        static void Postfix(Hud __instance)
        {
            __instance.m_effectsPerRow = ConfigurationFile.effectsPerRow.Value;
        }
    }
}
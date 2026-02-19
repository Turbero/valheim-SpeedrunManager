using HarmonyLib;

namespace SpeedrunManager
{
    [HarmonyPatch(typeof(Hud), "Update")]
    public static class Hud_Update_Patch
    {
        static void Postfix(Hud __instance)
        {
            if (!SpeedrunTimer.IsCreated())
                SpeedrunTimer.Create(__instance);
            
            SpeedrunTimer.Update();
        }
    }
}
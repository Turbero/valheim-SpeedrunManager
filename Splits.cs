using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SpeedrunManager
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetKnownTexts))]
    public class FixCompendium
    {
        public static void Postfix(ref List<KeyValuePair<string, string>> __result)
        {
            __result = __result.Where(p => !p.Key.StartsWith(SpeedrunManager.GUID)).ToList();
        }
    }
    
    [HarmonyPatch(typeof(Character), "OnDeath")]
    public class RegisterBossDefeatPatch
    {
        // This will be executed in the player that hits last when the boss dies
        public static void Postfix(Character __instance)
        {
            if (__instance != null && __instance.IsBoss())
            {
                string bossName = __instance.name.Replace("(Clone)", "");
                string timerValue = SpeedrunTimer._text.text;
                setupBossSplitTime(bossName, timerValue, false);
            }
        }

        public static void setupBossSplitTime(string bossName, string timerValue, bool overwrite)
        {
            string worldName = ZNet.instance?.GetWorldName();
            string splitKey = SpeedrunManager.GUID+"_"+worldName+"_"+bossName;

            var dicKnownTexts = (Dictionary<string, string>)ModUtils.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
            if (!dicKnownTexts.ContainsKey(splitKey))
            {
                dicKnownTexts.Add(splitKey, timerValue);
            } else if (overwrite)
            {
                dicKnownTexts.Remove(splitKey);
                dicKnownTexts.Add(splitKey, timerValue);
            }
        }
    }
}
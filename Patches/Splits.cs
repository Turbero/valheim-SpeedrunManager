using System.Collections.Generic;
using HarmonyLib;
using SpeedrunManager.UI;

namespace SpeedrunManager.Patches
{
    [HarmonyPatch(typeof(Character), "OnDeath")]
    public class RegisterBossDefeatPatch
    {
        // This will be executed in the player that hits last when the boss dies
        public static void Postfix(Character __instance)
        {
            if (__instance == null) return;
            if (__instance.IsBoss())
            {
                string bossName = __instance.name.Replace("(Clone)", "");
                string timerValue = SpeedrunTimer._text.text;
                setupBossSplitTime(bossName, timerValue, false);
                
                Logger.Log("Gonna parse "+bossName+"...");
                BossNameEnum bossNameEnum = ModUtils.parseBossName(bossName);
                Split split = new Split(bossNameEnum, timerValue);
                SpeedrunTimer.AddSplitTimer(split);
            } else if (__instance.IsPlayer() && ConfigurationFile.speedrunType.Value == SpeedrunType.Permadeath)
            {
                SpeedrunTimer.StopTimer();
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
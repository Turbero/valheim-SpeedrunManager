using System.Collections.Generic;
using System.Reflection;

namespace SpeedrunManager
{
    public class ModStatsUtils
    {
        public static Dictionary<string, string> GetKnownTexts()
        {
            if (Player.m_localPlayer == null)
                return new Dictionary<string, string>();
            
            return (Dictionary<string, string>)ModUtils.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
        }

        public static Dictionary<PlayerStatType, float> GetStats()
        {
            if (Game.instance == null)
                return new Dictionary<PlayerStatType, float>();

            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);

            var profile = (PlayerProfile)field?.GetValue(Game.instance);
            return profile?.m_playerStats.m_stats;
        }

        public static string GetSpeedrunKnownTextKey(string name)
        {
            var worldName = ZNet.instance?.GetWorldName();
            return SpeedrunManager.GUID + "_" + worldName + "_" + name;
        }
        
        public static string GetSpeedrunKnownTextValue(string name)
        {
            return GetKnownTexts().GetValueOrDefaultPiktiv(GetSpeedrunKnownTextKey(name), null);
        }
        
        public static void SetSpeedrunKnownTextKeyValue(string name, string value)
        {
            var dicKnownTexts = GetKnownTexts();
            var keyName = GetSpeedrunKnownTextKey(name);
            dicKnownTexts.Remove(keyName);
            dicKnownTexts.Add(keyName, value);
        }
    }
}
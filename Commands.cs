using System;
using System.Collections.Generic;
using SpeedrunManager.Patches;
using SpeedrunManager.UI;

namespace SpeedrunManager
{
    public class SplitsCommands {
        public static void RegisterConsoleCommand()
        {
            new Terminal.ConsoleCommand("speedrun_set_split", "[boss_prefab_id] [timer_value]", args =>
            {
                if (args.Args.Length < 3)
                {
                    args.Context.AddString("Usage: speedrun_set_split <boss_name> <timer_value>");
                    return;
                }
                
                //Validate boss name first
                bool isBossPrefabId = Enum.TryParse(args.Args[1], out BossNameEnum bossNameEnum);
                if (isBossPrefabId)
                {
                    RegisterBossDefeatPatch.setupBossSplitTime(args.Args[1], args.Args[2], true);
                    Split split = new Split(bossNameEnum, args.Args[2]);
                    SpeedrunTimer.AddSplitTimer(split);
                }
            });
            new Terminal.ConsoleCommand("speedrun_reset", "", args =>
            {
                if (args.Args.Length < 1)
                {
                    args.Context.AddString("Usage: speedrun_reset");
                    return;
                }

                if (Player.m_localPlayer == null)
                    return;

                if (ZNet.instance == null)
                    return;

                var dicKnownTexts = (Dictionary<string, string>)ModUtils.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
                List<string> keysToDelete = new List<string>();
                string worldName = ZNet.instance.GetWorldName();
                
                foreach (var key in dicKnownTexts.Keys)
                    if (key.StartsWith(SpeedrunManager.GUID + "_" + worldName))
                        keysToDelete.Add(key);

                foreach (var key in keysToDelete)
                    dicKnownTexts.Remove(key);
            });
        }
    }
}
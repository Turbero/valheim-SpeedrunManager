namespace SpeedrunManager
{
    public class SplitsCommands {
        public static void RegisterConsoleCommand()
        {
            new Terminal.ConsoleCommand("speedrun_set_split", "[boss_name] [timer_value]", args =>
            {
                if (args.Args.Length < 3)
                {
                    args.Context.AddString("Usage: speedrun_set_split <boss_name> <timer_value>");
                    return;
                }
                
                //TODO Validate boss name
                
                RegisterBossDefeatPatch.setupBossSplitTime(args.Args[1], args.Args[2], true);
            });
        }
    }
}
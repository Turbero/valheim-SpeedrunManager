using System;

namespace SpeedrunManager.UI
{
    public class Split
    {
        public BossNameEnum BossName { get; }
        public string TimerValue { get; }

        public Split(BossNameEnum bossName, string timerValue)
        {
            BossName = bossName;
            TimerValue = timerValue;
        }
    }
}
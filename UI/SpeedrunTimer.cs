using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public static class SpeedrunTimer
    {
        private static RectTransform rect;
        public static TextMeshProUGUI _text;
        private static GameObject goSplitsTimers;
        
        // Splits
        private static readonly List<Split> splits = new List<Split>();
        private static bool splitsLoaded = false;

        // Hybrid timer variables
        private static double _lastRealtime;
        private static double _displayedTime;
        private static float _lastStatTime;

        public static void Create(Hud hud)
        {
            if (_text != null)
                return;

            GameObject textObj = new GameObject("SpeedrunTimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(hud.transform, false);

            rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = ConfigurationFile.positionTimer.Value;
            rect.sizeDelta = new Vector2(500, 80);

            _text = textObj.GetComponent<TextMeshProUGUI>();
            _text.fontSize = ConfigurationFile.fontSizeTimer.Value;
            _text.alignment = TextAlignmentOptions.Left;
            _text.font = ModUtils.getFontAsset("Valheim-Norse");

            UpdateTimerUI();
            
            splits.Clear();
        }

        public static void UpdateTimerUI()
        {
            rect.anchoredPosition = ConfigurationFile.positionTimer.Value;
            _text.fontSize = ConfigurationFile.fontSizeTimer.Value;
            _text.color = ConfigurationFile.colorTimer.Value;
            
            Color colorToUse = HasPlayerDeadAlreadyInPermadeath()
                ? ConfigurationFile.colorTimerAfterDyingInPermadeath.Value
                : ConfigurationFile.colorTimer.Value;
            
            _text.color = colorToUse;
            _text.outlineColor = new Color32(
                (byte)(colorToUse.r * 255f),
                (byte)(colorToUse.g * 255f),
                (byte)(colorToUse.b * 255f),
                255);
            _text.outlineWidth = ConfigurationFile.colorWidthTimer.Value;
            
            if (goSplitsTimers != null)
                goSplitsTimers.SetActive(ConfigurationFile.showSplits.Value);
        }

        private static void LoadSplits()
        {
            Logger.Log("Loading splits...");
            
            string result = ModStatsUtils.GetSpeedrunKnownTextValue("Eikthyr");
            if (result != null)
            {
                Logger.Log("Found split for Eikthyr...");
                splits.Add(new Split(BossNameEnum.Eikthyr, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("gd_king");
            if (result != null)
            {
                Logger.Log("Found split for The Elder...");
                splits.Add(new Split(BossNameEnum.gd_king, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("Bonemass");
            if (result != null)
            {
                Logger.Log("Found split for Bonemass...");
                splits.Add(new Split(BossNameEnum.Bonemass, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("Dragon");
            if (result != null)
            {
                Logger.Log("Found split for Moder...");
                splits.Add(new Split(BossNameEnum.Dragon, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("GoblinKing");
            if (result != null)
            {
                Logger.Log("Found split for Yagluth...");
                splits.Add(new Split(BossNameEnum.GoblinKing, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("SeekerQueen");
            if (result != null)
            {
                Logger.Log("Found split for The Queen...");
                splits.Add(new Split(BossNameEnum.SeekerQueen, result));
            }
            result = ModStatsUtils.GetSpeedrunKnownTextValue("Fader");
            if (result != null)
            {
                Logger.Log("Found split for Fader...");
                splits.Add(new Split(BossNameEnum.Fader, result));
            }
            
            DrawSplits();
        }
        
        private static void DrawSplits()
        {
            Logger.Log("Drawing splits...");
            goSplitsTimers = GameObject.Find("SpeedrunTimerSplits") ?? new GameObject("SpeedrunTimerSplits", typeof(RectTransform));
            goSplitsTimers.transform.SetParent(Hud.instance.transform, false);
            goSplitsTimers.SetActive(ConfigurationFile.showSplits.Value);

            int width = (int)ConfigurationFile.positionSplits.Value.x;
            int height = (int)ConfigurationFile.positionSplits.Value.y;
            int count = 0;
            foreach (var split in splits)
            {
                string translatedBossName = Localization.instance.Localize(split.BossName.GetTranslationKey());
                Logger.Log("Drawing "+translatedBossName+" split");
                //Split icon
                string goIconName = "Split_" + translatedBossName + "_Icon";
                GameObject splitIconObj = GameObject.Find(goIconName) ?? new GameObject(goIconName, typeof(RectTransform), typeof(Image));
                splitIconObj.transform.SetParent(goSplitsTimers.transform);
                RectTransform rectIconObj = splitIconObj.GetComponent<RectTransform>();
                rectIconObj.anchorMin = new Vector2(0, 1);
                rectIconObj.anchorMax = new Vector2(0, 1);
                rectIconObj.pivot = new Vector2(0, 1);
                rectIconObj.anchoredPosition = new Vector2(width, height);
                rectIconObj.sizeDelta = new Vector2(32, 32);
                Image img = splitIconObj.GetComponent<Image>();
                img.sprite = ModUtils.getSprite(split.BossName.GetTrophySpriteKey());
                
                //Split time
                string goTimeName = "Split_"+translatedBossName+"_Time";
                GameObject splitTimeObj = GameObject.Find(goTimeName) ?? new GameObject(goTimeName, typeof(RectTransform), typeof(TextMeshProUGUI));
                splitTimeObj.transform.SetParent(goSplitsTimers.transform);
                RectTransform rectTimeObj = splitTimeObj.GetComponent<RectTransform>();
                rectTimeObj.anchorMin = new Vector2(0, 1);
                rectTimeObj.anchorMax = new Vector2(0, 1);
                rectTimeObj.pivot = new Vector2(0, 1);
                rectTimeObj.anchoredPosition = new Vector2(width + 40, height);
                rectTimeObj.sizeDelta = new Vector2(64, 32);
                TextMeshProUGUI textTimeObj = splitTimeObj.GetComponent<TextMeshProUGUI>();
                textTimeObj.transform.SetParent(goSplitsTimers.transform);
                textTimeObj.fontSize = ConfigurationFile.fontSizeSplits.Value;
                textTimeObj.alignment = TextAlignmentOptions.Left;
                textTimeObj.font = ModUtils.getFontAsset("Valheim-Norse");
                textTimeObj.color = ConfigurationFile.colorSplits.Value;
                textTimeObj.outlineColor = new Color32(
                    (byte)(textTimeObj.color.r * 255f),
                    (byte)(textTimeObj.color.g * 255f),
                    (byte)(textTimeObj.color.b * 255f),
                    255);
                textTimeObj.outlineWidth = ConfigurationFile.colorWidthSplits.Value;
                textTimeObj.text = split.TimerValue;

                count++;
                if (count == 4)
                {
                    width = (int)ConfigurationFile.positionSplits.Value.x + 102; //Default -198: -300 + 102
                    height = 455;
                    count = 0;
                }
                else
                {
                    height -= 40;
                }
            }
        }
        
        public static void Update()
        {
            if (_text == null || Game.instance == null)
                return;

            if (Time.timeScale == 0f)
            {
                _lastRealtime = Time.realtimeSinceStartupAsDouble;
                return;
            }

            if (Player.m_localPlayer == null)
                return;
            
            if (!splitsLoaded)
            {
                LoadSplits();
                splitsLoaded = true;
            }

            if (IsTimerStoppedInPermadeath())
                return;

            // Timer start check
            if (!"true".Equals(ModStatsUtils.GetSpeedrunKnownTextValue("TimerStarted"))) //Timer not started yet
            {
                //Will start when Huginn appears carrying the player
                if (ConfigurationFile.countHuginnInitTravelAsPartOfTimer.Value)
                {
                    // Start immediately
                    Logger.Log("Starting speedrun...");
                    ModStatsUtils.SetSpeedrunKnownTextKeyValue("TimerStarted", "true");
                }
                //Will start when Huginn drops off the player on the ground
                else if (Player.m_localPlayer.CanMove()) 
                {
                    Logger.Log("Starting speedrun...");
                    ModStatsUtils.SetSpeedrunKnownTextKeyValue("TimerStarted", "true");

                    var stats = ModStatsUtils.GetStats();
                    if (stats != null)
                    {
                        stats.IncrementOrSet(PlayerStatType.TimeInBase, stats.GetValueSafe(PlayerStatType.TimeInBase) * -1);
                        stats.IncrementOrSet(PlayerStatType.TimeOutOfBase, stats.GetValueSafe(PlayerStatType.TimeOutOfBase) * -1);
                    }

                    _lastRealtime = Time.realtimeSinceStartupAsDouble;
                    _displayedTime = 0;
                    _lastStatTime = 0;
                } else
                    return;
            }

            //Timer update
            float statTime = GetTotalPlaytimeSeconds();

            double now = Time.realtimeSinceStartupAsDouble;
            double deltaRealtime = now - _lastRealtime;
            _lastRealtime = now;

            // Fluid visual update
            _displayedTime += Mathf.Min((float)deltaRealtime, 0.25f);

            // Resync when stat changes
            if (statTime > _lastStatTime)
            {
                _displayedTime = statTime;
                _lastStatTime = statTime;
            }

            _text.text = !HasPlayerDeadAlreadyInPermadeath()
                ? FormatTime((float)_displayedTime)
                : GetTimerStopped();
            UpdateTimerUI();
        }

        private static string GetTimerStopped()
        {
            string timerStopped = ModStatsUtils.GetSpeedrunKnownTextValue("TimerStopped");
            if (timerStopped == null)
            {
                StopTimer();
                timerStopped = ModStatsUtils.GetSpeedrunKnownTextValue("TimerStopped");
            }

            return timerStopped;
        }

        private static float GetTotalPlaytimeSeconds()
        {
            var stats = ModStatsUtils.GetStats();
            if (stats == null)
                return 0f;

            float inBase = stats.GetValueSafe(PlayerStatType.TimeInBase);
            float outBase = stats.GetValueSafe(PlayerStatType.TimeOutOfBase);

            return inBase + outBase;
        }

        private static string FormatTime(float seconds)
        {
            int totalCentiseconds = Mathf.FloorToInt(seconds * 100f);
            int totalSeconds = totalCentiseconds / 100;

            int s = totalSeconds % 60;
            int m = (totalSeconds / 60) % 60;
            int h = totalSeconds / 3600;

            return $"{h:D2}:{m:D2}:{s:D2}";
        }

        public static bool IsCreated()
        {
            return _text != null;
        }

        public static void AddSplitTimer(Split split)
        {
            //Find existing split by name and replace, otherwise just add a new split
            int index = splits.FindIndex(s => s.BossName.Equals(split.BossName));
            if (index == -1)
            {
                splits.Add(split);
                DrawSplits();
            }
            else if (ConfigurationFile.overrideBossSplitTimerIfKilledAgain.Value)
            {
                splits[index].TimerValue = split.TimerValue;
                DrawSplits();
            }
            else
            {
                Logger.Log("Nothing to update.");
            }
        }

        public static void StopTimer()
        {
            ModStatsUtils.SetSpeedrunKnownTextKeyValue("TimerStopped", _text.text);
            UpdateTimerUI();
        }

        private static bool IsTimerStoppedInPermadeath()
        {
            return ConfigurationFile.speedrunType.Value == SpeedrunType.Permadeath &&
                   ModStatsUtils.GetSpeedrunKnownTextValue("TimerStopped") != null;
        }

        private static bool HasPlayerDeadAlreadyInPermadeath()
        {
            return ConfigurationFile.speedrunType.Value == SpeedrunType.Permadeath &&
                   ModStatsUtils.GetStats().GetValueOrDefaultPiktiv(PlayerStatType.Deaths, 0) > 0;
        }

        //For future
        public static void ResetDeaths()
        {
            ModStatsUtils.GetStats()[PlayerStatType.Deaths] = 0;
            ModStatsUtils.SetSpeedrunKnownTextKeyValue("TimerStopped", null);
        }
    }
}
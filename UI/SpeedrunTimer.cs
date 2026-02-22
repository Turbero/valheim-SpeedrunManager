using System.Collections.Generic;
using System.Reflection;
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
        public static GameObject goSplitsTimers;

        // Hybrid timer variables
        private static double _lastRealtime;
        private static double _displayedTime;
        private static float _lastStatTime;

        // Cache stats
        private static Dictionary<PlayerStatType, float> _cachedStats;
        private static Dictionary<string, string> _cachedKnownNames;
        
        // Splits
        private static List<Split> splits = new List<Split>();

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

            UpdateColor();
            
            splits.Clear();
        }

        private static void UpdateColor()
        {
            _text.color = ConfigurationFile.colorTimer.Value;
            _text.outlineColor = new Color32(
                (byte)(ConfigurationFile.colorTimer.Value.r * 255f),
                (byte)(ConfigurationFile.colorTimer.Value.g * 255f),
                (byte)(ConfigurationFile.colorTimer.Value.b * 255f),
                255);

            _text.outlineWidth = ConfigurationFile.colorWidthTimer.Value;
        }

        private static void LoadSplits()
        {
            Logger.Log("Loading splits...");
            string worldName = ZNet.instance?.GetWorldName();
            var dicKnownTexts = GetKnownNames();
            
            bool hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_Eikthyr", out var result);
            if (hasSplit)
            {
                Logger.Log("Found split for Eikthyr...");
                splits.Add(new Split(BossNameEnum.Eikthyr, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_gd_king", out result);
            if (hasSplit)
            {
                Logger.Log("Found split for The Elder...");
                splits.Add(new Split(BossNameEnum.gd_king, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_Bonemass", out result);
            if (hasSplit)
            {
                Logger.Log("Found split for Bonemass...");
                splits.Add(new Split(BossNameEnum.Bonemass, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_Dragon", out result);
            if (hasSplit)
            {
                Logger.Log("Found split for Moder...");
                splits.Add(new Split(BossNameEnum.Dragon, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_GoblinKing", out result);
            if (hasSplit)
            {
                Logger.Log("Found split for Yagluth...");
                splits.Add(new Split(BossNameEnum.GoblinKing, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_SeekerQueen", out result);
            if (hasSplit)
            {
                Logger.Log("Found split for The Queen...");
                splits.Add(new Split(BossNameEnum.SeekerQueen, result));
            }
            hasSplit = dicKnownTexts.TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_Fader", out result);
            if (hasSplit)
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

            int width = -300;
            int height = 455;
            int count = 0;
            foreach (var split in splits)
            {
                string translatedBossName = Localization.instance.Localize(split.BossName.GetTranslationKey());
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
                textTimeObj.fontSize = 20;
                textTimeObj.alignment = TextAlignmentOptions.Left;
                textTimeObj.font = ModUtils.getFontAsset("Valheim-Norse");
                textTimeObj.color = Color.white;
                textTimeObj.outlineColor = new Color32(255, 255, 255, 255);
                textTimeObj.outlineWidth = 0.05f;
                textTimeObj.text = split.TimerValue;

                count++;
                if (count == 4)
                {
                    width = -198;
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
            if (_text == null)
                return;

            if (Game.instance == null || Player.m_localPlayer == null)
            {
                _cachedStats = null;
                return;
            }

            if (splits.Count == 0)
                LoadSplits();
            
            if (Time.timeScale == 0f)
                return;

            // Inicio del timer
            /*string worldName = ZNet.instance?.GetWorldName();
            bool hasValue = GetKnownNames().TryGetValue(SpeedrunManager.GUID + "_" + worldName + "_TimerStarted", out string isTimerStarted);
            if (!hasValue || !isTimerStarted.Equals("true")) //Timer not started yet*/
            if (!ConfigurationFile.timerStarted.Value)
            {
                if (Player.m_localPlayer.CanMove())
                {
                    /* Start timer from zero */
                    ConfigurationFile.timerStarted.Value = true;

                    var stats = GetStats();
                    if (stats != null)
                    {
                        stats.IncrementOrSet(PlayerStatType.TimeInBase, stats.GetValueSafe(PlayerStatType.TimeInBase) * -1);
                        stats.IncrementOrSet(PlayerStatType.TimeOutOfBase, stats.GetValueSafe(PlayerStatType.TimeOutOfBase) * -1);
                    }

                    _lastRealtime = Time.realtimeSinceStartupAsDouble;
                    _displayedTime = 0;
                    _lastStatTime = 0;
                }
                else
                {
                    return;
                }
            }

            float statTime = GetTotalPlaytimeSeconds();

            double now = Time.realtimeSinceStartupAsDouble;
            double deltaRealtime = now - _lastRealtime;
            _lastRealtime = now;

            // Avance visual fluido
            _displayedTime += deltaRealtime;

            // Resincronización cuando el stat oficial cambia
            if (statTime > _lastStatTime)
            {
                _displayedTime = statTime;
                _lastStatTime = statTime;
            }

            _text.text = FormatTime((float)_displayedTime);

            // Reload config values
            rect.anchoredPosition = ConfigurationFile.positionTimer.Value;
            _text.fontSize = ConfigurationFile.fontSizeTimer.Value;
            _text.color = ConfigurationFile.colorTimer.Value;
            UpdateColor();
        }

        private static float GetTotalPlaytimeSeconds()
        {
            var stats = GetStats();
            if (stats == null)
                return 0f;

            float inBase = stats.GetValueSafe(PlayerStatType.TimeInBase);
            float outBase = stats.GetValueSafe(PlayerStatType.TimeOutOfBase);

            return inBase + outBase;
        }

        private static Dictionary<string, string> GetKnownNames()
        {
            if (_cachedKnownNames != null)
                return _cachedKnownNames;

            if (Player.m_localPlayer == null)
                return null;
            
            var dicKnownTexts = (Dictionary<string, string>)ModUtils.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
            _cachedKnownNames = dicKnownTexts;
            return _cachedKnownNames;
        }

        private static Dictionary<PlayerStatType, float> GetStats()
        {
            if (_cachedStats != null)
                return _cachedStats;

            if (Game.instance == null)
                return null;

            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);

            var profile = (PlayerProfile)field?.GetValue(Game.instance);
            _cachedStats = profile?.m_playerStats.m_stats;
            return _cachedStats;
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

        public static void AddNewSplit(Split split)
        {
            splits.Add(split);
            DrawSplits();
        }
    }
}
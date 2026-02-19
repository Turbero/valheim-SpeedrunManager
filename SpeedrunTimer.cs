using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace SpeedrunManager
{
    public static class SpeedrunTimer
    {
        private static RectTransform rect;
        public static TextMeshProUGUI _text;

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
            _text.color = new Color(1f, 0.7176f, 0.3603f);
            _text.font = ModUtils.getFontAsset("Valheim-Norse");

            _text.outlineColor = new Color32(255, 183, 92, 255);
        }

        public static void Update()
        {
            if (_text == null)
                return;

            if (Player.m_localPlayer == null)
                return;

            if (Time.timeScale == 0f)
                return;

            if (!ConfigurationFile.timerStarted.Value)
            {
                if (Player.m_localPlayer.CanMove())
                {
                    Logger.Log("Can move now!");
                    ConfigurationFile.timerStarted.Value = true;
                    //reset stats to avoid counting the time player was in the intro or flying with huginn
                    Dictionary<PlayerStatType, float> mStats = getPlayerDictionaryStats();
                    mStats.IncrementOrSet(PlayerStatType.TimeInBase, mStats.GetValueSafe(PlayerStatType.TimeInBase) * -1);
                    mStats.IncrementOrSet(PlayerStatType.TimeOutOfBase, mStats.GetValueSafe(PlayerStatType.TimeOutOfBase) * -1);
                }
                else
                {
                    return;
                }
            }

            _text.text = FormatTime(GetTotalPlaytimeSeconds());

            // Reload config values
            rect.anchoredPosition = ConfigurationFile.positionTimer.Value;
            _text.fontSize = ConfigurationFile.fontSizeTimer.Value;
        }

        private static float GetTotalPlaytimeSeconds()
        {
            if (Game.instance == null)
                return 0f;

            Dictionary<PlayerStatType, float> stats = getPlayerDictionaryStats();
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
        
        private static Dictionary<PlayerStatType, float> getPlayerDictionaryStats()
        {
            var field = typeof(Game).GetField("m_playerProfile", BindingFlags.Instance | BindingFlags.NonPublic);
            return ((PlayerProfile)field?.GetValue(Game.instance))?.m_playerStats.m_stats;
        }
    }
}
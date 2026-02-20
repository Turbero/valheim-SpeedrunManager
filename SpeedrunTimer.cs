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

        // Hybrid timer variables
        private static double _lastRealtime;
        private static double _displayedTime;
        private static float _lastStatTime;

        // Cache stats
        private static Dictionary<PlayerStatType, float> _cachedStats;

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

        public static void Update()
        {
            if (_text == null)
                return;

            if (Game.instance == null || Player.m_localPlayer == null)
            {
                _cachedStats = null;
                return;
            }

            if (Time.timeScale == 0f)
                return;

            // Inicio del timer
            if (!ConfigurationFile.timerStarted.Value)
            {
                if (Player.m_localPlayer.CanMove())
                {
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
    }
}
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;

namespace SpeedrunManager
{
    public static class SpeedrunTimer
    {
        private static TextMeshProUGUI _text;
        private static float _updateTimer;
        private const float UPDATE_INTERVAL = 1f;

        public static void Create(Hud hud)
        {
            if (_text != null)
                return;

            GameObject textObj = new GameObject("SpeedrunTimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(hud.transform, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(707, -20);
            rect.sizeDelta = new Vector2(500, 80);

            _text = textObj.GetComponent<TextMeshProUGUI>();
            _text.fontSize = 32;
            _text.alignment = TextAlignmentOptions.Center;
            _text.color = new Color(1f, 0.7176f, 0.3603f);
            _text.font = ModUtils.getFontAsset("Valheim-Norse");

            // Outline estilo Valheim
            //_text.outlineWidth = 0.25f;
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

            _updateTimer += Time.deltaTime;
            if (_updateTimer < UPDATE_INTERVAL)
                return;

            _updateTimer = 0f;

            float totalSeconds = GetTotalPlaytimeSeconds();
            _text.text = FormatTime(totalSeconds);
        }

        private static float GetTotalPlaytimeSeconds()
        {
            if (Game.instance == null)
                return 0f;

            var field = typeof(Game).GetField("m_playerProfile",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var profile = (PlayerProfile)field?.GetValue(Game.instance);
            if (profile == null) return 0f;

            Dictionary<PlayerStatType, float> stats =
                profile.m_playerStats.m_stats;

            if (stats == null) return 0f;

            float inBase = stats.GetValueSafe(PlayerStatType.TimeInBase);
            float outBase = stats.GetValueSafe(PlayerStatType.TimeOutOfBase);

            return inBase + outBase;
        }

        private static string FormatTime(float seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }

        public static bool IsCreated()
        {
            return _text != null;
        }
    }

    [HarmonyPatch(typeof(Hud), "Update")]
    public static class Hud_Update_Patch
    {
        static void Postfix(Hud __instance)
        {
            if (!SpeedrunTimer.IsCreated())
                SpeedrunTimer.Create(__instance);
            
            SpeedrunTimer.Update();
        }
    }
}
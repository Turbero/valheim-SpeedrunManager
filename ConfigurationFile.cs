using BepInEx.Configuration;
using BepInEx;
using System;
using System.IO;
using SpeedrunManager.UI;
using UnityEngine;

namespace SpeedrunManager
{
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        [Obsolete("To be removed")]
        public static ConfigEntry<bool> timerStarted;

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = SpeedrunManager.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        
        //Timer configuration
        public static ConfigEntry<Vector2> positionTimer;
        public static ConfigEntry<Color> colorTimer;
        public static ConfigEntry<float> colorWidthTimer;
        public static ConfigEntry<float> fontSizeTimer;
        
        //Speedrun configuration
        public static ConfigEntry<bool> showSplits;
        public static ConfigEntry<bool> stopTimerAfterDyingFirstTime; //TODO
        public static ConfigEntry<Color> colorTimerAfterDyingFirstTime; //TODO

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                debug = configFile.Bind("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                timerStarted = configFile.Bind("1 - General", "Speedrun Active", false, "Enabling/Disabling the speedrun (DO NOT CHANGE IN-GAME! ONLY FROM MAIN SCREEN)");
                hotKey = configFile.Bind("1 - General", "Speedrun Hotkey Panel", KeyCode.Y, "Key to show/hide the speedrun panel configuration");

                positionTimer = configFile.Bind("2 - UI Timer", "Position", new Vector2(870, -20), new ConfigDescription("UI Timer position"));
                colorTimer = configFile.Bind("2 - UI Timer", "Color", new Color(1f, 0.7176f, 0.3603f), new ConfigDescription("UI Timer color"));
                colorWidthTimer = configFile.Bind("2 - UI Timer", "Color Intensity", 0.15f, new ConfigDescription("UI Timer color intensity (recommended between 0 and 0.5f)"));
                fontSizeTimer = configFile.Bind("2 - UI Timer", "Size", 32f, new ConfigDescription("UI Timer size"));
                colorTimerAfterDyingFirstTime = configFile.Bind("2 - UI Timer", "Timer Color After Dying for First Time", new Color(1, 0, 0), "Timer color after dying for first time");

                showSplits = configFile.Bind("3 - Configuration", "Show Splits", true, "Show/hide splits information");
                stopTimerAfterDyingFirstTime = configFile.Bind("3 - Configuration", "Stop Timer After Dying for First Time", false, "Stops the timer after dying for first time");
                
                SetupWatcher();
            }
        }

        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.Log("Attempting to reload configuration...");
                configFile.Reload();
                SettingsChanged(null, null);
            }
            catch (Exception ex)
            {
                Logger.LogError($"There was an issue loading {ConfigFileName}, {ex}");
            }
        }

        private static void SettingsChanged(object sender, EventArgs e)
        {
            SpeedrunTimer.goSplitsTimers.SetActive(showSplits.Value);
            if (SpeedrunTimer._text != null)
            {
                SpeedrunTimer._text.color = colorTimer.Value;
                SpeedrunTimer._text.outlineColor = new Color32(
                    byte.Parse((colorTimer.Value.r * 255f).ToGlobalInvariantString()),
                    byte.Parse((colorTimer.Value.g * 255f).ToGlobalInvariantString()),
                    byte.Parse((colorTimer.Value.b * 255f).ToGlobalInvariantString()),
                    byte.Parse("255"));
            }
        }
    }
}
using BepInEx.Configuration;
using BepInEx;
using System;
using System.IO;
using SpeedrunManager.UI;
using UnityEngine;

namespace SpeedrunManager
{
    public enum SpeedrunType
    {
        Permadeath,
        InfiniteLives
    }
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        public static ConfigEntry<SpeedrunType> speedrunType;

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = SpeedrunManager.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        
        //Timer configuration
        public static ConfigEntry<Vector2> positionTimer;
        public static ConfigEntry<Color> colorTimer;
        public static ConfigEntry<float> colorWidthTimer;
        public static ConfigEntry<int> fontSizeTimer;
        //Splits configuration
        public static ConfigEntry<Vector2> positionSplits;
        public static ConfigEntry<Color> colorSplits;
        public static ConfigEntry<float> colorWidthSplits;
        public static ConfigEntry<int> fontSizeSplits;
        
        //Speedrun configuration
        public static ConfigEntry<bool> showSplits;
        public static ConfigEntry<Color> colorTimerAfterDyingInPermadeath;
        public static ConfigEntry<bool> countHuginnInitTravelAsPartOfTimer;
        public static ConfigEntry<bool> overrideBossSplitTimerIfKilledAgain;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                debug = configFile.Bind("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                hotKey = configFile.Bind("1 - General", "Hotkey Panel", KeyCode.Y, "Key to show/hide the speedrun panel configuration");

                positionTimer = configFile.Bind("2 - UI Timer", "Position", new Vector2(870, -20), new ConfigDescription("UI Timer position"));
                colorTimer = configFile.Bind("2 - UI Timer", "Color", new Color(1f, 0.7176f, 0.3603f), new ConfigDescription("UI Timer color"));
                colorWidthTimer = configFile.Bind("2 - UI Timer", "Color Intensity", 0.15f, new ConfigDescription("UI Timer color intensity (recommended between 0 and 0.5f)"));
                fontSizeTimer = configFile.Bind("2 - UI Timer", "Size", 64, new ConfigDescription("UI Timer size"));
                colorTimerAfterDyingInPermadeath = configFile.Bind("2 - UI Timer", "Timer Color After Dying In Permadeath", new Color(1, 0, 0), "Timer color after dying for first time in permadeath mode");

                positionSplits = configFile.Bind("2.1 - UI Splits", "Splits Position", new Vector2(-300, 455), new ConfigDescription("UI Splits position"));
                colorSplits = configFile.Bind("2.1 - UI Splits", "Splits Color", Color.white, new ConfigDescription("UI Splits color"));
                colorWidthSplits = configFile.Bind("2.1 - UI Splits", "Splits Color Intensity", 0.05f, new ConfigDescription("UI Splits color intensity (recommended between 0 and 0.5f)"));
                fontSizeSplits = configFile.Bind("2.1 - UI Splits", "Splits Size", 20, new ConfigDescription("UI Splits size"));
                
                speedrunType =  configFile.Bind("3 - Configuration", "Speedrun Type", SpeedrunType.Permadeath, new ConfigDescription("Speedrun type"));
                showSplits = configFile.Bind("3 - Configuration", "Show Splits", true, new ConfigDescription("Show/hide splits information"));
                countHuginnInitTravelAsPartOfTimer = configFile.Bind("3 - Configuration", "Count from Huginn Intro", false, new ConfigDescription("If active, the time will start since Huginn is taking you to the spawn, that's ~1min47seg (default = false)"));
                overrideBossSplitTimerIfKilledAgain = configFile.Bind("3 - Configuration", "Override Boss Split Timer if killed again", false, new ConfigDescription("Replaces the time the boss was killed in his split when it is killed again if this is enabled (default = false)"));
                    
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
            if (SpeedrunTimer._text != null)
            {
                // Reload config values
                SpeedrunTimer.UpdateTimerUI();
            }
        }
    }
}
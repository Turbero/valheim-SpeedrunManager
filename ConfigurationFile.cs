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
    
    public enum Sort
    {
        Ascending,
        Descending
    }

    public class DefaultUIPositions
    {
        public static readonly Vector2 positionTimer = new Vector2(1540, 1000);
        public static readonly int fontSizeTimer = 54;
        
        public static readonly Vector2 positionSplits = new Vector2(1780, 620);
        public static readonly int fontSizeSplits = 20;
        public static readonly int splitsColumnSize = 8;
        public static readonly int splitsColumnsSpace = 0;
        public static readonly int splitsRowsSpace = 40;
    }
    
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<KeyCode> hotKey;
        public static ConfigEntry<SpeedrunType> speedrunType;

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = SpeedrunManager.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        
        //Speedrun configuration
        public static ConfigEntry<Color> colorTimerAfterDyingInPermadeath;
        public static ConfigEntry<bool> countHuginnInitTravelAsPartOfTimer;
        public static ConfigEntry<bool> overrideBossSplitTimerIfKilledAgain;
        public static ConfigEntry<int> effectsPerRow;
        public static ConfigEntry<string> fontNameTimer;
        
        //Timer configuration
        public static ConfigEntry<bool> showTimer;
        public static ConfigEntry<Vector2> positionTimer;
        public static ConfigEntry<Color> colorTimer;
        public static ConfigEntry<float> colorWidthTimer;
        public static ConfigEntry<int> fontSizeTimer;
        //Splits configuration
        public static ConfigEntry<bool> showSplits;
        public static ConfigEntry<string> fontNameSplits;
        public static ConfigEntry<Vector2> positionSplits;
        public static ConfigEntry<Color> colorSplits;
        public static ConfigEntry<float> colorWidthSplits;
        public static ConfigEntry<int> fontSizeSplits;
        public static ConfigEntry<int> splitsColumnSize;
        public static ConfigEntry<int> splitsColumnsSpace;
        public static ConfigEntry<int> splitsRowsSpace;
        public static ConfigEntry<Sort> splitSort;
        

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                debug = configFile.Bind("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                hotKey = configFile.Bind("1 - General", "Hotkey Panel", KeyCode.Y, "Key to show/hide the speedrun panel configuration");
                
                speedrunType =  configFile.Bind("2 - Configuration", "Speedrun Type", SpeedrunType.Permadeath, new ConfigDescription("Speedrun type"));
                countHuginnInitTravelAsPartOfTimer = configFile.Bind("2 - Configuration", "Count from Huginn Intro", false, new ConfigDescription("If active, the time will start since Huginn is taking you to the spawn, that's ~1min47seg (default = false)"));
                overrideBossSplitTimerIfKilledAgain = configFile.Bind("2 - Configuration", "Override Boss Split Timer if killed again", false, new ConfigDescription("Replaces the time the boss was killed in his split when it is killed again if this is enabled (default = false)"));
                effectsPerRow = configFile.Bind("2 - Configuration", "Effects Per Row", 7, new ConfigDescription("Effects Per Row", new AcceptableValueRange<int>(1, 8)));

                showTimer = configFile.Bind("3 - UI", "Show Timer", true, new ConfigDescription("Show/hide timer (still running while hidden)"));
                showSplits = configFile.Bind("3 - UI", "Show Splits", true, new ConfigDescription("Show/hide splits information"));
                fontNameTimer = configFile.Bind("3 - UI", "Font Timer", "segoeui.ttf", new ConfigDescription("Choose a font file name on your computer for a custom timer font style (Example: segoeui.ttf). Empty = default game font"));
                fontNameSplits = configFile.Bind("3 - UI", "Font Splits", "segoeui.ttf", new ConfigDescription("Choose a font file name on your computer for a custom splits font style (Example: segoeui.ttf). Empty = default game font"));

                positionTimer = configFile.Bind("3.1 - UI Timer", "Position", DefaultUIPositions.positionTimer, new ConfigDescription("UI Timer position"));
                colorTimer = configFile.Bind("3.1 - UI Timer", "Color", new Color(0, 1, 0), new ConfigDescription("UI Timer color"));
                colorWidthTimer = configFile.Bind("3.1 - UI Timer", "Color Intensity", 0.15f, new ConfigDescription("UI Timer color intensity (recommended between 0 and 0.5f)"));
                fontSizeTimer = configFile.Bind("3.1 - UI Timer", "Size", DefaultUIPositions.fontSizeTimer, new ConfigDescription("UI Timer size"));
                colorTimerAfterDyingInPermadeath = configFile.Bind("3.1 - UI Timer", "Timer Color After Dying In Permadeath", new Color(1, 0, 0), "Timer color after dying for first time in permadeath mode");

                positionSplits = configFile.Bind("3.2 - UI Splits", "Splits Position", DefaultUIPositions.positionSplits, new ConfigDescription("UI Splits position"));
                colorSplits = configFile.Bind("3.2 - UI Splits", "Splits Color", Color.white, new ConfigDescription("UI Splits color"));
                colorWidthSplits = configFile.Bind("3.2 - UI Splits", "Splits Color Intensity", 0.05f, new ConfigDescription("UI Splits color intensity (recommended between 0 and 0.5f)"));
                fontSizeSplits = configFile.Bind("3.2 - UI Splits", "Splits Font Size", DefaultUIPositions.fontSizeSplits, new ConfigDescription("UI Splits size"));
                splitsColumnSize = configFile.Bind("3.2 - UI Splits", "Splits Column Size", DefaultUIPositions.splitsColumnSize, new ConfigDescription("UI Splits size", new AcceptableValueRange<int>(4, 8)));
                splitsColumnsSpace = configFile.Bind("3.2 - UI Splits", "Splits Columns Space", DefaultUIPositions.splitsColumnsSpace, new ConfigDescription("UI Splits Columns Space"));
                splitsRowsSpace = configFile.Bind("3.2 - UI Splits", "Splits Rows Space", DefaultUIPositions.splitsRowsSpace, new ConfigDescription("UI Splits Rows Space"));
                splitSort = configFile.Bind("3.2 - UI Splits", "Splits Sort", Sort.Ascending, new ConfigDescription("UI Splits Sort"));

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
                Hud.instance.m_effectsPerRow = effectsPerRow.Value;
                SpeedrunTimer._text.font = ModFonts.GetFontTimer(true);
                // Reload config values
                SpeedrunTimer.UpdateVisibility();
                SpeedrunTimer.UpdateTimer();
                SpeedrunTimer.UpdateTimerUI();
                SpeedrunTimer.DrawSplits();
            }
        }
    }
}
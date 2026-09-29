using TMPro;
using UnityEngine;

namespace SpeedrunManager
{
    public static class ModFonts
    {
        private static TMP_FontAsset _font;

        public static TMP_FontAsset GetFontTimer(bool recalculate = false)
        {
            if (_font != null && !recalculate)
                return _font;

            if (!string.IsNullOrEmpty(ConfigurationFile.fontNameTimer.Value))
            {
                foreach (var fontPath in Font.GetPathsToOSFonts())
                {
                    Logger.Log("OS font path: " + fontPath);
                    if (fontPath.Contains(ConfigurationFile.fontNameTimer.Value))
                    {
                        Font font = new Font(fontPath);
                        _font = TMP_FontAsset.CreateFontAsset(font);
                        Logger.LogInfo($"{ConfigurationFile.fontNameTimer.Value} found in OS.");
                        return _font;
                    }
                }
                Logger.LogInfo($"{ConfigurationFile.fontNameTimer.Value} not found in OS. Using Valheim-Norse as default font");
            }
            
            _font = ModUtils.getFontAsset("Valheim-Norse");
            return _font;
        }
        
        public static TMP_FontAsset GetFontSplits(bool recalculate = false)
        {
            if (_font != null && !recalculate)
                return _font;

            if (!string.IsNullOrEmpty(ConfigurationFile.fontNameSplits.Value))
            {
                foreach (var fontPath in Font.GetPathsToOSFonts())
                {
                    Logger.Log("OS font path: " + fontPath);
                    if (fontPath.Contains(ConfigurationFile.fontNameSplits.Value))
                    {
                        Font font = new Font(fontPath);
                        _font = TMP_FontAsset.CreateFontAsset(font);
                        Logger.LogInfo($"{ConfigurationFile.fontNameSplits.Value} found in OS.");
                        return _font;
                    }
                }
                Logger.LogInfo($"{ConfigurationFile.fontNameSplits.Value} not found in OS. Using Valheim-Norse as default font");
            }
            
            _font = ModUtils.getFontAsset("Valheim-Norse");
            return _font;
        }
    }
}
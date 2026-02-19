using BepInEx;
using HarmonyLib;

namespace SpeedrunManager
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class SpeedrunManager : BaseUnityPlugin
    {
        public const string GUID = "Turbero.SpeedrunManager";
        public const string NAME = "Speedrun Manager";
        public const string VERSION = "1.0.0";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            ConfigurationFile.LoadConfig(this); 

            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace SpeedrunManager
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class SpeedrunManager : BaseUnityPlugin
    {
        public const string GUID = "Turbero.SpeedrunManager";
        public const string NAME = "Speedrun Manager";
        public const string VERSION = "0.1.0";

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
        
        private void Start()
        {
            StartCoroutine(WaitForNetworking());
        }

        private System.Collections.IEnumerator WaitForNetworking()
        {
            // Wait until full networking initialization
            while (ZRoutedRpc.instance == null || ZNet.instance == null)
                yield return new WaitForSeconds(1f);
            
            // Commands registration
            SplitsCommands.RegisterConsoleCommand();
        }
    }
}

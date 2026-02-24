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
        public const string VERSION = "0.2.0";

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
        
        void Update()
        {
            if (!Player.m_localPlayer || !InventoryGui.instance) return;

            // Check if certain keys are hit to close Almanac GUI
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Player.m_localPlayer.IsDead())
            {
                hideSpeedrunPanel();
            }

            // Hotkey to open/close skills dialog (if game is not paused)
            if (Input.GetKeyDown(ConfigurationFile.hotKey.Value) && Time.timeScale > 0)
            {
                if (Hud.instance.gameObject.activeSelf)
                {
                    hideSpeedrunPanel();
                }
                else
                {
                    showSpeedrunPanel();
                }
            }
        }

        private void hideSpeedrunPanel()
        {
            //TODO
            
        }

        private void showSpeedrunPanel()
        {
            //TODO
            
        }
    }
}

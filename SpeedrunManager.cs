using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using SpeedrunManager.UI;
using UnityEngine;

namespace SpeedrunManager
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class SpeedrunManager : BaseUnityPlugin
    {
        public const string GUID = "Turbero.SpeedrunManager";
        public const string NAME = "Speedrun Manager";
        public const string VERSION = "1.0.2";

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
            if (!Player.m_localPlayer || InventoryGui.IsVisible() || !Hud.instance || !SpeedrunConfigPanel.IsCreated()) return;

            // Check if certain keys are hit to close panel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SpeedrunConfigPanel.IsVisible())
                {
                    SpeedrunConfigPanel.Hide(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Player.m_localPlayer.IsDead())
            {
                if (SpeedrunConfigPanel.IsVisible())
                {
                    SpeedrunConfigPanel.Hide();
                }
            }

            // Hotkey to open/close speedrun config
            if (Input.GetKeyDown(ConfigurationFile.hotKey.Value))
            {
                if (SpeedrunConfigPanel.IsVisible())
                {
                    SpeedrunConfigPanel.Hide();
                }
                else if (CanShowSpeedrunConfigPanel())
                {
                    if (!SpeedrunConfigPanel.IsCreated())
                        SpeedrunConfigPanel.Create();
            
                    SpeedrunConfigPanel.Show();
                }
            }
        }

        private static bool CanShowSpeedrunConfigPanel()
        {
            return Player.m_localPlayer != null && 
                   (Player.m_localPlayer.CanMove() || Player.m_localPlayer.IsSitting()) && //player must be able to move or be sitting
                   !Game.IsPaused() && //not when game is paused
                   !InventoryGui.IsVisible() && //not when inventory is opened
                   !Console.IsVisible() && //not when console is visible
                   !Hud.instance.transform.parent.Find("Chat_box/root/ChatInput").gameObject.activeSelf && // Not when typing in the chat
                   !Game.instance.transform.Find("LoadingGUI/PixelFix/IngameGui/TextInput/panel").gameObject.activeSelf && //Not when changing an input field
                   !Minimap.instance.transform.Find("large").gameObject.activeSelf && //Not while large map is opened
                   !StoreGui.instance.m_rootPanel.activeSelf; //Not when any store is opened
        }
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.GetKnownTexts))]
    public class FixCompendium
    {
        public static void Postfix(ref List<KeyValuePair<string, string>> __result)
        {
            __result = __result.Where(p => !p.Key.StartsWith(SpeedrunManager.GUID)).ToList();
        }
    }
}

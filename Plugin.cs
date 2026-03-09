using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameServerUtilsForWL {

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        internal static new ManualLogSource Logger;
        public static Harmony harmony;
        public static Settings settings;
        private void Awake() {
            // Plugin startup logic
            settings = JsonUtility.FromJson<Settings>(PlayerPrefs.GetString(MyPluginInfo.PLUGIN_GUID, JsonUtility.ToJson(new Settings())));
            HashCode = GetHashCode();
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal void Update() {
            if (GameInstance.InstanceExists) {
                foreach (var controller in GameInstance.Instance.GetPlayerControllers()) {
                    if (controller.IsLocal() && controller.IsServer() && DedicatedServerMode) {
                        controller.networkObject.Destroy();
                    }
                }
                if (settings.patchVehicles) {
                    VehicleManager.Instance.GetVehicles().ForEach(x => x.networkObject.Destroy());
                }
            }
        }

        private static int HashCode = 0;
        public static bool Setup {
            get; private set;
        } = false;
        private static bool active = false;
        public static bool Active => Setup && active;
        public static bool DedicatedServerMode => Active && settings.dedicatedServer;
        private static Vector2 res => new(Screen.width * 0.6f, Screen.height * 0.3f);
        internal void OnGUI() {
            if (Setup)
                return;
            GUILayout.Window(HashCode, new(Screen.width / 2 - res.x / 2, Screen.height / 1.5f, res.x, res.y), WindowLogic, "Game Server for Wobbly Life Setup");
        }
        private bool openSettings = false;
        private void WindowLogic(int id) {
            GUILayout.BeginVertical();
            GUILayout.Box("This plugin is used to setup a game server for Wobbly Life. It will patch the game for this instance only and CANNOT be reverted unless you restart the game");
            GUILayout.Box("Do you want to set it up?");
            active = GUILayout.Toggle(active, "Yes, I want to set it up");
            if (GUILayout.Button("Settings")) {
                openSettings = !openSettings;
            }
            if (openSettings)
                SettingsLogic();
            if (GUILayout.Button("Confirm")) {
                Setup = true;
                if (!active) {
                    settings = new();
                    return;
                }
                harmony = new(MyPluginInfo.PLUGIN_GUID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            GUILayout.EndVertical();
        }

        private void SettingsLogic() {
            if (GUILayout.Button(settings.dedicatedServer ? "Dedicated Server" : "Peer-to-Peer")) {
                settings.dedicatedServer = !settings.dedicatedServer;
            }
            settings.patchPropShop = GUILayout.Toggle(settings.patchPropShop, "Patch Prop Shop (Block Prop Shop Spawning)");
            settings.patchTrafficManager = GUILayout.Toggle(settings.patchTrafficManager, "Patch Traffic Manager (Block Npc Cars Spawning)");
            settings.patchJobs = GUILayout.Toggle(settings.patchJobs, "Patch Jobs (Block Jobs from Starting)");
            settings.patchVehicles = GUILayout.Toggle(settings.patchVehicles, "Patch Vehicles (Remove Vehicles from the Game)");
            if(!active) PlayerPrefs.SetString(MyPluginInfo.PLUGIN_GUID, JsonUtility.ToJson(settings));
        }
    }
}

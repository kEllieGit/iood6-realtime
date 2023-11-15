using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace RealTime
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            HarmonyPatch();
        }

        private static void HarmonyPatch()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}

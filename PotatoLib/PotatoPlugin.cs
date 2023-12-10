using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using PotatoLib.API;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PotatoLib
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PotatoPlugin : BaseUnityPlugin
    {
        private const string PLUGIN_ASCII_ART = @"

  _                    
 |_) _ _|_  _. _|_  _  
 |  (_) |_ (_|  |_ (_) 

";
        
        public const string MOD_GAME_VERSION = "v45";
        
        public static PotatoPlugin? Instance { get; private set; }
        public ManualLogSource PluginLogger => Logger;

        private Harmony? _harmony;
        public string? GameVersion { get; private set; }
        
        private void Awake()
        {
            Instance = this;

            Logger.LogInfo(PLUGIN_ASCII_ART);
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Patched {_harmony.GetPatchedMethods().Count()} methods.");
            
            PotatoTerminal.Initialize();

            PotatoTerminal.RegisterCommand(
                new StaticTerminalCommand(
                    "mods", 
                    "List all loaded mods", 
                    $"Loaded mods: [modsCount]\n- [modsList]"
                    )
                );

            // PotatoTerminal.RegisterCommand(new StaticTerminalCommand("test", "Test command")
            //     .WithHidden(true)
            //     .WithActionAndOnlyAction((terminal, _node) =>
            //     {
            //         terminal.QuitTerminal();
            //         GameNetworkManager.Instance.localPlayerController.TeleportPlayer(new Vector3(0,500,0));
            //         Logger.LogInfo("Test command invoked!");
            //     }));
            //
            // PotatoTerminal.RegisterCommand(new StaticTerminalCommand("boykisser")
            //     .WithHidden(true)
            //     .WithTextureUrl("https://img.itch.zone/aW1nLzE0MjM1OTU1LmpwZw==/original/sh4SvI.jpg")
            //     );

            PotatoTerminal.RegisterTextPostProcess(
                new ReplaceTerminalTextPostProcess("[modsCount]", () => Chainloader.PluginInfos.Count().ToString()),
                new ReplaceTerminalTextPostProcess("[modsList]", () => string.Join("\n- ", Chainloader.PluginInfos.Keys))
                );

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Logger.LogInfo("Ready!");
        }

        /// <summary>
        /// Creates a logger with the name of the plugin.
        /// </summary>
        /// <param name="subname">The subname of the logger.</param>
        /// <returns>The created logger.</returns>
        public ManualLogSource CreateLogger(string subname)
        {
            return BepInEx.Logging.Logger.CreateLogSource($"{PluginInfo.PLUGIN_NAME}/{subname}");
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenu")
            {
                var versionNum = GameObject.Find("VersionNum");
                if (versionNum != null)
                {
                    GameVersion = versionNum.GetComponent<TextMeshProUGUI>().text;
                    Logger.LogInfo($"Found game version: {GameVersion}");
                    if (GameVersion != MOD_GAME_VERSION)
                        Logger.LogWarning($"Game version mismatch! Expected {MOD_GAME_VERSION}, got {GameVersion}, some features may not work!");
                }
                else
                {
                    Logger.LogWarning("Failed to find game version!");
                }
            }
        }
    }
}
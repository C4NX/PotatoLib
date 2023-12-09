using System.Collections;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using PotatoLib.API;
using PotatoLib.Patchs;
using PotatoLib.Utils;
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
        
        public static PotatoPlugin Instance { get; private set; }
        public ManualLogSource PluginLogger => Logger;

        private Harmony _harmony;
        private string _gameVersion = "Unknown";
        
        private void Awake()
        {
            Instance = this;

            Logger.LogInfo(PLUGIN_ASCII_ART);
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Patched {_harmony.GetPatchedMethods().Count()} methods.");
            
            PotatoTerminal.Initialize();
            var modsCount = Chainloader.PluginInfos.Count();
            
            PotatoTerminal.RegisterCommand(
                new StaticTerminalCommand(
                    "mods", 
                    "List all loaded mods", 
                    $"Loaded mods: {modsCount}\n- {string.Join("\n- ", Chainloader.PluginInfos.Keys)}"
                    )
                );

            PotatoTerminal.RegisterCommand(new StaticTerminalCommand("test")
                .WithActionAndOnlyAction((terminal, _node) =>
                {
                    terminal.QuitTerminal();
                    GameNetworkManager.Instance.localPlayerController.TeleportPlayer(Vector3.zero);
                    Logger.LogInfo("Test command invoked!");
                }));
            
            PotatoTerminal.OnTerminalTextPostProcess += text =>
            {
                Logger.LogInfo($"Terminal text: {text}");
                return text;
            };

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Logger.LogInfo("Ready!");
        }

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
                    _gameVersion = versionNum.GetComponent<TextMeshProUGUI>().text;
                    Logger.LogInfo($"Found game version: {_gameVersion}");
                }
                else
                {
                    Logger.LogWarning("Failed to find game version!");
                }
            }
        }
    }
}
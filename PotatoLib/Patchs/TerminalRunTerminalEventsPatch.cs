using System;
using BepInEx.Logging;
using HarmonyLib;

namespace PotatoLib.Patchs;

[HarmonyPatch(typeof(Terminal), "RunTerminalEvents")]
public class TerminalRunTerminalEventsPatch
{
    public static Action<Terminal, TerminalNode> OnTerminalRunTerminalEvents;
    
    [HarmonyPostfix]
    public static void Postfix(Terminal __instance, TerminalNode node)
    {
        OnTerminalRunTerminalEvents?.Invoke(__instance, node);
    }
}
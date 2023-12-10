using System;
using HarmonyLib;
using PotatoLib.API;

namespace PotatoLib.Patchs;

[HarmonyPatch(typeof(Terminal), "TextPostProcess")]
public class TerminalTextPostProcessPatch
{
    public static event Func<string, string> OnTerminalTextPostProcess;
    
    [HarmonyPostfix]
    public static void Postfix(ref string __result)
    {
        __result = OnTerminalTextPostProcess?.Invoke(__result) ?? __result;
    }
}
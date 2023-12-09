using HarmonyLib;
using PotatoLib.API;

namespace PotatoLib.Patchs;

[HarmonyPatch(typeof(Terminal), "TextPostProcess")]
public class TerminalTextPostProcessPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result)
    {
        __result = PotatoTerminal.InvokeTerminalTextPostProcess(__result);
    }
}
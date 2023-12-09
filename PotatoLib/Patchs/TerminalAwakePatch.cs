using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using PotatoLib.API;
using UnityEngine;

namespace PotatoLib.Patchs;

[HarmonyPatch(typeof(Terminal), "Awake")]
public class TerminalAwakePatch
{
    [CanBeNull] public static Terminal Instance { get; private set; }
    
    public static event Action<Terminal> OnTerminalAwake;
    
    public static string OriginalHelpText { get; private set; }

    [HarmonyPostfix]
    public static void Postfix(Terminal __instance)
    {
        Instance = __instance;
        OriginalHelpText = __instance.terminalNodes.specialNodes[13].displayText;
        OnTerminalAwake?.Invoke(__instance);
    }
    
    public static void AddPotatoCommand(string keyword, ITerminalCommand command, [CanBeNull] TerminalKeyword verb = null)
    {
        if(Instance == null)
            throw new NullReferenceException("Terminal is not awake yet!");
        
        var allKeywords = new List<TerminalKeyword>(Instance.terminalNodes.allKeywords);
        var newKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        newKeyword.word = keyword;
        newKeyword.isVerb = verb != null;
        newKeyword.defaultVerb = verb;
        
        command.Apply(newKeyword, Instance);
        
        allKeywords.Add(newKeyword);
        Instance.terminalNodes.allKeywords = allKeywords.ToArray();
    }
}
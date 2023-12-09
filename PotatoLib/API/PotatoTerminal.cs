using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using PotatoLib.Patchs;

namespace PotatoLib.API;

public static class PotatoTerminal
{
    public static event Func<string, string> OnTerminalTextPostProcess;

    private static List<ITerminalCommand> _waitToBeRegistered = new List<ITerminalCommand>();
    private static List<ITerminalCommand> _commands = new List<ITerminalCommand>();
    private static ManualLogSource _logger = PotatoPlugin.Instance.CreateLogger("Terminal");
    
    internal static void Initialize()
    {
        _logger = PotatoPlugin.Instance.CreateLogger("Terminal");
        TerminalAwakePatch.OnTerminalAwake += (_) =>
        {
            foreach (var command in _waitToBeRegistered)
            {
                _RegisterCommand(command);
            }
            
            _waitToBeRegistered.Clear();
        };
        TerminalRunTerminalEventsPatch.OnTerminalRunTerminalEvents += (terminal, node) =>
        {
            foreach (var command in _commands)
            {
                if(node.terminalEvent == $"potato:{command.Name}")
                    try
                    {
                        command.EventAction?.Invoke(terminal, node);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error while invoking command {command.Name}: {e}");
                    }
            }
        };
    }
    
    internal static string InvokeTerminalTextPostProcess(string text)
    {
        return OnTerminalTextPostProcess?.Invoke(text) ?? text;
    }
    
    public static void RegisterCommand(ITerminalCommand command)
    {
        _commands.Add(command);
        
        if (TerminalAwakePatch.Instance == null)
            _waitToBeRegistered.Add(command);
        else
            _RegisterCommand(command);
    }
    
    private static void _RegisterCommand(ITerminalCommand command)
    {
        _logger.LogInfo($"Registering command: {command.Name}");
        TerminalAwakePatch.AddPotatoCommand(command.Name, command);
        _BuildHelpText();
    }
    
    private static void _BuildHelpText()
    {
        if (TerminalAwakePatch.Instance == null)
            throw new NullReferenceException("Terminal is not awake yet!");

        var sb = new StringBuilder(
            TerminalAwakePatch.OriginalHelpText
                .Replace("[numberOfItemsOnRoute]", string.Empty)
                .TrimEnd('\n')
            );

        sb.AppendLine("\n");
        foreach (var command in _commands.Where(x=>!x.IsHidden))
        {
            sb.AppendLine($"><color=yellow>{command.Name.ToUpperInvariant()}</color> \n{command.Description}\n");
        }

        sb.AppendLine("[numberOfItemsOnRoute]");
        
        TerminalAwakePatch.Instance.terminalNodes.specialNodes[13].displayText = sb.ToString();
    }
}
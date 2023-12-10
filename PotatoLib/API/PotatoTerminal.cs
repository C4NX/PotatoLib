using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using PotatoLib.Patchs;

namespace PotatoLib.API;

/// <summary>
/// The main class for registering commands to the terminal.
/// </summary>
public static class PotatoTerminal
{
    private static readonly List<ITerminalCommand> _waitToBeRegistered = new List<ITerminalCommand>();
    private static readonly List<ITerminalCommand> _commands = new List<ITerminalCommand>();
    private static readonly List<ITerminalTextPostProcess> _textPostProcesses = new List<ITerminalTextPostProcess>();
    private static ManualLogSource? _logger;
    
    /// <summary>
    /// Initializes the terminal API.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if <see cref="PotatoPlugin.Instance"/> is null.</exception>
    internal static void Initialize()
    {
        if(PotatoPlugin.Instance == null)
            throw new NullReferenceException("PotatoPlugin.Instance is null!");

        _logger = PotatoPlugin.Instance.CreateLogger("Terminal");
        
        TerminalAwakePatch.OnTerminalAwake += _OnTerminalAwake;
        TerminalRunTerminalEventsPatch.OnTerminalRunTerminalEvents += _OnTerminalRunTerminalEvents;
        TerminalTextPostProcessPatch.OnTerminalTextPostProcess += _OnOnTerminalTextPostProcess;
    }

    /// <summary>
    /// Registers a terminal command, you can use <see cref="StaticTerminalCommand"/> for a simple implementation.
    /// </summary>
    /// <param name="command">The command to register.</param>
    public static void RegisterCommand(ITerminalCommand command)
    {
        _commands.Add(command);
        
        if (TerminalAwakePatch.Instance == null)
            _waitToBeRegistered.Add(command);
        else
            _RegisterCommand(command);
    }
    
    /// <summary>
    /// Registers multiple terminal text post processes, you can use <see cref="ReplaceTerminalTextPostProcess"/> for a simple implementation.
    /// </summary>
    /// <param name="postProcess">The post process to register.</param>
    public static void RegisterTextPostProcess(params ITerminalTextPostProcess[] postProcess)
        => _textPostProcesses.AddRange(postProcess);

    private static void _RegisterCommand(ITerminalCommand command)
    {
        _logger?.LogInfo($"Registering command: {command.Name}");
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
    
    private static void _OnTerminalAwake(Terminal _)
    {
        foreach (var command in _waitToBeRegistered)
            _RegisterCommand(command);
            
        _waitToBeRegistered.Clear();
    }
    
    private static void _OnTerminalRunTerminalEvents(Terminal terminal, TerminalNode node)
    {
        foreach (var command in _commands.Where(command => node.terminalEvent == $"potato:{command.Name}"))
        {
            try
            {
                command.EventAction?.Invoke(terminal, node);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while invoking command {command.Name}: {e}");
            }
        }
    }
    
    private static string _OnOnTerminalTextPostProcess(string text)
        => _textPostProcesses.Aggregate(text, (current, postProcess) => postProcess.Apply(current));
}
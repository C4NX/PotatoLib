using System;

namespace PotatoLib.API;

/// <summary>
///  A command that can be registered to the terminal.
/// </summary>
public interface ITerminalCommand
{
    /// <summary>
    /// Applies the command to the terminal (registers it).
    /// </summary>
    /// <param name="keyword">The keyword that is used to invoke the command.</param>
    /// <param name="terminal">The terminal to register the command to.</param>
    void Apply(TerminalKeyword keyword, Terminal terminal);
    
    /// <summary>
    /// The name of the command (used by the help command and default keyword to be used).
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The description of the command used by the help command.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// If the command is hidden from the help command.
    /// </summary>
    bool IsHidden { get; }
    
    /// <summary>
    /// The action to be invoked when the command is invoked.
    /// </summary>
    Action<Terminal, TerminalNode> EventAction { get; }
}
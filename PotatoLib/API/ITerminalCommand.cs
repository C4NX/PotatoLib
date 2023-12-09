using System;

namespace PotatoLib.API;

public interface ITerminalCommand
{
    void Apply(TerminalKeyword keyword, Terminal terminal);
    string Name { get; }
    string Description { get; }
    Action<Terminal, TerminalNode> EventAction { get; }
    bool IsHidden { get; }
}
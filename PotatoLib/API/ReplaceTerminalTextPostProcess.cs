using System;

namespace PotatoLib.API;

/// <summary>
/// Can be used to post process the terminal text before it is displayed.
/// </summary>
public class ReplaceTerminalTextPostProcess : ITerminalTextPostProcess
{
    /// <summary>
    /// The text to be replaced.
    /// </summary>
    public string From { get; set; }
    
    /// <summary>
    /// The text to replace with dynamically, if null, <see cref="To"/> will be used.
    /// </summary>
    public Func<string>? ToFunc { get; set; }
    
    /// <summary>
    /// The text to replace with, if null, nothing will be replaced.
    /// </summary>
    public string? To { get; set; }
    
    /// <summary>
    /// Creates a new instance of <see cref="ReplaceTerminalTextPostProcess"/>.
    /// </summary>
    /// <param name="from">The text to be replaced.</param>
    /// <param name="to">The text to replace with.</param>
    public ReplaceTerminalTextPostProcess(string from, string to)
    {
        From = from;
        To = to;
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="ReplaceTerminalTextPostProcess"/>.
    /// </summary>
    /// <param name="from">The text to be replaced.</param>
    /// <param name="toFunc">The function to replace with dynamically.</param>
    public ReplaceTerminalTextPostProcess(string from, Func<string> toFunc)
    {
        From = from;
        ToFunc = toFunc;
    }

    public string Apply(string text)
        => text.Replace(From, To ?? ToFunc?.Invoke() ?? From);
}
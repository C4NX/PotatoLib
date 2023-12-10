namespace PotatoLib.API;

/// <summary>
/// Can be used to post process the terminal text before it is displayed, usually used with <see cref="string"/> replace methods.
/// </summary>
public interface ITerminalTextPostProcess
{
    /// <summary>
    /// Applies the post process to the text, and returns the processed text.
    /// </summary>
    /// <param name="text">The text to be processed.</param>
    /// <returns>The processed text.</returns>
    string Apply(string text);
}
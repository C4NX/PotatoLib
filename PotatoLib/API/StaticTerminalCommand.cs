using System;
using JetBrains.Annotations;
using PotatoLib.Utils;
using UnityEngine;

namespace PotatoLib.API;

public class StaticTerminalCommand : ITerminalCommand
{
    public void Apply(TerminalKeyword keyword, Terminal terminal)
    {
        var terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
        
        terminalNode.displayText = $"{Text}\n";
        terminalNode.clearPreviousText = ClearPreviousText;
        terminalNode.displayTexture = DisplayTexture;
        terminalNode.terminalEvent = $"potato:{Name}";
        
        keyword.specialKeywordResult = terminalNode;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsHidden { get; set; } = false;
    public string Text { get; set;  }
    public bool ClearPreviousText { get; set; } = true;

    public Action<Terminal, TerminalNode> EventAction { get; set; }
    
    [CanBeNull] public Texture DisplayTexture { get; set; }

    public StaticTerminalCommand(string name, string description, string text)
    {
        Name = name;
        Description = description;
        Text = text;
    }
    
    public StaticTerminalCommand(string name)
    {
        Name = name;
        Description = "No description";
        Text = string.Empty;
    }
    
    public StaticTerminalCommand(string name, string description)
    {
        Name = name;
        Description = description;
        Text = string.Empty;
    }
    
    public StaticTerminalCommand WithText(string text, bool clearPreviousText = true)
    {
        Text = text;
        ClearPreviousText = clearPreviousText;
        return this;
    }
    
    public StaticTerminalCommand WithTexture(Texture texture)
    {
        DisplayTexture = texture;
        return this;
    }
    
    /// <summary>
    /// Loads the texture from the given url. Warning: This is a blocking operation and the texture for now is not cached.
    /// </summary>
    /// <param name="textureUrl">The url to load the texture from.</param>
    /// <returns>The current instance of the command.</returns>
    public StaticTerminalCommand WithTextureUrl(string textureUrl)
    {
        DisplayTexture = TextureLoaderUtils.LoadSyncTextureFromUrl(textureUrl);
        return this;
    }

    public StaticTerminalCommand WithAction(Action<Terminal, TerminalNode> eventAction)
    {
        EventAction = eventAction;
        return this;
    }
    
    public StaticTerminalCommand WithActionAndOnlyAction(Action<Terminal, TerminalNode> eventAction)
    {
        EventAction = eventAction;
        Text = string.Empty;
        ClearPreviousText = false;
        return this;
    }

    public StaticTerminalCommand WithHidden(bool isHidden = true)
    {
        IsHidden = isHidden;
        return this;
    }
    
    public StaticTerminalCommand WithDescription(string description)
    {
        Description = description;
        return this;
    }
}
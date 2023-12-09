using System;
using System.Net.Http;
using UnityEngine;

namespace PotatoLib.Utils;

public static class TextureLoaderUtils
{
    public static Texture LoadSyncTexture(string path, Texture defaultTexture = null)
    {
        var bytes = System.IO.File.ReadAllBytes(path);
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        try
        {
            texture.LoadImage(bytes);
            texture.Apply();
            return texture;
        }
        catch(Exception e)
        {
            PotatoPlugin.Instance?.PluginLogger.LogError($"Failed to load texture from path: {path}\n{e}");
            return defaultTexture ?? texture;
        }
    }
    
    public static Texture LoadSyncTextureFromUrl(string url, Texture defaultTexture = null)
    {
        var client = new HttpClient(new HttpClientHandler());
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        try
        {
            texture.LoadImage(client.GetByteArrayAsync(url).GetAwaiter().GetResult());
            texture.Apply();
            return texture;
        }
        catch (Exception e)
        {
            PotatoPlugin.Instance?.PluginLogger.LogError($"Failed to load texture from url: {url}\n{e}");
            return defaultTexture ?? texture;
        }
    }
}
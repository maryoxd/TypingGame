using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace TypingGame;

/// <summary>
/// Trieda TextureManager slúži na načítanie a uchovanie textúr.
/// </summary>
public static class TextureManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new();

    /// <summary>
    /// Metóda LoadTexture slúži na načítanie textúry.
    /// </summary>
    /// <param name="name"></param> - Slúži na určenie názvu textúry.
    /// <param name="texture"></param> - Slúži na určenie textúry.
    public static void LoadTexture(string name, Texture2D texture)
    {
        if (!Textures.ContainsKey(name))
        {
            Textures[name] = texture;
        }
    }

    /// <summary>
    /// Metóda GetTexture slúži na získanie textúry.
    /// </summary>
    /// <param name="name"></param> - Slúži na určenie názvu textúry.
    /// <returns></returns>
    public static Texture2D GetTexture(string name)
    {
        return Textures.ContainsKey(name) ? Textures[name] : null;
    }
}
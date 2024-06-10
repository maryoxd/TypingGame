using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TypingGame;

/// <summary>
/// Trieda Car predstavuje auto.
/// </summary>
public class Car
{
    public Texture2D Texture { get; private set; }
    public Vector2 Position { get; private set; }
    private float _speed;

    /// <summary>
    /// Konštruktor triedy Car.
    /// </summary>
    /// <param name="texture"></param> - Slúži na určenie textúry auta.
    /// <param name="startPosition"></param> - Slúži na určenie počiatočnej pozície auta.
    public Car(Texture2D texture, Vector2 startPosition)
    {
        Texture = texture;
        Position = startPosition;
    }

    /// <summary>
    /// Metóda SetSpeed slúži na nastavenie rýchlosti auta.
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    /// <summary>
    /// Metóda GetSpeed slúži na získanie rýchlosti auta.
    /// </summary>
    /// <returns></returns>
    public float GetSpeed()
    {
        return _speed;
    }

    /// <summary>
    /// Metóda Update slúži na aktualizáciu pozície auta.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        Position += new Vector2(_speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
    }

    /// <summary>
    /// Metóda Draw slúži na vykreslenie auta.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.White);
    }
}
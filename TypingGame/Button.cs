using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TypingGame;

/// <summary>
/// Trieda Button predstavuje tlačidlo.
/// </summary>
public class Button
{
    public Rectangle Rectangle { get; private set; }
    public string Text { get; private set; }
    private Color _defaultColor;
    private Color _hoverColor;
    private Color _currentColor;

    /// <summary>
    /// Konštruktor triedy Button.
    /// </summary>
    /// <param name="rectangle"></param> - Slúži na určenie veľkosti a pozície tlačidla.
    /// <param name="text"></param> - Slúži na určenie textu tlačidla.
    /// <param name="defaultColor"></param> - Slúži na určenie farby tlačidla.
    public Button(Rectangle rectangle, string text, Color defaultColor)
    {
        Rectangle = rectangle;
        Text = text;
        _defaultColor = defaultColor;
        _hoverColor = Color.LightGray;
        _currentColor = defaultColor;
    }

    /// <summary>
    /// Metóda Update slúži na aktualizáciu farby tlačidla.
    /// </summary>
    /// <param name="mousePoint"></param>
    public void Update(Point mousePoint)
    {
        if (Rectangle.Contains(mousePoint))
        {
            _currentColor = _hoverColor;
        }
        else
        {
            _currentColor = _defaultColor;
        }
    }

    /// <summary>
    /// Metóda Draw slúži na vykreslenie tlačidla.
    /// </summary>
    /// <param name="spriteBatch"></param> - Slúži na samotné vykreslenie
    /// <param name="font"></param> - Slúži na určenie fontu textu tlačidla.
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        var buttonRectangle = new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        spriteBatch.Draw(TextureManager.GetTexture("button"), buttonRectangle, _currentColor);

        Vector2 textSize = font.MeasureString(Text);
        Vector2 textPosition = new(
            buttonRectangle.X + (buttonRectangle.Width - textSize.X) / 2,
            buttonRectangle.Y + (buttonRectangle.Height - textSize.Y) / 2);
        spriteBatch.DrawString(font, Text, textPosition, Color.Black);
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TypingGame
{
    /// <summary>
    /// Trieda UiController slúži na ovládanie UI.
    /// </summary>
    public class UiController
    {
        private Button _continueButton;
        private Button _restartButton;
        private Button _exitButton;
        private Button _backButton;

        public event Action OnContinue;
        public event Action OnRestart;
        public event Action OnExit;
        public event Action OnBackToMenu;

        /// <summary>
        /// Metóda LoadContent slúži na načítanie tlačidiel.
        /// </summary>
        public void LoadContent()
        {
            _continueButton = new Button(new Rectangle(540, 200, 250, 50), "POKRAČOVAŤ", Color.Green);
            _restartButton = new Button(new Rectangle(540, 270, 250, 50), "REŠTART", Color.Orange);
            _exitButton = new Button(new Rectangle(540, 340, 250, 50), "EXIT", Color.Red);
            _backButton = new Button(new Rectangle(540, 410, 250, 50), "NASPAŤ", Color.Blue);
        }

        /// <summary>
        /// Metóda UpdatePauseScreen slúži na aktualizáciu tlačidiel.
        /// </summary>
        /// <param name="isFinished"></param> - Slúži na určenie, či je hra ukončená.
        public void UpdatePauseScreen(bool isFinished)
        {
            var mouseState = Mouse.GetState();
            Point mousePoint = new(mouseState.X, mouseState.Y);

            _continueButton.Update(mousePoint);
            _restartButton.Update(mousePoint);
            _exitButton.Update(mousePoint);
            _backButton.Update(mousePoint);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!isFinished && _continueButton.Rectangle.Contains(mousePoint))
                {
                    OnContinue?.Invoke();
                }
                else if (_restartButton.Rectangle.Contains(mousePoint))
                {
                    OnRestart?.Invoke();
                }
                else if (_exitButton.Rectangle.Contains(mousePoint))
                {
                    OnExit?.Invoke();
                }
                else if (_backButton.Rectangle.Contains(mousePoint))
                {
                    OnBackToMenu?.Invoke();
                }
            }
        }

        /// <summary>
        /// Metóda DrawPauseScreen slúži na vykreslenie tlačidiel pri pauznutej hre.
        /// </summary>
        /// <param name="spriteBatch"></param> - Slúži na samotné vykreslenie.
        /// <param name="font"></param> - Slúži na určenie fontu textu tlačidla.
        /// <param name="isFinished"></param> - Slúži na určenie, či je hra ukončená.
        public void DrawPauseScreen(SpriteBatch spriteBatch, SpriteFont font, bool isFinished)
        {
            if (isFinished)
            {
                _restartButton.Draw(spriteBatch, font);
                _exitButton.Draw(spriteBatch, font);
                _backButton.Draw(spriteBatch, font);
            }
            else
            {
                _continueButton.Draw(spriteBatch, font);
                _restartButton.Draw(spriteBatch, font);
                _exitButton.Draw(spriteBatch, font);
                _backButton.Draw(spriteBatch, font);
            }
        }
    }
}

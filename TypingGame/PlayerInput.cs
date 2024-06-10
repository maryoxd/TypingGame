using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TypingGame
{
    /// <summary>
    /// Trieda PlayerInput reprezentuje vstup od hráča.
    /// </summary>
    public class PlayerInput
    {
        private readonly int _numPlayers;
        private readonly string _difficulty;
        private readonly TextTask _textTask;
        private string _currentSentence;
        private string _userInput = "";
        private int _currentCharIndex;
        private int _errorCount;
        private bool _isError;
        private Keys _lastKey;

        /// <summary>
        /// Konštruktor triedy PlayerInput.
        /// </summary>
        /// <param name="numPlayers"></param> - Slúži na určenie počtu hráčov.
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti hry.
        /// <param name="useRandomizer"></param> - Slúži na určenie, či sa má použiť randomizér.
        public PlayerInput(int numPlayers, string difficulty, bool useRandomizer)
        {
            _numPlayers = numPlayers;
            _difficulty = difficulty;
            _textTask = new TextTask(difficulty, useRandomizer);
            _currentSentence = _textTask.GetRandomSentence();
        }

        /// <summary>
        /// Metóda GetNumPlayers slúži na získanie počtu hráčov.
        /// </summary>
        /// <returns></returns>
        public int GetNumPlayers() => _numPlayers;

        /// <summary>
        /// Metóda GetDifficulty slúži na získanie obtiažnosti hry.
        /// </summary>
        /// <returns></returns>
        public string GetDifficulty() => _difficulty;

        /// <summary>
        /// Metóda Update slúži na aktualizáciu hry.
        /// </summary>
        /// <param name="gameTime"></param> - Slúži na získanie času od posledného framu.
        /// <param name="playerCar"></param> - Slúži na získanie auta hráča.
        public void Update(GameTime gameTime, Car playerCar)
        {
            var keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            if (pressedKeys.Length > 0)
            {
                Keys key = pressedKeys[0];
                if (key != _lastKey)
                {
                    char keyChar = KeyToChar(key, keyboardState);

                    if (keyChar != '\0')
                    {
                        if (_currentCharIndex < _currentSentence.Length &&
                            keyChar == _currentSentence[_currentCharIndex])
                        {
                            ProcessCorrectKeyPress(playerCar, keyChar);
                        }
                        else
                        {
                            ProcessIncorrectKeyPress(playerCar);
                        }
                    }

                    _lastKey = key;
                }
            }
            else
            {
                ProcessNoKeyPress(gameTime, playerCar);
                _lastKey = Keys.None;
            }

        }

        /// <summary>
        /// Metóda ProcessCorrectKeyPress slúži na spracovanie správneho stlačenia klávesy.
        /// </summary>
        /// <param name="playerCar"></param> - Slúži na získanie auta hráča.
        /// <param name="keyChar"></param> - Slúži na získanie stlačenej klávesy.
        private void ProcessCorrectKeyPress(Car playerCar, char keyChar)
        {
            _userInput += keyChar;
            _currentCharIndex++;
            _isError = false;

            float newSpeed = playerCar.GetSpeed() + 10f;
            playerCar.SetSpeed(MathHelper.Clamp(newSpeed, 0f, 380f));

            if (_currentCharIndex == _currentSentence.Length)
            {
                _currentSentence = _textTask.GetRandomSentence();
                _userInput = "";
                _currentCharIndex = 0;
            }
        }

        /// <summary>
        /// Metóda ProcessIncorrectKeyPress slúži na spracovanie nesprávneho stlačenia klávesy.
        /// </summary>
        /// <param name="playerCar"></param> - Slúži na získanie auta hráča.
        private void ProcessIncorrectKeyPress(Car playerCar)
        {
            _isError = true;
            _errorCount++;
            float newSpeed = playerCar.GetSpeed() - 5f;
            playerCar.SetSpeed(MathHelper.Clamp(newSpeed, 0f, 380f));
        }

        private static void ProcessNoKeyPress(GameTime gameTime, Car playerCar)
        {
            float decrement = new Random().Next(30, 60) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float newSpeed = playerCar.GetSpeed() - decrement;
            playerCar.SetSpeed(MathHelper.Clamp(newSpeed, 0f, 380f));
        }

        /// <summary>
        /// Metóda DrawText slúži na vykreslenie textu.
        /// </summary>
        /// <param name="spriteBatch"></param> - Slúži na samotné vykreslenie.
        /// <param name="cameraPosition"></param> - Slúži na určenie pozície kamery.
        /// <param name="font"></param> - Slúži na určenie fontu textu.
        /// <param name="cars"></param> - Slúži na získanie vozidiel.
        /// <param name="font2"></param> - Slúži na určenie fontu textu.
        public void DrawText(SpriteBatch spriteBatch, Vector2 cameraPosition, SpriteFont font, List<Car> cars, SpriteFont font2)
        {
            string sentenceLabel = "VETA: ";
            string fullSentence = sentenceLabel + _currentSentence;
            Vector2 sentenceSize = font.MeasureString(fullSentence);
            Vector2 sentencePosition = new(cameraPosition.X + 40 + (1280 - sentenceSize.X) / 2, 15);
            spriteBatch.DrawString(font, fullSentence, sentencePosition, Color.White);

            string inputLabel = "VY:";
            Vector2 inputLabelSize = font.MeasureString(inputLabel);
            Vector2 inputPosition = new(cameraPosition.X + 80 + (1280 - sentenceSize.X) / 2, 70);
            spriteBatch.DrawString(font, inputLabel, inputPosition, Color.Yellow);

            Vector2 userInputPosition = new(inputPosition.X + inputLabelSize.X + 10, 70);
            spriteBatch.DrawString(font, _userInput, userInputPosition, Color.Yellow);

            if (_currentCharIndex < _currentSentence.Length)
            {
                Vector2 currentCharPosition = new(
                    sentencePosition.X + font.MeasureString(fullSentence[..(sentenceLabel.Length + _currentCharIndex)]).X,
                    15);
                Color highlightColor = _isError ? Color.Red : Color.Yellow;
                spriteBatch.DrawString(font, _currentSentence[_currentCharIndex].ToString(), currentCharPosition, highlightColor);
            }

            string errorText = $"CHYBY: {_errorCount}";
            Vector2 errorTextPosition = new(cameraPosition.X + 10, 10);
            spriteBatch.DrawString(font2, errorText, errorTextPosition, Color.Red);

            float speedY = 20;
            string playerSpeed = $"VY: {cars[0].GetSpeed():F1}";
            Vector2 playerSpeedPosition = new(cameraPosition.X + 10, speedY + 20);
            spriteBatch.DrawString(font2, playerSpeed, playerSpeedPosition, Color.Yellow);

            if (_numPlayers >= 2)
            {
                string car2Speed = $"AUTO2: {cars[1].GetSpeed():F1}";
                Vector2 car2SpeedPosition = new(cameraPosition.X + 10, speedY + 50);
                spriteBatch.DrawString(font2, car2Speed, car2SpeedPosition, Color.Aqua);
            }

            if (_numPlayers == 3)
            {
                string car3Speed = $"AUTO3: {cars[2].GetSpeed():F1}";
                Vector2 car3SpeedPosition = new(cameraPosition.X + 10, speedY + 80);
                spriteBatch.DrawString(font2, car3Speed, car3SpeedPosition, Color.Green);
            }
        }

        /// <summary>
        /// Metóda KeyToChar slúži na prevod klávesy na znak.
        /// </summary>
        /// <param name="key"></param> - Slúži na získanie stlačenej klávesy.
        /// <param name="keyboardState"></param> - Slúži na získanie stavu klávesnice.
        /// <returns></returns>
        private static char KeyToChar(Keys key, KeyboardState keyboardState)
        {
            bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            bool capsLock = Console.CapsLock; 
            bool altGr = keyboardState.IsKeyDown(Keys.RightAlt);

            if (altGr)
            {
                if (key == Keys.OemQuotes)
                {
                    return '\'';
                }
            }

            if (key >= Keys.A && key <= Keys.Z)
            {
                if (shift || capsLock)
                {
                    return (char)('A' + (key - Keys.A));
                }
                else
                {
                    return (char)('a' + (key - Keys.A));
                }
            }

            if (key >= Keys.D0 && key <= Keys.D9)
            {
                if (shift)
                {
                    return key == Keys.D1 ? '!' :
                        key == Keys.D2 ? '@' :
                        key == Keys.D3 ? '#' :
                        key == Keys.D4 ? '$' :
                        key == Keys.D5 ? '%' :
                        key == Keys.D6 ? '^' :
                        key == Keys.D7 ? '&' :
                        key == Keys.D8 ? '*' :
                        key == Keys.D9 ? '(' :
                        ')';
                }

                return (char)('0' + (key - Keys.D0));
            }

            return key switch
            {
                Keys.OemPeriod => '.',
                Keys.OemComma => ',',
                Keys.OemQuestion => shift ? '?' : '/',
                Keys.OemSemicolon => shift ? ':' : ';',
                Keys.OemQuotes => shift ? '"' : '\'',
                Keys.OemOpenBrackets => shift ? '{' : '[',
                Keys.OemCloseBrackets => shift ? '}' : ']',
                Keys.OemMinus => shift ? '_' : '-',
                Keys.OemPlus => shift ? '+' : '=',
                Keys.OemBackslash => shift ? '|' : '\\',
                Keys.Space => ' ',
                _ => '\0',
            };
        }
    }
}

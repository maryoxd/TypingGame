using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TypingGame
{
    /// <summary>
    /// Trieda GameScreen je druhá najdôležitejšia trieda ktorá sa stará o priebeh hry, a spája všetky komponenty hry.
    /// </summary>
    public class GameScreen
    {
        private Texture2D _trackTexture; 
        private Texture2D _finishLineTexture; 
        private readonly List<Car> _cars = new();
        private readonly float _trackLength;
        private Vector2 _cameraPosition = Vector2.Zero;

        private SpriteFont _font;
        private SpriteFont _font2;
        private SpriteFont _font3;

        private Color _winnerColor;

        private bool _isCountingDown = true;
        private double _countdownTime = 3.0; 
        private bool _isRaceFinished;
        private string _winner = "";
        private bool _isPaused;

        private bool _tabWasPressed;

        private readonly PlayerInput _playerInput;
        private readonly BotController _botController;
        private readonly UiController _uiController;

        public event Action RestartRace;
        public event Action ExitGame;
        public event Action BackToMenu;

        /// <summary>
        /// Konštruktor triedy GameScreen.
        /// </summary>
        /// <param name="numPlayers"></param> - Slúži na určenie počtu hráčov.
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti botov.
        /// <param name="useRandomizer"></param> - Slúži na určenie použitia randomizéra.
        public GameScreen(int numPlayers, string difficulty, bool useRandomizer)
        {

            BotDifficulty botDifficulty = difficulty.ToLower() switch
            {
                "easy" => BotDifficulty.Easy,
                "medium" => BotDifficulty.Medium,
                "hard" => BotDifficulty.Hard,
                _ => BotDifficulty.Medium
            };
            _playerInput = new PlayerInput(numPlayers, difficulty, useRandomizer);
            _botController = new BotController(numPlayers, botDifficulty);
            _uiController = new UiController();

            _uiController.OnContinue += () => _isPaused = false;
            _uiController.OnRestart += () => RestartRace?.Invoke();
            _uiController.OnExit += () => ExitGame?.Invoke();
            _uiController.OnBackToMenu += () => BackToMenu?.Invoke();

            if (numPlayers == 1)
            {
                _trackLength = 999999f; // Nekonečná trať pre solo režim
            }
            else
            {
                _trackLength = 5000f * 2.0f; // Trať pre viac hráčov
            }
        }
        /// <summary>
        /// Metóda GetNumPlayers slúži na získanie počtu hráčov.
        /// </summary>
        /// <returns></returns>
        public int GetNumPlayers() => _playerInput.GetNumPlayers();

        /// <summary>
        /// Metóda GetDifficulty slúži na získanie obtiažnosti hry.
        /// </summary>
        /// <returns></returns>
        public string GetDifficulty() => _playerInput.GetDifficulty();

        /// <summary>
        /// Metóda LoadContent slúži na načítanie assetov (obrázkov) pre hru.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            _trackTexture = content.Load<Texture2D>("track3"); 
            _finishLineTexture = content.Load<Texture2D>("finishh"); 
            _font = content.Load<SpriteFont>("Arial");
            _font2 = content.Load<SpriteFont>("ArialSmall");
            _font3 = content.Load<SpriteFont>("ArialBig");

            var racecar1Texture = content.Load<Texture2D>("racecar1");
            var playerCar = new Car(racecar1Texture, new Vector2(0, 530));
            _cars.Add(playerCar);

            _botController.LoadContent(content, _cars);
            _uiController.LoadContent();
        }

        /// <summary>
        /// Metóda Update slúži na aktualizáciu hry.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (_isCountingDown)
            {
                UpdateCountdown(gameTime);
            }
            else if (_isRaceFinished)
            {
                _uiController.UpdatePauseScreen(true);
            }
            else
            {
                HandlePause();
                if (_isPaused)
                {
                    _uiController.UpdatePauseScreen(false);
                }
                else
                {
                    _playerInput.Update(gameTime, _cars[0]);
                    _botController.Update(gameTime);

                    foreach (var car in _cars)
                    {
                        car.Update(gameTime);
                    }

                    if (_playerInput.GetNumPlayers() > 1)
                    {
                        CheckForFinish();
                    }

                    UpdateCameraPosition(gameTime);
                }
            }
        }

        /// <summary>
        /// Metóda UpdateCountdown slúži na aktualizáciu odpočítavania pred začiatkom hry.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCountdown(GameTime gameTime)
        {
            _countdownTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_countdownTime <= 0)
            {
                _isCountingDown = false;
            }
        }

        /// <summary>
        /// Metóda HandlePause slúži na spracovanie stlačenia klávesy TAB.
        /// </summary>
        private void HandlePause()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Tab) && !_tabWasPressed)
            {
                _isPaused = !_isPaused;
                _tabWasPressed = true;
            }
            else if (keyboardState.IsKeyUp(Keys.Tab))
            {
                _tabWasPressed = false;
            }
        }

        /// <summary>
        /// Metóda UpdateCameraPosition slúži na aktualizáciu pozície kamery.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCameraPosition(GameTime gameTime)
        {
            float carPositionX = _cars[0].Position.X;
            float targetCameraX = carPositionX - 640;
            float cameraSpeed = 2f;

            _cameraPosition.X += (targetCameraX - _cameraPosition.X) * cameraSpeed *
                                 (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_cameraPosition.X < 0)
            {
                _cameraPosition.X = 0;
            }
        }

        /// <summary>
        /// Metóda CheckForFinish slúži na kontrolu ukončenia hry.
        /// </summary>
        private void CheckForFinish()
        {
            foreach (var car in _cars)
            {
                if (car.Position.X >= _trackLength - _finishLineTexture.Width)
                {
                    _isRaceFinished = true;
                    if (car == _cars[0])
                    {
                        _winner = "VYHRAL SI";
                        _winnerColor = Color.Green;
                    }
                    else if (_cars.Count > 1 && car == _cars[1])
                    {
                        _winner = "AUTO2 VYHRALO!";
                        _winnerColor = Color.Red;
                    }
                    else if (_cars.Count > 2 && car == _cars[2])
                    {
                        _winner = "AUTO3 VYHRALO!";
                        _winnerColor = Color.Red;
                    }
                    _isPaused = true; 
                    break;
                }
            }
        }

        /// <summary>
        /// Metóda Draw slúži na vykreslenie hry.
        /// </summary>
        /// <param name="graphicsDevice"></param> - Slúži na prekreslenie obrazovky.
        /// <param name="spriteBatch"></param> - Slúži na vykreslenie textúr.
        public void Draw(GraphicsDevice graphicsDevice ,SpriteBatch spriteBatch)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(
                transformMatrix: Matrix.CreateTranslation(new Vector3(-_cameraPosition.X, -_cameraPosition.Y, 0)));

            DrawTrack(spriteBatch);
            DrawCars(spriteBatch);
            _playerInput.DrawText(spriteBatch, _cameraPosition, _font, _cars, _font2);
            DrawCountdown(spriteBatch);
            DrawWinner(spriteBatch);

            spriteBatch.End();

            if (_isPaused || _isRaceFinished)
            {
                spriteBatch.Begin();
                _uiController.DrawPauseScreen(spriteBatch, _font, _isRaceFinished);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Metóda DrawWinner slúži na vykreslenie víťaza hry.
        /// </summary>
        /// <param name="spriteBatch"></param> - Slúži na vykreslenie textu.
        private void DrawWinner(SpriteBatch spriteBatch)
        {
            if (_isRaceFinished)
            {
                Vector2 textSize = _font.MeasureString(_winner);
                Vector2 textPosition = new(_cameraPosition.X - 40 + (1280 - textSize.X) / 2 , 150);
                spriteBatch.DrawString(_font3, _winner, textPosition, _winnerColor);
            }
        }

        /// <summary>
        /// Metóda DrawTrack slúži na vykreslenie trate.
        /// </summary>
        /// <param name="spriteBatch"></param> - Slúži na vykreslenie textúr.
        private void DrawTrack(SpriteBatch spriteBatch)
        {
            for (int i = (int)_cameraPosition.X; i < _trackLength - _finishLineTexture.Width; i += _trackTexture.Width)
            {
                spriteBatch.Draw(_trackTexture, new Rectangle(i, 0, _trackTexture.Width, 720), Color.White);
            }

            if (_playerInput.GetNumPlayers() > 1)
            {
                spriteBatch.Draw(_finishLineTexture,
                    new Rectangle((int)(_trackLength - _finishLineTexture.Width), 0, _finishLineTexture.Width, 720),
                    Color.White);
            }
        }

        /// <summary>
        /// Metóda DrawCars slúži na vykreslenie áut.
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawCars(SpriteBatch spriteBatch)
        {
            foreach (var car in _cars)
            {
                car.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Metóda DrawCountdown slúži na vykreslenie odpočítavania pred začiatkom hry.
        /// </summary>
        /// <param name="spriteBatch"></param> - Slúži na vykreslenie textu.
        private void DrawCountdown(SpriteBatch spriteBatch)
        {
            if (_isCountingDown)
            {
                string countdownText = ((int)Math.Ceiling(_countdownTime)).ToString();
                Vector2 textSize = _font.MeasureString(countdownText);
                Vector2 textPosition = new(_cameraPosition.X + (1280 - textSize.X) / 2, 360);
                spriteBatch.DrawString(_font3, countdownText, textPosition, Color.White);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TypingGame
{
    /// <summary>
    /// Trieda Game1 predstavuje hlavnú triedu hry.
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;

        private Texture2D _buttonTexture;
        private Rectangle _startButtonRect;
        private Rectangle _soloButtonRect;
        private Rectangle _exitButtonRect;
        private Rectangle _difficultyButtonRect;
        private Rectangle _randomizerButtonRect;
        private MouseState _previousMouseState;
        private SpriteFont _font;

        private int _soloButtonClickCount;
        private string _soloButtonText = "SOLO";
        private Color _soloButtonColor = new(0x1D, 0x65, 0xC7); // Modrá: #1D65C7

        private int _difficultyButtonClickCount;
        private string _difficultyButtonText = "EASY";
        private Color _difficultyButtonColor = new(0x29, 0xB1, 0x59); // Zelená: #29B159

        private bool _randomizerEnabled;
        private Color _randomizerButtonColor = new(0xDD, 0x4B, 0x48); // Červená: #DD4B48 (vypnuté)

        private bool _isGameScreenActive;
        private GameScreen _gameScreen;

        /// <summary>
        /// Konštruktor triedy Game1.
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Nastavenie veľkosti okna
            _graphics.PreferredBackBufferWidth = 1280; // Šírka okna
            _graphics.PreferredBackBufferHeight = 720; // Výška okna
            _graphics.ApplyChanges();
        }
        /// <summary>
        /// Metóda Initialize slúži na inicializáciu hry.
        /// </summary>
        protected override void Initialize()
        {
            // Nastavenie pozícií tlačidiel
            int buttonWidth = 200;
            int buttonHeight = 50;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            // Posun tlačidiel viac dole
            _startButtonRect = new Rectangle((screenWidth - buttonWidth) / 2, screenHeight / 2 + 100, buttonWidth, buttonHeight);
            _soloButtonRect = new Rectangle((screenWidth - buttonWidth) / 2, screenHeight / 2 + 160, buttonWidth, buttonHeight);
            _difficultyButtonRect = new Rectangle((screenWidth - buttonWidth) / 2 - 130, screenHeight / 2 + 220, buttonWidth + 20, buttonHeight);
            _randomizerButtonRect = new Rectangle((screenWidth - buttonWidth) / 2 + 130, screenHeight / 2 + 220, buttonWidth + 20, buttonHeight);
            _exitButtonRect = new Rectangle((screenWidth - buttonWidth) / 2, screenHeight / 2 + 280, buttonWidth, buttonHeight);

            base.Initialize();
        }

        /// <summary>
        /// Metóda LoadContent slúži na načítanie obsahu hry.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("mainscreen_background");
            _buttonTexture = new Texture2D(GraphicsDevice, 1, 1);
            _buttonTexture.SetData(new[] { Color.White });
            _font = Content.Load<SpriteFont>("Arial");

            // Načítanie textúr pre tlačidlá
            TextureManager.LoadTexture("button", _buttonTexture);
        }

        /// <summary>
        /// Metóda Update slúži na aktualizáciu hry, vrátane eventov a aktualizácií ostatných tried.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (_isGameScreenActive)
            {
                _gameScreen.Update(gameTime);
                return;
            }

            var mouseState = Mouse.GetState();
            Point mousePoint = new(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                if (_startButtonRect.Contains(mousePoint))
                {
                    // Prepnutie na obrazovku hry
                    int numPlayers = _soloButtonClickCount + 1;
                    string difficulty = _difficultyButtonText.ToLower();
                    _gameScreen = new GameScreen(numPlayers, difficulty, _randomizerEnabled);
                    _gameScreen.LoadContent(Content); 
                    _gameScreen.RestartRace += RestartGame;
                    _gameScreen.ExitGame += ExitGame;
                    _gameScreen.BackToMenu += BackToMenu;
                    _isGameScreenActive = true;
                }
                else if (_soloButtonRect.Contains(mousePoint))
                {
                    _soloButtonClickCount = (_soloButtonClickCount + 1) % 3;

                    switch (_soloButtonClickCount)
                    {
                        case 0:
                            _soloButtonText = "SOLO";
                            _soloButtonColor = new Color(0x1D, 0x65, 0xC7); // Pôvodná farba
                            break;
                        case 1:
                            _soloButtonText = "2 HRÁČI";
                            _soloButtonColor = new Color(0x61, 0x9E, 0xAA); // Farba pre 2 hráčov
                            break;
                        case 2:
                            _soloButtonText = "3 HRÁČI";
                            _soloButtonColor = new Color(0x61, 0x9E, 0xE2); // Farba pre 3 hráčov
                            break;
                    }
                }
                else if (_difficultyButtonRect.Contains(mousePoint))
                {
                    _difficultyButtonClickCount = (_difficultyButtonClickCount + 1) % 3;

                    switch (_difficultyButtonClickCount)
                    {
                        case 0:
                            _difficultyButtonText = "EASY";
                            _difficultyButtonColor = new Color(0x29, 0xB1, 0x59); // Zelená: #29B159
                            break;
                        case 1:
                            _difficultyButtonText = "MEDIUM";
                            _difficultyButtonColor = new Color(0xFF, 0xD7, 0x00); // Žltá: #FFD700
                            break;
                        case 2:
                            _difficultyButtonText = "HARD";
                            _difficultyButtonColor = new Color(0xDD, 0x4B, 0x48); // Červená: #DD4B48
                            break;
                    }
                }
                else if (_randomizerButtonRect.Contains(mousePoint))
                {
                    _randomizerEnabled = !_randomizerEnabled;
                    _randomizerButtonColor = _randomizerEnabled ? new Color(0x29, 0xB1, 0x59) : new Color(0xDD, 0x4B, 0x48); // Zelená: #29B159 (zapnuté) alebo Červená: #DD4B48 (vypnuté)
                }
                else if (_exitButtonRect.Contains(mousePoint))
                {
                    Exit();
                }
            }

            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Metóda RestartGame slúži na reštartovanie hry.
        /// </summary>
        private void RestartGame()
        {
            _isGameScreenActive = false;

            int numPlayers = _gameScreen.GetNumPlayers();
            string difficulty = _gameScreen.GetDifficulty();

            _gameScreen = new GameScreen(numPlayers, difficulty, _randomizerEnabled);
            _gameScreen.LoadContent(Content);
            _gameScreen.RestartRace += RestartGame;
            _gameScreen.ExitGame += ExitGame;
            _gameScreen.BackToMenu += BackToMenu;
            _isGameScreenActive = true;
        }

        /// <summary>
        /// Metóda ExitGame slúži na ukončenie hry.
        /// </summary>
        private void ExitGame()
        {
            Exit();
        }

        /// <summary>
        /// Metóda BackToMenu slúži na návrat do hlavného menu.
        /// </summary>
        private void BackToMenu()
        {
            _isGameScreenActive = false;
            Initialize();
        }

        /// <summary>
        /// Metóda Draw slúži na vykreslenie hry a jej komponentov.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_isGameScreenActive)
            {
                _gameScreen.Draw(GraphicsDevice,_spriteBatch);
            }
            else
            {
                var mouseState = Mouse.GetState();
                Point mousePoint = new(mouseState.X, mouseState.Y);

                _spriteBatch.Begin();

                _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

                DrawButton(_startButtonRect, "ŠTART", new Color(0x29, 0xB1, 0x59), _startButtonRect.Contains(mousePoint)); // Zelená: #29B159
                DrawButton(_soloButtonRect, _soloButtonText, _soloButtonColor, _soloButtonRect.Contains(mousePoint)); // Dynamický text a farba
                DrawButton(_difficultyButtonRect, _difficultyButtonText, _difficultyButtonColor, _difficultyButtonRect.Contains(mousePoint)); // Dynamický text a farba pre obtiažnosť
                DrawButton(_randomizerButtonRect, "RANDOMIZER", _randomizerButtonColor, _randomizerButtonRect.Contains(mousePoint)); // Farba pre RANDOMIZER
                DrawButton(_exitButtonRect, "KONIEC", new Color(0xDD, 0x4B, 0x48), _exitButtonRect.Contains(mousePoint)); // Červená: #DD4B48

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Metóda DrawButton slúži na vykreslenie tlačidiel.
        /// </summary>
        /// <param name="buttonRect"></param> - Slúži na určenie veľkosti a pozície tlačidla.
        /// <param name="buttonText"></param> - Slúži na určenie textu tlačidla.
        /// <param name="buttonColor"></param> - Slúži na určenie farby tlačidla.
        /// <param name="isHovered"></param> - Slúži na určenie, či je myš nad tlačidlom.
        private void DrawButton(Rectangle buttonRect, string buttonText, Color buttonColor, bool isHovered)
        {
            Color drawColor = isHovered ? Color.LightGray : buttonColor;
            _spriteBatch.Draw(_buttonTexture, buttonRect, drawColor);
            Vector2 textSize = _font.MeasureString(buttonText); 
            Vector2 textPosition = new(
                buttonRect.X + (buttonRect.Width - textSize.X) / 2,
                buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);
            _spriteBatch.DrawString(_font, buttonText, textPosition, Color.Black);
        }
    }
}

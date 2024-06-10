using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace TypingGame
{
    /// <summary>
    /// Trieda BotController slúži na riadenie botov v hre.
    /// </summary>
    public class BotController
    {
        private readonly int _numPlayers;
        private readonly BotDifficulty _difficulty;
        private readonly Dictionary<Car, BotMode> _carBotModes = new();
        private TimeSpan _lastBotModeChangeTime;
        private readonly TimeSpan _botModeChangeInterval = TimeSpan.FromSeconds(3);
        public readonly TimeSpan InitialAccelerationTime = TimeSpan.FromSeconds(10);
        private TimeSpan _elapsedTime;
        private readonly List<BotMode> _botModes = new() { BotMode.Slower, BotMode.Equal, BotMode.Faster };
        private readonly Random _random = new();

        /// <summary>
        /// Konštruktor triedy BotController.
        /// </summary>
        /// <param name="numPlayers"></param> - Slúži na určenie počtu hráčov.
        /// <param name="difficulty"></param> - Slúži na určenie obtiažnosti botov.
        public BotController(int numPlayers, BotDifficulty difficulty)
        {
            _numPlayers = numPlayers;
            _difficulty = difficulty;
        }

        /// <summary>
        /// Metóda LoadContent slúži na načítanie assetov (obrázkov) vozidiel botov a inicializáciu ich počiatočných rýchlostí
        /// </summary>
        /// <param name="content"></param>
        /// <param name="cars"></param>
        public void LoadContent(ContentManager content, List<Car> cars)
        {
            if (_numPlayers >= 2)
            {
                var botCar2 = new Car(content.Load<Texture2D>("racecar2"), new Vector2(0, 340));
                _carBotModes[botCar2] = BotMode.Slower;
                botCar2.SetSpeed(_random.Next(20,101));
                cars.Add(botCar2);
            }

            if (_numPlayers == 3)
            {
                var botCar3 = new Car(content.Load<Texture2D>("racecar3"), new Vector2(0, 140));
                _carBotModes[botCar3] = BotMode.Slower;
                botCar3.SetSpeed(_random.Next(20,101));
                cars.Add(botCar3);
            }
        }

        /// <summary>
        /// Metóda Update slúži na aktualizáciu rýchlosti botov v hre.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime < InitialAccelerationTime)
            {
                foreach (var botCar in _carBotModes.Keys)
                {
                    float targetSpeed = GetBotSpeed(_carBotModes[botCar]);
                    float acceleration = (targetSpeed - botCar.GetSpeed()) / (float)InitialAccelerationTime.TotalSeconds;
                    float newSpeed = botCar.GetSpeed() + acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    botCar.SetSpeed(newSpeed);
                }
            }
            else
            {
                if (gameTime.TotalGameTime - _lastBotModeChangeTime > _botModeChangeInterval)
                {
                    foreach (var botCar in _carBotModes.Keys)
                    {
                        _carBotModes[botCar] = GetRandomBotMode();
                    }

                    _lastBotModeChangeTime = gameTime.TotalGameTime;
                }

                foreach (var botCar in _carBotModes.Keys)
                {
                    float newSpeed = GetBotSpeed(_carBotModes[botCar]);
                    botCar.SetSpeed(newSpeed);
                }
            }
        }

        /// <summary>
        /// Metóda GetBotSpeed slúži na pridelenie rýchlostí botom v hre.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private float GetBotSpeed(BotMode mode)
        {
            return _difficulty switch
            {
                BotDifficulty.Easy => mode switch
                {
                    BotMode.Slower => _random.Next(150, 199),
                    BotMode.Equal => _random.Next(200, 249),
                    BotMode.Faster => _random.Next(250, 299),
                    _ => 150
                },
                BotDifficulty.Medium => mode switch
                {
                    BotMode.Slower => _random.Next(200, 249),
                    BotMode.Equal => _random.Next(250, 299),
                    BotMode.Faster => _random.Next(300, 349),
                    _ => 200
                },
                BotDifficulty.Hard => mode switch
                {
                    BotMode.Slower => _random.Next(250, 299),
                    BotMode.Equal => _random.Next(300, 349),
                    BotMode.Faster => _random.Next(350, 400),
                    _ => 250
                },
                _ => 200
            };
        }

        /// <summary>
        /// Metóda GetRandomBotMode slúži na náhodné pridelenie rýchlosti botov v hre.
        /// </summary>
        /// <returns></returns>
        private BotMode GetRandomBotMode()
        {
            return _botModes[_random.Next(_botModes.Count)];
        }
    }
}
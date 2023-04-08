using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Constants;
using PaddleBounceBlockBreak.HUD;
using System;

namespace PaddleBounceBlockBreak
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // non-static?
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static Random Random;

        private Level _level;
        private DynamicHudText _scoreOverlay;
        private int _totalScore;
        private int _lives;
        private GameState _gameState;

        private HudText _gameOverOverlay;
        private DynamicHudText _livesOverlay;
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;
            Random = new Random();

            _lives = 3; // Start with 3 lives
            _gameState = GameState.GAME_ACTIVE;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _scoreOverlay = new DynamicHudText(Content.Load<SpriteFont>("HudFont"), "Score: ", new Vector2(10, 0));
            _livesOverlay = new DynamicHudText(Content.Load<SpriteFont>("HudFont"), "Lives: ", new Vector2(10, _scoreOverlay.Size.Y));
            _gameOverOverlay = new CentredHudText(Content.Load<SpriteFont>("HudFont"), "Game Over!", new Vector2(ScreenWidth/2, ScreenHeight/2));

            LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            if (_level != null)
            {
                _totalScore += _level.LevelScore;
                _level.Dispose();
            }

            _level = new Level(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (_gameState == GameState.GAME_ACTIVE)
            {
                _level.Update(gameTime);

                if (_level.LevelState == LevelState.LEVEL_FAIL)
                {
                    _lives -= 1;
                    if (_lives == 0)
                    {
                        _gameState = GameState.GAME_OVER;
                    }
                    else
                    {
                        _level.Reset();
                    }
                }

                if (_level.LevelState == LevelState.LEVEL_COMPLETE)
                {
                    LoadNextLevel();
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            _spriteBatch.Begin();

            _level.Draw(gameTime, _spriteBatch);

            _scoreOverlay.Draw(_spriteBatch, (_totalScore + _level.LevelScore).ToString());
            _livesOverlay.Draw(_spriteBatch, _lives.ToString());

            if (_gameState == GameState.GAME_OVER)
            {
                _gameOverOverlay.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
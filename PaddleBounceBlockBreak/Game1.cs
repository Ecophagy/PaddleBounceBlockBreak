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
        private Score _score;
        private int _totalScore;
        private GameState _gameState;

        private HudText _gameOverOverlay;
        

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

            _gameState = GameState.GAME_ACTIVE;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _score = new Score(Content.Load<SpriteFont>("ScoreFont"));
            _gameOverOverlay = new HudText(Content.Load<SpriteFont>("ScoreFont"), "Game Over!", new Vector2(ScreenWidth/2, ScreenHeight/2));

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

            _level.Update(gameTime);

            if(_level.LevelState == LevelState.LEVEL_FAIL)
            {
                _gameState = GameState.GAME_OVER;
            }

            if (_level.LevelState == LevelState.LEVEL_COMPLETE)
            {
                LoadNextLevel();
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            _spriteBatch.Begin();

            _level.Draw(gameTime, _spriteBatch);

            _score.Draw(_spriteBatch, _totalScore + _level.LevelScore);

            if (_gameState == GameState.GAME_OVER)
            {
                _gameOverOverlay.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Models;
using PaddleBounceBlockBreak.Sprites;
using System;
using System.Collections.Generic;

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

        private Score _score;
        private List<Sprite> _sprites;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var batTexture = Content.Load<Texture2D>("bat");
            var ballTexture = Content.Load<Texture2D>("ball");

            _score = new Score(Content.Load<SpriteFont>("ScoreFont"));

            _sprites = new List<Sprite>()
            {
                // TODO: Background goes here
                new Bat(batTexture)
                {
                    Position = new Vector2((ScreenWidth/2) - (batTexture.Width/2), 20),
                    Input = new Input()
                    {
                        Left = Keys.Left,
                        Right = Keys.Right
                    }
                },
                new Ball(ballTexture)
                {
                    Position = new Vector2((ScreenWidth/2) - (ballTexture.Width/2),  (ScreenHeight/2) - (ballTexture.Height/2)),
                }
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            foreach (var sprite in _sprites)
            {
                sprite.Update(gameTime, _sprites);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach(var sprite in _sprites)
            {
                sprite.Draw(_spriteBatch);
            }

            _score.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
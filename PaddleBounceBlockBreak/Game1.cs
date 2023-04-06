using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Models;
using PaddleBounceBlockBreak.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var paddleTexture = Content.Load<Texture2D>("paddle");
            var ballTexture = Content.Load<Texture2D>("ball");
            var blockTexture = Content.Load<Texture2D>("block");

            _score = new Score(Content.Load<SpriteFont>("ScoreFont"));

            _sprites = new List<Sprite>();
            // TODO: Background goes here

            // Add 10 block in a random pattern
            foreach (var _ in Enumerable.Range(0, 10))
            {
                var block = new Block(blockTexture, 10);

                // Randomise block position
                var blockX = Random.Next(0, ScreenWidth - block.Rectangle.Width);
                var ylimit = (ScreenHeight/2) - block.Rectangle.Height; // Limit Y to top half of screen
                var blockY = Random.Next(0, ylimit);

                block.Position = new Vector2(blockX, blockY);

                _sprites.Add(block);
            }

            _sprites.Add(new Paddle(paddleTexture)
            {
                Position = new Vector2((ScreenWidth / 2) - (paddleTexture.Width / 2), ScreenHeight - 40),
                Input = new Input()
                {
                    Left = Keys.Left,
                    Right = Keys.Right
                }
            });
            _sprites.Add(new Ball(ballTexture)
            {
                Position = new Vector2((ScreenWidth / 2) - (ballTexture.Width / 2), (ScreenHeight / 2) - (ballTexture.Height / 2)),
            });

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

            PostUpdate();

            base.Update(gameTime);
        }

        private void PostUpdate()
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i].IsRemoved)
                {
                    _sprites.RemoveAt(i);
                    i--;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

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
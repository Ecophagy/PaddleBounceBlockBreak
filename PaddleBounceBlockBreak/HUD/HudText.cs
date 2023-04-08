using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaddleBounceBlockBreak.HUD
{
    public class HudText
    {
        protected readonly SpriteFont _font;
        protected Vector2 _position;
        protected readonly string _text;

        public Vector2 Size
        {
            get
            {
                return _font.MeasureString(_text);
            }
        }

        public HudText(SpriteFont font, string text, Vector2 position)
        {
            _font = font;
            _text = text;
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _text, _position, Color.White);
        }
    }
}

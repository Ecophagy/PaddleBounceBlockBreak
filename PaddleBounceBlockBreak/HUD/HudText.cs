using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PaddleBounceBlockBreak.HUD
{
    public class HudText
    {
        private readonly SpriteFont _font;
        private readonly Vector2 _position;
        private readonly string _text;

        public HudText(SpriteFont font, string text, Vector2 position)
        {
            _font = font;
            _text = text;

            // Draw text centred around position
            _position = position - _font.MeasureString(_text) / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _text, _position, Color.White);
        }
    }
}

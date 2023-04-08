using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PaddleBounceBlockBreak.HUD
{
    public class CentredHudText : HudText
    {
        public CentredHudText(SpriteFont font, string text, Vector2 position) : base(font, text, position)
        {
            // Centre text around position
            _position = position - _font.MeasureString(_text) / 2;
        }
    }
}

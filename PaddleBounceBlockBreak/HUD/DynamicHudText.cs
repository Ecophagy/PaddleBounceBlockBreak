using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PaddleBounceBlockBreak.HUD
{
    public class DynamicHudText : HudText
    {

        public DynamicHudText(SpriteFont font, string text, Vector2 position) : base(font, text, position) { }

        public void Draw(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(_font, _text + text, _position, Color.White);
        }

    }
}

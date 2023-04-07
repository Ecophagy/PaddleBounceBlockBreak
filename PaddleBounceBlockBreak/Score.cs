using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaddleBounceBlockBreak
{
    public class Score
    {
        private SpriteFont _font;

        public Score(SpriteFont font)
        {
            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch, int score)
        {
            // TODO: Change hardcoded position to handle resizing
            spriteBatch.DrawString(_font, score.ToString(), new Vector2(10, 0), Color.White);
        }
    }
}

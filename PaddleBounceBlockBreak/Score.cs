using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaddleBounceBlockBreak
{
    public class Score
    {
        public int CurrentScore;

        private SpriteFont _font;

        public Score(SpriteFont font)
        {
            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Change hardcoded position to handle resizing
            spriteBatch.DrawString(_font, CurrentScore.ToString(), new Vector2(320, 70), Color.White);
        }
    }
}

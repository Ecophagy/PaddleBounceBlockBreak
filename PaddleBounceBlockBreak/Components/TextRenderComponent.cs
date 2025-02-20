using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaddleBounceBlockBreak.Components;

public class TextRenderComponent
{
    public readonly SpriteFont Font;
    public readonly string BaseText;
    public string Text;
    
    public Vector2 Size => Font.MeasureString(BaseText);
    
    public TextRenderComponent(SpriteFont font, string baseText)
    {
        Font = font;
        BaseText = baseText;
    }
}
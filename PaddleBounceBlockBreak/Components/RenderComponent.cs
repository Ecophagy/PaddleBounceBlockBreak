using Microsoft.Xna.Framework.Graphics;

namespace PaddleBounceBlockBreak.Components;

public class RenderComponent
{
    public Texture2D Texture { get; set; }

    public RenderComponent(Texture2D texture)
    {
        Texture = texture;
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace PaddleBounceBlockBreak.Components;

public class HealthComponent
{
    public int Health { get; set; }
    public Dictionary<int, Texture2D> DamageTextures { get; }

    public HealthComponent(int startingHealth, Dictionary<int, Texture2D> damageTextures)
    {
        Health = startingHealth;
        DamageTextures = damageTextures;
    }
}
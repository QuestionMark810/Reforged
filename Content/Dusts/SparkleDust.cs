﻿using Reforged.Common.OverDust;

namespace Reforged.Content.Dusts;

public class SparkleDust : ModDust, IOverDust
{
    public override bool Update(Dust dust)
    {
        dust.fadeIn = MathHelper.Min(dust.fadeIn + 1, 10);

        dust.velocity *= .98f;
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X / 5f;

        if ((dust.scale -= .01f) <= 0)
            dust.active = false;

        return false;
    }

    public override bool PreDraw(Dust dust) => false;

    public void CustomDraw(Dust dust)
    {
        var innerColor = Color.White;
        var outerColor = dust.color;
        var position = dust.position - Main.screenPosition;

        for (int i = 0; i < 2; i++)
        {
            var color = ((i == 0) ? outerColor : innerColor) with { A = 0 };
            float scale = dust.scale * ((i == 0) ? 1f : .75f) * (dust.fadeIn / 10f);

            Main.spriteBatch.Draw(Texture2D.Value, position, Texture2D.Frame(), color, dust.rotation,
                Texture2D.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}

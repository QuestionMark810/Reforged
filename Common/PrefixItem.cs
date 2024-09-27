using CraftingPlus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class PrefixItem : GlobalItem
{
    private static readonly List<Dust> sparks = [];

    internal static float ReforgeAnimationTime;
    internal static int OverrideReforgePrice = -1;

    public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
    {
        if (OverrideReforgePrice > -1)
            reforgePrice = OverrideReforgePrice;

        OverrideReforgePrice = -1;
        return true;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        for (int i = sparks.Count - 1; i >= 0; i--)
        {
            var s = sparks[i];
            if (s.type == ModContent.DustType<SparkDust>())
            {
                if (!s.active)
                {
                    sparks.RemoveAt(i);
                    continue;
                }

                ModContent.GetInstance<SparkDust>().CustomDraw(s);
            }
        }

        if (ReforgeAnimationTime == 0 || item != Main.reforgeItem)
            return;

        if (ReforgeAnimationTime == 1)
        {
            for (int i = 0; i < 15; i++)
                sparks.Add(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkDust>(),
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f) - (Vector2.UnitY * 2), Scale: Main.rand.NextFloat(.7f)));
        }

        for (int i = 0; i < 3; i++)
            spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, (Color.Yellow with { A = 0 }) * ReforgeAnimationTime, 0, origin, scale, SpriteEffects.None, 0);

        var starTexture = TextureAssets.Projectile[ProjectileID.RainbowRodBullet].Value;
        float rotation = (float)Main.timeForVisualEffects * .1f;
        spriteBatch.Draw(starTexture, position, null, Color.Yellow with { A = 0 }, rotation, starTexture.Size() / 2, scale * 1.25f * ReforgeAnimationTime, SpriteEffects.None, 0);

        ReforgeAnimationTime = MathHelper.Max(ReforgeAnimationTime - .05f, 0);
    }
}

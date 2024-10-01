using CraftingPlus.Common.OverDust;
using CraftingPlus.Content.Dusts;
using ReLogic.Content;
using Terraria.GameContent;

namespace CraftingPlus.Common;

public class PrefixItem : GlobalItem
{
    private static Asset<Effect> dissipate;

    internal static float ReforgeAnimationTime;
    internal static int OverrideReforgePrice = -1;

    //Temp
    //private static int[] rarePrefixes = [PrefixID.Legendary, PrefixID.Legendary2, PrefixID.Mythical, PrefixID.Unreal, PrefixID.Godly, PrefixID.Masterful];

    public override void Load() => dissipate = Mod.Assets.Request<Effect>("Assets/Effects/Dissipate");

    public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
    {
        if (OverrideReforgePrice > -1)
            reforgePrice = OverrideReforgePrice;

        OverrideReforgePrice = -1;
        return true;
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        OverDustSystem.DrawAllDusts();
        if (ReforgeAnimationTime == 0 || item != Main.reforgeItem)
            return;

        if (ReforgeAnimationTime == 1) //Spawn dusts
        {
            for (int i = 0; i < 15; i++)
                OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkDust>(),
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), Scale: Main.rand.NextFloat(.7f)));
        }

        for (int i = 0; i < 3; i++)
            spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, (Color.Yellow with { A = 0 }) * ReforgeAnimationTime, 0, origin, scale, SpriteEffects.None, 0);

        var starTexture = TextureAssets.Projectile[79].Value;
        float rotation = (float)Main.timeForVisualEffects * .05f;
        for (int i = 0; i < 2; i++)
        {
            var color = ((i > 0) ? Color.White : Color.Yellow) with { A = 0 };
            float newScale = scale * 1.25f * ReforgeAnimationTime;
            if (i > 0)
                scale *= .5f;

            spriteBatch.Draw(starTexture, position, null, color * .25f, rotation, starTexture.Size() / 2, newScale, SpriteEffects.None, 0);
        }

        if (dissipate.IsLoaded)
        {
            spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

            var shader = dissipate.Value;
            shader.Parameters["noiseTexture"].SetValue(TextureAssets.Extra[193].Value);
            shader.Parameters["time"].SetValue(rotation * .1f);
            shader.Parameters["sizeMult"].SetValue(ReforgeAnimationTime * 5);
            shader.CurrentTechnique.Passes[0].Apply();

            var ringTexture = TextureAssets.GlowMask[239].Value;
            float growScale = Main.inventoryScale * .5f * (1f - ReforgeAnimationTime);
            spriteBatch.Draw(ringTexture, position, null, Color.Orange with { A = 0 }, 0, ringTexture.Size() / 2, growScale, SpriteEffects.None, 0);

            spriteBatch.End(); spriteBatch.Begin();
        }

        ReforgeAnimationTime = ReforgeAnimationTime = MathHelper.Max(ReforgeAnimationTime - .05f, 0);
    }
}

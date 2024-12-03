using Reforged.Common.OverDust;
using Reforged.Content.Dusts;
using ReLogic.Content;
using System.Linq;
using Terraria.GameContent;
using Terraria.ID;

namespace Reforged.Common;

public partial class PrefixItem : GlobalItem
{
    private static Asset<Effect> dissipate;
    private static Asset<Texture2D> glow;

    private static int animTime, animTimeMax;

    private static readonly int[] rarePrefixes = [PrefixID.Legendary, PrefixID.Legendary2, PrefixID.Mythical, PrefixID.Unreal, PrefixID.Godly, PrefixID.Masterful];

    public static bool RolledRarePrefix()
    {
        int prefix = Main.reforgeItem.prefix;
        return rarePrefixes.Contains(prefix);
    }

    public static void StartAnimation(int time) => animTime = animTimeMax = time;

    public override void Load()
    {
        dissipate = Mod.Assets.Request<Effect>("Assets/Effects/Dissipate");
        glow = Mod.Assets.Request<Texture2D>("Assets/Textures/SlotGlow");
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (item != Main.reforgeItem)
            return;

        OverDustSystem.DrawAllDusts();

        if (animTime == 0)
            return;

        float time = animTime / (float)animTimeMax;
        if (RolledRarePrefix())
        {
            if (time == 1) //Spawn dusts
            {
                for (int i = 0; i < 15; i++)
                    OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkDust>(),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), Scale: Main.rand.NextFloat(.7f)));

                for (int i = 0; i < 5; i++)
                    OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkleDust>(),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f), 0, RandomColor(), Main.rand.NextFloat(.7f)));
            }

            if (Main.rand.NextBool(5))
                OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(30f), ModContent.DustType<SparkleDust>(),
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(.1f), 0, RandomColor(), Main.rand.NextFloat(.3f, .5f)));

            spriteBatch.Draw(glow.Value, position, null, (Main.DiscoColor with { A = 0 }) * time * .25f, 0, glow.Size() / 2 + new Vector2(0, 6), Main.inventoryScale, SpriteEffects.None, 0);
            spriteBatch.Draw(glow.Value, position, null, (Color.White with { A = 0 }) * time * .2f, 0, glow.Size() / 2 + new Vector2(0, 6), Main.inventoryScale, SpriteEffects.None, 0);

            var starTexture = TextureAssets.Projectile[79].Value;
            float rotation = (float)Main.timeForVisualEffects * .05f;

            for (int i = 0; i < 2; i++)
            {
                var color = ((i > 0) ? Color.White : Color.Yellow) with { A = 0 };
                float newScale = scale * 1.25f * time;

                if (i > 0)
                    newScale *= .5f;

                spriteBatch.Draw(starTexture, position, null, color * .25f, rotation, starTexture.Size() / 2, newScale, SpriteEffects.None, 0);
            }

            if (dissipate.IsLoaded)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                var shader = dissipate.Value;
                shader.Parameters["noiseTexture"].SetValue(TextureAssets.Extra[193].Value);
                shader.Parameters["time"].SetValue(rotation * .1f);
                shader.Parameters["sizeMult"].SetValue(time * 5);
                shader.CurrentTechnique.Passes[0].Apply();

                const int layers = 3;
                for (int i = 0; i < layers; i++)
                {
                    var newColor = Main.DiscoColor with { A = 0 };
                    spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, newColor, 0, origin, scale, SpriteEffects.None, 0);
                }

                var ringTexture = TextureAssets.GlowMask[239].Value;
                float growScale = Main.inventoryScale * .6f * (1f - time);
                spriteBatch.Draw(ringTexture, position, null, Main.DiscoColor with { A = 0 }, 0, ringTexture.Size() / 2, growScale, SpriteEffects.None, 0);
                spriteBatch.Draw(ringTexture, position, null, (Color.White with { A = 0 }) * .5f, 0, ringTexture.Size() / 2, growScale, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
            }
        }
        else
        {
            if (time == 1) //Spawn dusts
            {
                for (int i = 0; i < 15; i++)
                    OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkDust>(),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), Scale: Main.rand.NextFloat(.7f)));

                for (int i = 0; i < 3; i++)
                    OverDustSystem.AddDust(Dust.NewDustPerfect(position + Main.screenPosition, ModContent.DustType<SparkleDust>(),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f), 0, Color.Red, Main.rand.NextFloat(.7f)));
            }

            spriteBatch.Draw(glow.Value, position, null, (Color.Goldenrod with { A = 0 }) * time * .25f, 0, glow.Size() / 2 + new Vector2(0, 6), Main.inventoryScale, SpriteEffects.None, 0);

            const int layers = 3;
            for (int i = 0; i < layers; i++)
            {
                var newColor = (Color.Lerp(Color.Red, Color.Yellow, i / (layers - 1f)) with { A = 0 }) * time;
                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, newColor, 0, origin, scale, SpriteEffects.None, 0);
            }

            var starTexture = TextureAssets.Projectile[79].Value;
            float rotation = (float)Main.timeForVisualEffects * .05f;

            for (int i = 0; i < 2; i++)
            {
                var color = ((i > 0) ? Color.White : Color.Yellow) with { A = 0 };
                float newScale = scale * 1.25f * time;

                if (i > 0)
                    newScale *= .5f;

                spriteBatch.Draw(starTexture, position, null, color * .25f, rotation, starTexture.Size() / 2, newScale, SpriteEffects.None, 0);
            }

            if (dissipate.IsLoaded)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                var shader = dissipate.Value;
                shader.Parameters["noiseTexture"].SetValue(TextureAssets.Extra[193].Value);
                shader.Parameters["time"].SetValue(rotation * .1f);
                shader.Parameters["sizeMult"].SetValue(time * 5);
                shader.CurrentTechnique.Passes[0].Apply();

                var ringTexture = TextureAssets.GlowMask[239].Value;
                float growScale = Main.inventoryScale * .5f * (1f - time);
                spriteBatch.Draw(ringTexture, position, null, Color.Orange with { A = 0 }, 0, ringTexture.Size() / 2, growScale, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
            }
        }

        animTime--;
        static Color RandomColor() => Main.rand.NextFromList(Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Yellow, Color.Purple);
    }
}

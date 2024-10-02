using Reforged.Common.OverDust;
using Reforged.Content.Dusts;
using ReLogic.Content;
using System.Collections.ObjectModel;
using Terraria.GameContent;
using Terraria.Localization;

namespace Reforged.Common;

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
        reforgePrice = (int)(reforgePrice * ModContent.GetInstance<ServerConfig>().ReforgeCostMultiplier);

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

    public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
    {
        static void DrawBoxExtension(int width, int height, Vector2 position, Color? overrideColor = null)
        {
            overrideColor ??= new Color(100, 100, 150) * .9f;

            var texture = TextureAssets.InventoryBack.Value;

            var loops = width;
            for (int i = 0; i < (loops * height); i++)
            {
                var frameX = ((i % loops) == 0) ? 0 : (((i % loops) == (loops - 1)) ? 2 : 1);
                var frameY = ((i / loops) == 0) ? 0 : (((i / loops) == (height - 1)) ? 2 : 1);

                var frame = texture.Frame(3, 3, frameX, frameY);
                var tiledPos = position + new Vector2(frame.Width * (i % loops) * Main.UIScale, frame.Height * (i / loops) * Main.UIScale);

                Main.spriteBatch.Draw(texture, tiledPos, frame, overrideColor.Value, 0, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0);
            }
        }

        if (item.CanHavePrefixes() && Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().ForgeUnlocked && Loader.HoverOverCraftingButton)
        {
            var tutorial = Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().showTutorial;

            var position = new Vector2(x - 14, y + 10);
            foreach (var line in lines)
                position.Y += FontAssets.MouseText.Value.MeasureString(line.Text).Y;

            //Draw an opaque tooltip box
            if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
                DrawBoxExtension(15, 2, position, tutorial ? Color.LightBlue * (Main.mouseTextColor / 255f) : null);

            //Draw a glowing reforge icon
            var reforge = TextureAssets.Reforge[1].Value;
            Main.spriteBatch.Draw(reforge, position - new Vector2(2, 14), null, Main.MouseTextColorReal, 0, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0);

            //Draw text
            var text = Language.GetTextValue(Reforged.locKey + "Misc.Forge");
            Utils.DrawBorderString(Main.spriteBatch, text, position + new Vector2(16, 6), tutorial ? Color.Yellow * (Main.mouseTextColor / 255f) : Main.MouseTextColorReal, 1);
        }

        return true;
    }
}

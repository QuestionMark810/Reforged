using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class PrefixItem : GlobalItem
{
    internal static int OverrideReforgePrice = -1;

    public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
    {
        if (OverrideReforgePrice > -1)
            reforgePrice = OverrideReforgePrice;

        OverrideReforgePrice = -1;

        return true;
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
            var reforge = Main.Assets.Request<Texture2D>("Images/UI/Reforge_1");
            Main.spriteBatch.Draw(reforge.Value, position - new Vector2(2, 14), null, Main.MouseTextColorReal, 0, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0);

            //Draw text
            var text = Language.GetTextValue("Mods.CraftingPlus.UI.Forge");
            Utils.DrawBorderString(Main.spriteBatch, text, position + new Vector2(16, 6), tutorial ? Color.Yellow * (Main.mouseTextColor / 255f) : Main.MouseTextColorReal, 1);
        }

        return true;
    }
}

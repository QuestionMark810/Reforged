using System.Collections.ObjectModel;
using Terraria.GameContent;
using Terraria.Localization;

namespace Reforged.Common;

public partial class PrefixItem : GlobalItem
{
    internal static int OverrideReforgePrice = -1;

    public override bool AllowPrefix(Item item, int pre) => !RepeatPrefix.IsRepeated(pre);

    public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
    {
        reforgePrice = (int)(reforgePrice * ModContent.GetInstance<ServerConfig>().reforgeMult);

        if (OverrideReforgePrice > -1)
            reforgePrice = OverrideReforgePrice;

        OverrideReforgePrice = -1;
        return true;
    }

    public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) //Used by the forge mechanic to draw custom tooltip hints
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
                var tiledPos = position + new Vector2(frame.Width * (i % loops), frame.Height * (i / loops));

                Main.spriteBatch.Draw(texture, tiledPos, frame, overrideColor.Value, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
            Main.spriteBatch.Draw(reforge, position - new Vector2(2, 14), null, Main.MouseTextColorReal, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            //Draw text
            var text = Language.GetTextValue(Reforged.locKey + "Misc.Forge");
            Utils.DrawBorderString(Main.spriteBatch, text, position + new Vector2(16, 6), tutorial ? Color.Yellow * (Main.mouseTextColor / 255f) : Main.MouseTextColorReal, 1);
        }

        return true;
    }
}

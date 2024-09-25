using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CraftingPlus.Common;

public static class Helpers
{
    public static void DrawResourceBar(SpriteBatch spriteBatch, Vector2 position, Point size, float progress, float opacity = 1)
    {
        var bar = TextureAssets.MagicPixel.Value;
        var frame = new Rectangle(0, 0, size.X, size.Y);

        //Draw the progress bar base
        spriteBatch.Draw(bar, position, frame with { Width = frame.Width + 2, Height = frame.Height + 2 }, Color.Black * opacity, 0, (frame.Size() / 2) + new Vector2(1), 1, SpriteEffects.None, 0);
        spriteBatch.Draw(bar, position, frame, Color.DarkSlateBlue * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);

        var loops = frame.Width;
        for (int i = 0; i < loops; i++)
        {
            var half = loops / 2;

            //Lerp in 2 phases to get a wider gradient
            var color = (i <= half) ? Color.Lerp(Color.Red, Color.Orange, (float)i / half) : Color.Lerp(Color.Orange, Color.White, (float)(i - half) / half);

            //Draw a progress bar endcap
            if (i >= (int)(loops * progress) - 3)
                color *= 3;
            if (i >= (int)(loops * progress))
                break;

            spriteBatch.Draw(bar, position + Vector2.UnitX * i, new Rectangle(0, 0, frame.Width / loops, frame.Height), color * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);
        }

        //Draw a highlight
        spriteBatch.Draw(bar, position + new Vector2(0, 2), new Rectangle(0, 0, (int)(frame.Width * progress), 8), Color.White * .5f * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);
    }

    /*public enum PrefixStyle : int
    {
        Unfavourable = -4,
        Favourable = -3,
        Tinkerer = -2,
        ChestLoot = -1,
        None = 0
    }*/

    /// <summary> Reforges <see cref="Main.reforgeItem"/> using the Goblin Tinkerer's reforge logic </summary>
    /// <param name="prefix">The prefix you want for <see cref="Main.reforgeItem"/></param>
    public static void Reforge(int prefix = -2)
    {
        ItemLoader.PreReforge(Main.reforgeItem);

        Main.reforgeItem.ResetPrefix();
        Main.reforgeItem.Prefix(prefix);

        var player = Main.LocalPlayer;
        Main.reforgeItem.position.X = player.position.X + (float)(player.width / 2) - (float)(Main.reforgeItem.width / 2);
        Main.reforgeItem.position.Y = player.position.Y + (float)(player.height / 2) - (float)(Main.reforgeItem.height / 2);
        ItemLoader.PostReforge(Main.reforgeItem);
        PopupText.NewText(PopupTextContext.ItemReforge, Main.reforgeItem, Main.reforgeItem.stack, noStack: true);
        SoundEngine.PlaySound(in SoundID.Item37);
    }

    /// <summary> Calls <see cref="ItemSlot.Draw"/> but allows for direct control over scale. </summary>
    public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, int context, Vector2 position, float scale = -1, Color lightColor = default)
    {
        var oldScale = Main.inventoryScale;
        Main.inventoryScale = (scale == -1) ? Main.inventoryScale : scale;

        ItemSlot.Draw(spriteBatch, ref item, context, position, lightColor);

        Main.inventoryScale = oldScale;
    }
}

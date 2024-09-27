using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CraftingPlus.Common;

public static class Helpers
{
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

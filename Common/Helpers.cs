using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace Reforged.Common;

public static class Helpers
{
    internal static int PrefixOnCraft = -1;

    /// /// <summary> Reforges <see cref="Main.reforgeItem"/> using the Goblin Tinkerer's reforge logic. </summary>
    /// <param name="prefix"> The prefix you want for <see cref="Main.reforgeItem"/>. </param>
    /// /// <param name="allowRepeats"> Whether a repeat prefix can be rolled in one reforge session. </param>
    public static void Reforge(int prefix = -2, bool allowRepeats = true)
    {
        ItemLoader.PreReforge(Main.reforgeItem);

        if (!allowRepeats)
        {
            RepeatPrefix.RollFromList();
        }
        else
        {
            Main.reforgeItem.ResetPrefix();
            Main.reforgeItem.Prefix(prefix);
        }

        Main.reforgeItem.Center = Main.LocalPlayer.Center;
        ItemLoader.PostReforge(Main.reforgeItem);
        PopupText.NewText(PopupTextContext.ItemReforge, Main.reforgeItem, Main.reforgeItem.stack, noStack: true);
        SoundEngine.PlaySound(in SoundID.Item37);
    }

    public static void CraftItemWithPrefix(Recipe recipe, int prefix)
    {
        PrefixOnCraft = prefix;
        Main.CraftItem(recipe);
        PrefixOnCraft = -1;
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

using Terraria.GameContent;
using Terraria.UI;

namespace Reforged.Common.UI;

internal class ReforgeSlot : UIElement
{
    private readonly float scale;
    internal float opacity;

    public ReforgeSlot(float scale = 1f, float opacity = 1f)
    {
        this.scale = scale;
        this.opacity = opacity;

        Width.Set(TextureAssets.InventoryBack9.Value.Width * scale, 0f);
        Height.Set(TextureAssets.InventoryBack9.Value.Height * scale, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Color oldColor = Main.inventoryBack;
        Main.inventoryScale = scale;
        Main.inventoryBack = Color.Lerp(Color.Transparent, oldColor, opacity);

        var context = ItemSlot.Context.PrefixItem;

        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.craftingHide = true;

            ItemSlot.LeftClick(ref Main.reforgeItem, context);

            if (Main.mouseLeftRelease && Main.mouseLeft)
                Recipe.FindRecipes();

            ItemSlot.RightClick(ref Main.reforgeItem, context);
            ItemSlot.MouseHover(ref Main.reforgeItem, context);
        }
        ItemSlot.Draw(spriteBatch, ref Main.reforgeItem, context, GetDimensions().ToRectangle().TopLeft());

        Main.inventoryScale = oldScale;
        Main.inventoryBack = oldColor;
    }
}

﻿using Terraria.GameContent;
using Terraria.UI;

namespace Reforged.Common.UI;

internal class InventorySlot : UIElement
{
    internal Item Item;
    private readonly float scale;
    internal float opacity;
    private readonly bool noInteraction;

    public InventorySlot(Item item, float scale = 1f, float opacity = 1f, bool noInteraction = false)
    {
        Item = item;
        this.scale = scale;
        this.opacity = opacity;
        this.noInteraction = noInteraction;

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

            if (!noInteraction)
            {
                ItemSlot.LeftClick(ref Item, context);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                    Recipe.FindRecipes();

                ItemSlot.RightClick(ref Item, context);
                ItemSlot.MouseHover(ref Item, context);
            }
        }
        ItemSlot.Draw(spriteBatch, ref Item, context, GetDimensions().ToRectangle().TopLeft());

        Main.inventoryScale = oldScale;
        Main.inventoryBack = oldColor;
    }
}

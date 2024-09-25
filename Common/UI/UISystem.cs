using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CraftingPlus.Common.UI;

public class UISystem : ModSystem
{
    internal static UserInterface ReforgeInterface, /*RandomizerInterface,*/ MinigameInterface;
    //internal static Randomizer RandomizerState;
    internal static Reforge ReforgeState;

    public override void Load()
    {
        //RandomizerInterface = new();
        ReforgeInterface = new();
        MinigameInterface = new();
        //RandomizerState = new();
        ReforgeState = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        MinigameInterface?.Update(gameTime);
        //RandomizerInterface?.Update(gameTime);
        ReforgeInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer(
                $"{Mod.Name}: MinigameUI",
                delegate {
                    MinigameInterface?.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
            /*layers.Insert(index, new LegacyGameInterfaceLayer(
                $"{Mod.Name}: RandomizerUI",
                delegate {
                    RandomizerInterface?.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );*/
            layers.Insert(index, new LegacyGameInterfaceLayer(
                $"{Mod.Name}: RandomizerUI",
                delegate {
                    ReforgeInterface?.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
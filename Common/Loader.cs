using Reforged.Common.UI.Core;
using Reforged.Common.UI;
using MonoMod.Cil;
using Terraria.ID;
using System.Reflection;
using log4net;

namespace Reforged.Common;

public class Loader : ILoadable
{
    private static ILog Logger => ModContent.GetInstance<Reforged>().Logger;
    public static bool HoverOverCraftingButton { get; private set; }

    public void Load(Mod mod)
    {
        On_Main.CraftItem += CraftItem;
        IL_Main.CraftItem += CraftItemWithPrefix;
        On_Main.HoverOverCraftingItemButton += HoverOverCraftingItemButton;
        On_Player.ResetEffects += ResetHoverOverCraftingItemButton;
        IL_Main.DrawInventory += RemoveReforgeButton;
    }

    public void Unload() { }

    private void CraftItem(On_Main.orig_CraftItem orig, Recipe r)
    {
        if (r.createItem.CanHavePrefixes() && Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().ForgeUnlocked && Main.mouseRight 
            && UISystem.GetState<Minigame>().UserInterface.CurrentState is null)
        {
            var inst = r.requiredTile.Contains(TileID.Anvils) ? new UI.Minigames.Anvil() : new UI.Minigames.Workbench();
            inst.SetRecipe(r);

            UISystem.GetState<Minigame>().UserInterface.SetState(inst);
            Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().showTutorial = false;
        }
        else orig(r); //Skip orig
    }

    private void CraftItemWithPrefix(ILContext il)
    {
        ILCursor c = new(il);

        var prefix = typeof(Item).GetMethod(nameof(Item.Prefix), BindingFlags.Public | BindingFlags.Instance, [typeof(int)]);
        if (prefix is null || !c.TryGotoNext(x => x.MatchCallvirt(prefix)))
        {
            string isNull = (prefix is null) ? ", value is null" : string.Empty;
            Logger.Debug("Failed goto: " + nameof(Item.Prefix) + isNull);
            return;
        }

        //Manually override a crafted item's prefix right before ModItem.Create is called
        c.EmitDelegate((int prefix) => { return (Helpers.PrefixOnCraft == -1) ? prefix : Helpers.PrefixOnCraft; });
    }

    private void HoverOverCraftingItemButton(On_Main.orig_HoverOverCraftingItemButton orig, int recipeIndex)
    {
        HoverOverCraftingButton = true;
        orig(recipeIndex);
    }

    private void ResetHoverOverCraftingItemButton(On_Player.orig_ResetEffects orig, Player self)
    {
        HoverOverCraftingButton = false;
        orig(self);
    }

    private void RemoveReforgeButton(ILContext il)
    {
        ILCursor c = new(il);

        //Don't open the reforge menu
        if (!c.TryGotoNext(x => x.MatchLdsfld<Main>("InReforgeMenu")))
        {
            Logger.Debug("Failed goto: " + nameof(Main.InReforgeMenu));
            return;
        }

        c.Remove();
        c.EmitDelegate(() => false);
    }
}

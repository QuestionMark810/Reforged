using CraftingPlus.Common.UI;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class Loader : ILoadable
{
    private static int PrefixOnCraft = -1;
    //internal static string OverrideReforgeText;

    public static bool HoverOverCraftingButton { get; private set; }

    public void Load(Mod mod)
    {
        On_Main.CraftItem += CraftItem;
        On_Main.HoverOverCraftingItemButton += HoverOverCraftingItemButton;
        On_Main.DrawInventory += DrawInventory;
        On_Player.ResetEffects += ResetEffects;

        IL_Main.DrawInventory += IL_DrawInventory;
        IL_Main.CraftItem += IL_CraftItem;
    }

    public void Unload() { }

    private void ResetEffects(On_Player.orig_ResetEffects orig, Player self) => HoverOverCraftingButton = false;

    private void CraftItem(On_Main.orig_CraftItem orig, Recipe r)
    {
        if (r.createItem.CanHavePrefixes() && Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().ForgeUnlocked && Main.mouseRight && (UISystem.MinigameInterface.CurrentState is null || (UISystem.MinigameInterface.CurrentState is Minigame minigame && minigame.state == Minigame.State.InProgress)))
        {
            Minigame minigameType = r.requiredTile.Contains(TileID.Anvils) ? new UI.Minigames.Anvil(r) : new UI.Minigames.Workbench(r);

            Minigame.Enable(minigameType);
            Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().showTutorial = false;
        }
        else orig(r);
    }

    private void HoverOverCraftingItemButton(On_Main.orig_HoverOverCraftingItemButton orig, int recipeIndex)
    {
        HoverOverCraftingButton = true;
        orig(recipeIndex);
    }

    private void DrawInventory(On_Main.orig_DrawInventory orig, Main self)
    {
        //Trick DrawInventory into thinking we're not in the reforge menu conditionally
        /*var _inReforgeMenu = Main.InReforgeMenu;

        if (Main.InReforgeMenu && UISystem.MinigameInterface.CurrentState is not null)
            Main.InReforgeMenu = false;

        if (Main.InReforgeMenu && Main.reforgeItem is not null && !Main.reforgeItem.IsAir && UISystem.RandomizerInterface.CurrentState == null) //Hovering over reforge button
            Randomizer.Enable();*/

        orig(self);

        //Main.InReforgeMenu = _inReforgeMenu;
    }

    private void IL_DrawInventory(ILContext il)
    {
        ILCursor c = new(il);

        /*#region reforge price
        c.GotoNext(x => x.MatchCall<ItemSlot>("DrawSavings"));
        c.Index += 5;

        c.EmitDelegate((string def) => 
        {
            if (OverrideReforgeText == string.Empty)
                return def;
            else
                return OverrideReforgeText;
        });
        #endregion*/

        //Don't open the reforge menu
        c.GotoNext(x => x.MatchLdsfld<Main>("InReforgeMenu"));
        c.Remove();
        c.EmitDelegate(() => { return false; });

        /*#region reforge button
        var buyItem = typeof(Player).GetMethod(nameof(Player.BuyItem), BindingFlags.Public | BindingFlags.Instance, [typeof(int), typeof(int)]);
        c.GotoNext(MoveType.After, x => x.MatchCallvirt(buyItem));
        c.Index++;

        c.EmitDelegate(() =>
        {
            //The player has clicked the reforge button
            if (UISystem.MinigameInterface.CurrentState is null)
            {
                var prefix = UISystem.RandomizerState.Prefix;

                if (prefix > 0 && (Main.reforgeItem.prefix == prefix || !Main.reforgeItem.CanApplyPrefix(prefix)))
                    UISystem.RandomizerState.ShakeButton();
                else if (prefix > 0)
                    Minigame.Enable(new UI.Minigames.Forge());
                else
                    Helpers.Reforge();
            }
        });
        c.RemoveRange(69); //Remove all vanilla reforge functionality from the tinkerer's reforge button
        #endregion*/
    }

    private void IL_CraftItem(ILContext il)
    {
        ILCursor c = new(il);

        var prefix = typeof(Item).GetMethod(nameof(Item.Prefix), BindingFlags.Public | BindingFlags.Instance, [typeof(int)]);
        c.GotoNext(x => x.MatchCallvirt(prefix));

        //c.Remove(); //Remove -1 from the evaluation stack //This is unecessary if we consume it
        c.EmitDelegate((int prefix) => { return PrefixOnCraft; }); //Manually override a crafted item's prefix right before ModItem.Create is called
    }

    public static void CraftItemWithPrefix(Recipe recipe, int prefix)
    {
        PrefixOnCraft = prefix;
        Main.CraftItem(recipe);
        PrefixOnCraft = -1;
    }

    //public override void PostUpdatePlayers() => HoverOverCraftingButton = false;
}

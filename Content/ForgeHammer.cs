using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Content;

public class ForgeHammer : ModItem
{
    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 5;

    public override void SetDefaults() => Item.CloneDefaults(ItemID.LicenseCat);

    public override bool? UseItem(Player player)
    {
        /*if (!player.GetModPlayer<Common.SaveDataPlayer>().ForgeUnlocked)
        {
            Common.SaveDataPlayer.UnlockForgeMechanic(player);
            return true;
        }*/

        return null;
    }
}

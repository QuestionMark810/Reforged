using Terraria.ID;

namespace Reforged.Common;

public class VendorNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.GoblinTinkerer)
            shop.Add(ModContent.ItemType<Content.ForgeHammer>(), new Condition(Reforged.locKey + "Misc.Obtained", 
                () => !Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().ForgeUnlocked));
    }
}

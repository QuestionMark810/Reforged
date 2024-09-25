using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class VendorNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.GoblinTinkerer)
            shop.Add(ModContent.ItemType<Content.ForgeHammer>(), new Condition(string.Empty, () => !Main.LocalPlayer.GetModPlayer<SaveDataPlayer>().ForgeUnlocked));
    }

    public override void OnChatButtonClicked(NPC npc, bool firstButton)
    {
        if (npc.type == NPCID.GoblinTinkerer && !firstButton)
            UI.Reforge.Enable();
    }
}

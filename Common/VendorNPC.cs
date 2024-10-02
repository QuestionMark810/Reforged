using Reforged.Common.UI;
using Reforged.Common.UI.Core;
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

    public override void OnChatButtonClicked(NPC npc, bool firstButton)
    {
        if (npc.type == NPCID.GoblinTinkerer && !firstButton)
            UISystem.GetState<ReforgeMenu>().UserInterface.SetState(UISystem.GetState<ReforgeMenu>());
    }
}

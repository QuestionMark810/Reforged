using CraftingPlus.Common.UI;
using CraftingPlus.Common.UI.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class VendorNPC : GlobalNPC
{
    public override void OnChatButtonClicked(NPC npc, bool firstButton)
    {
        if (npc.type == NPCID.GoblinTinkerer && !firstButton)
            UISystem.GetState<ReforgeMenu>().UserInterface.SetState(UISystem.GetState<ReforgeMenu>());
    }
}

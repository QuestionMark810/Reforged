using Terraria.ModLoader.IO;

namespace CraftingPlus.Common;

public class SaveDataPlayer : ModPlayer
{
    public bool showTutorial = false;
    public bool ForgeUnlocked { get; private set; }

    public static void UnlockForgeMechanic(Player player)
    {
        var mp = player.GetModPlayer<SaveDataPlayer>();

        if (!mp.ForgeUnlocked)
        {
            mp.showTutorial = true;
            mp.ForgeUnlocked = true;
        }
    }

    public override void SaveData(TagCompound tag) => tag[nameof(ForgeUnlocked)] = ForgeUnlocked;

    public override void LoadData(TagCompound tag) => ForgeUnlocked = tag.GetBool(nameof(ForgeUnlocked));
}

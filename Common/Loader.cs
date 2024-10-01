using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace CraftingPlus.Common;

public class Loader : ILoadable
{
    public void Load(Mod mod) => IL_Main.DrawInventory += RemoveReforgeButton;

    public void Unload() { }

    private void RemoveReforgeButton(ILContext il)
    {
        ILCursor c = new(il);

        //Don't open the reforge menu
        c.GotoNext(x => x.MatchLdsfld<Main>("InReforgeMenu"));
        c.Remove();
        c.EmitDelegate(() => false);
    }
}

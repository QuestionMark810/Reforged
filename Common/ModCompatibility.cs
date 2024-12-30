using Reforged.Common.UI;

namespace Reforged.Common;

internal class ModCompatibility : ILoadable
{
    public void Load(Mod mod)
    {
        if (ModLoader.TryGetMod("ConsistentReforging", out _))
        {
            ReforgeMenu.ButtonOffset += new Vector2(30, 6);
            ReforgeMenu.TextPosition += new Vector2(40, 6);
        }
        else if (ModLoader.TryGetMod("OffHandidiotmod", out _))
        {
            ReforgeMenu.ButtonOffset += new Vector2(30, 0);
            ReforgeMenu.TextPosition += new Vector2(10, 0);
        }
    }

    public void Unload() { }
}

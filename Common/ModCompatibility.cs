using Reforged.Common.UI;

namespace Reforged.Common;

internal class ModCompatibility : ILoadable
{
    public void Load(Mod mod)
    {
        if (ModLoader.TryGetMod("ConsistentReforging", out _))
        {
            ReforgeMenu.ButtonPosition += new Vector2(10, 6);
            ReforgeMenu.TextPosition += new Vector2(40, 6);
        }
    }

    public void Unload() { }
}

using System.Collections.Generic;

namespace Reforged.Common.OverDust;

internal class OverDustSystem : ModSystem
{
    private static readonly List<Dust> dusts = [];

    public static void AddDust(Dust dust) => dusts.Add(dust);

    public static void DrawAllDusts()
    {
        foreach (var d in dusts)
            if (d.active && DustLoader.GetDust(d.type) is ModDust md && md is IOverDust od)
                od.CustomDraw(d);
    }

    public override void PostUpdateDusts()
    {
        for (int i = dusts.Count - 1; i >= 0; i--)
        {
            var s = dusts[i];
            if (!s.active && DustLoader.GetDust(s.type) is ModDust md && md is IOverDust)
                dusts.RemoveAt(i);
        }
    }
}

using System.Collections.Generic;

namespace Reforged.Common;

internal static class RepeatPrefix
{
    private static bool RollingFromList;
    public static float Count { get; private set; }
    private static readonly List<int> prefixCache = [];

    public static bool IsRepeated(int prefix) => RollingFromList && prefixCache.Contains(prefix);

    public static void RollFromList()
    {
        RollingFromList = true;
        prefixCache.Add(Main.reforgeItem.prefix);

        for (int i = 0; i < 10; i++) //Multiplied by the number of times Prefix rolls (50)
        {
            Main.reforgeItem.ResetPrefix();

            if (Main.reforgeItem.Prefix(-2) && Main.reforgeItem.prefix != 0)
                break;

            if (prefixCache.Count != 0)
                prefixCache.RemoveAt(0); //We have failed to roll a non-repeat prefix (per PrefixItem.AllowPrefix)
        }

        Count++;
        RollingFromList = false;
    }

    public static void Reset()
    {
        prefixCache.Clear();
        Count = 0;
    }
}

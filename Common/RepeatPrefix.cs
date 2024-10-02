using System.Linq;

namespace Reforged.Common;

internal static class RepeatPrefix
{
    public static float Count { get; private set; }
    private static readonly int[] prefixCache = new int[10];

    public static void RollFromList()
    {
        static void AddToCache(int prefix)
        {
            if (prefixCache.Contains(prefix))
                return;

            if (prefixCache.Last() != -1) //The array is full- shuffle down
                for (int i = 0; i < prefixCache.Length; i++)
                {
                    if (i == (prefixCache.Length - 1))
                        prefixCache[i] = -1;
                    else
                        prefixCache[i] = prefixCache[i + 1];
                }

            for (int i = 0; i < prefixCache.Length; i++)
                if (prefixCache[i] == -1)
                    prefixCache[i] = prefix;
        }

        AddToCache(Main.reforgeItem.prefix);
        for (int i = 0; i < 30; i++)
        {
            Main.reforgeItem.ResetPrefix();
            Main.reforgeItem.Prefix(-2); //Tinkerer reforge roll

            if (!prefixCache.Contains(Main.reforgeItem.prefix))
                break; //Stop selecting prefixes if this prefix isn't a repeat
        }

        Count++;
    }

    public static void Reset()
    {
        for (int i = 0; i < prefixCache.Length; i++)
            prefixCache[i] = -1;

        Count = 0;
    }
}

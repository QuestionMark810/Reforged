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

        int prefix = 0;
        for (int i = 0; i < 30; i++) //Randomly select a non-repeat prefix 30 times before giving up
        {
            var item = Main.reforgeItem;
            item.ResetPrefix();
            item.Prefix(-2);

            if (!prefixCache.Contains(item.prefix))
                break;
        }

        AddToCache(prefix);
        Count++;
    }

    public static void Reset()
    {
        for (int i = 0; i < prefixCache.Length; i++)
            prefixCache[i] = -1;

        Count = 0;
    }
}

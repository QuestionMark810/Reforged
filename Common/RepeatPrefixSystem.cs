using System.Collections.Generic;
using System.Linq;

namespace CraftingPlus.Common;

internal class RepeatPrefixSystem : ModSystem
{
    private static int[] prefixCache;
    private static HashSet<int> rollablePrefixes;

    public static float Count { get; private set; }

    public static void RollFromList()
    {
        if (rollablePrefixes.Count == 0)
            return;

        var myList = rollablePrefixes.Where(x => !prefixCache.Contains(x)).ToList();
        int length = rollablePrefixes.Count;
        if (myList.Count == 0) //No more available prefixes- shuffle down
        {
            myList.Add(prefixCache[0]);

            for (int i = 0; i < length; i++)
                prefixCache[i] = prefixCache[i + 1];
        }

        int selectedPrefix = myList[Main.rand.Next(myList.Count)];
        Main.reforgeItem.ResetPrefix();
        Main.reforgeItem.Prefix(selectedPrefix);
        if (!Main.reforgeItem.Prefix(selectedPrefix))
        {
            //If the reforge still couldn't be applied, add it to the cache
            AddToCache(selectedPrefix);
        }

        AddToCache(Main.reforgeItem.prefix);
        void AddToCache(int prefix)
        {
            if (prefix == 0)
                return;

            for (int i = 0; i < length; i++)
            {
                if (prefixCache[i] == prefix) //A duplicate was found
                    break;
                if (prefixCache[i] == 0) //An empty slot was found
                {
                    prefixCache[i] = prefix;
                    break;
                }
            }
        }

        Count = 1f - ((float)myList.Count / rollablePrefixes.Count);
    }

    public static void Set()
    {
        ClearCache();
        if (Main.reforgeItem.IsAir)
            return;

        rollablePrefixes.Clear();
        for (int p = 0; p < PrefixLoader.PrefixCount; p++)
            if (Main.reforgeItem.CanApplyPrefix(p))
                rollablePrefixes.Add(p);
    }

    public static void ClearCache()
    {
        for (int i = 0; i < prefixCache.Length; i++)
            prefixCache[i] = 0;

        Count = 0;
    }

    public override void PostSetupContent()
    {
        prefixCache = new int[PrefixLoader.PrefixCount];
        rollablePrefixes = [];
    }
}

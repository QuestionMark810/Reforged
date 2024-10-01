using Terraria.Audio;
using Terraria.ID;
using System.Collections.Generic;

namespace CraftingPlus.Common.UI.Minigames;

public class Anvil : Minigame
{
    private readonly Recipe recipe = null;

    private int timer = timerMax;
    private const int timerMax = 60 * 3;

    public Anvil()
    {
        displayItem = Main.reforgeItem;//displayItem = recipe.createItem;
    }

    public override void OnClick()
    {
        if (state != State.InProgress)
            return;

        Progress = MathHelper.Min(1, Progress + .08f);
        SoundEngine.PlaySound(SoundID.Item53 with { MaxInstances = 3, Pitch = -1 + (Progress * 2) });

        if (Progress == 1)
            Complete();
    }

    public override void Update()
    {
        if (state != State.InProgress)
            return;
        if (timer - 1 == 0)
            Fail();

        timer--;
        Progress = MathHelper.Max(0, Progress - .0025f);
    }

    public override void OnComplete()
    {
        base.OnComplete();

        return;
        var item = recipe.createItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && !PrefixID.Sets.ReducedNaturalChance[p] && p != PrefixID.Annoying)
                allowed.Add(p);
        }

        //Loader.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }

    public override void OnFail()
    {
        base.OnFail();

        return;
        var item = recipe.createItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && (PrefixID.Sets.ReducedNaturalChance[p] || p == PrefixID.Annoying))
                allowed.Add(p);
        }

        //Loader.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }
}

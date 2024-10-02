using Terraria.Audio;
using Terraria.ID;
using System.Collections.Generic;

namespace Reforged.Common.UI.Minigames;

public class Anvil : Minigame
{
    public Recipe recipe;

    private int timer = timerMax;
    private const int timerMax = (int)(60 * 3.5f);

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        displayItem = recipe.createItem;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        On_Player.ResetEffects += (On_Player.orig_ResetEffects orig, Player self) =>
        {
            orig(self);

            if (Main.mouseLeft && Main.mouseLeftRelease)
                OnClick(); //Allow OnClick to happen
        };
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
        Progress = MathHelper.Max(0, Progress - .002f);
    }

    public override void OnComplete()
    {
        base.OnComplete();

        var item = displayItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && !PrefixID.Sets.ReducedNaturalChance[p] && p != PrefixID.Annoying)
                allowed.Add(p);
        }

        Helpers.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }

    public override void OnFail()
    {
        base.OnFail();

        var item = displayItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && (PrefixID.Sets.ReducedNaturalChance[p] || p == PrefixID.Annoying))
                allowed.Add(p);
        }

        Helpers.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }
}

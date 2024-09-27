using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CraftingPlus.Common.UI.Minigames;

public class Workbench : Minigame
{
    private readonly Recipe recipe = null;

    private readonly float target = Main.rand.NextFloat(.5f, 1f - targetWindow);
    private const float targetWindow = .1f;
    private float acceleration;
    private bool releasedOnce = false;

    public override void Update()
    {
        if (!(Main.mouseLeft || Main.mouseRight))
            releasedOnce = true;
        if (state != State.InProgress || !releasedOnce)
            return;

        if (Main.mouseLeft)
        {
            Progress = MathHelper.Min(1, Progress + .014f * acceleration);
            acceleration = MathHelper.Min(acceleration + .05f, 1);

            if ((int)(Progress * 100f) % 5 == 0)
                SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/Gears") { Volume = .5f, Pitch = .1f });
        }
        else if (Progress > 0)
        {
            SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/GearClick"));

            if (System.Math.Abs(Progress - target) > (targetWindow / 2))
                Fail();
            else
                Complete();
        }

        if (Progress == 1)
            Fail();
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

    public override void PostDrawBar(SpriteBatch spriteBatch)
    {
        var source = main.GetDimensions().ToRectangle();
        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);

        //Draw a window of opportunity
        var scrollbar = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar").Value;
        var color = Color.White;
        var Pos = source.Center() + new Vector2(-(dimensions.X / 2) + (dimensions.X * target), 0) - new Vector2(0, 2);

        if (state == State.Completed)
        {
            color = new Color(200, 200, 200);
            Pos.Y += 2;
        }
        spriteBatch.Draw(scrollbar, Pos, null, color * opacity, 0, scrollbar.Size() / 2, 1, SpriteEffects.None, 0);
    }
}

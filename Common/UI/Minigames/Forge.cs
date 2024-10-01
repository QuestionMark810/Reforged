using System.Linq;
using Terraria.Audio;
using Terraria.ID;

namespace CraftingPlus.Common.UI.Minigames;

public class Forge : Minigame
{
    private readonly (float, bool)[] targets;
    private const float targetWindow = .1f;
    private float acceleration;

    public Forge()
    {
        displayItem = Main.reforgeItem;
        targets = new (float, bool)[3];

        for (int i = 0; i < targets.Length; i++)
        {
            float half = targetWindow / 2f;
            float div = 1f / targets.Length;

            var lower = (div * i) + half;
            var upper = (div * (i + 1)) - half;

            targets[i] = (MathHelper.Max(.2f, Main.rand.NextFloat(lower, upper)), false);
        }
    }

    public override void OnClick()
    {
        bool hit = false;
        for (int i = 0; i < targets.Length; i++)
            if (System.Math.Abs(Progress - targets[i].Item1) < (targetWindow / 2))
            {
                targets[i].Item2 = true;
                hit = true;
            }

        if (hit)
        {
            SoundEngine.PlaySound(SoundID.Item53 with { Pitch = 0 + Progress });
            SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/GearClick"));

            if (targets.All(x => x.Item2)) //Check if all buttons were hit
                Complete();
        }
        else Fail();
    }

    public override void Update()
    {
        PrefixItem.OverrideReforgePrice = 0;
        if (state != State.InProgress)
            return;

        Progress = MathHelper.Min(1, Progress + .009f * acceleration);
        acceleration = MathHelper.Min(acceleration + .01f, 1);

        if (Progress == 1)
            Fail();
    }

    public override bool Active() => base.Active() && Main.InReforgeMenu && Main.reforgeItem is not null && !Main.reforgeItem.IsAir;

    public override void OnComplete()
    {
        SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/Hammer"));
        PrefixItem.ReforgeAnimationTime = 1;
        Helpers.Reforge(-2, false);
    }

    public override void OnFail()
    {
        base.OnFail();
        RepeatPrefixSystem.ClearCache();
    }

    public override void PostDrawBar(SpriteBatch spriteBatch)
    {
        var source = main.GetDimensions().ToRectangle();
        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);

        for (int i = 0; i < targets.Length; i++) //Draw windows of opportunity
        {
            var scrollbar = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar").Value;
            var color = Color.White;
            var wPos = source.Center() + new Vector2(-(dimensions.X / 2) + (dimensions.X * targets[i].Item1), -2);

            if (targets[i].Item2)
            {
                color = new Color(200, 200, 200);
                wPos.Y += 2;
            }

            spriteBatch.Draw(scrollbar, wPos, null, color * opacity, 0, scrollbar.Size() / 2, 1, SpriteEffects.None, 0);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.Graphics.Renderers;

namespace CraftingPlus.Common.UI.Minigames;

public class Forge : Minigame
{
    private readonly (float, bool)[] targets;
    private const float targetWindow = .1f;

    private float acceleration;

    public Forge()
    {
        targets = new (float, bool)[3];

        for (int i = 0; i < targets.Length; i++)
        {
            var lower = (.33f * i) + (targetWindow / 2);
            var upper = (1f / targets.Length * (i + 1)) - (targetWindow / 2);

            targets[i] = (MathHelper.Max(.2f, Main.rand.NextFloat(lower, upper)), false);
        }
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        //Reposition main to the reforge slot's location
        main.Left.Set(94, 0);
        main.Top.Set(267, 0);
    }

    public override void UpdateSelf()
    {
        PrefixItem.OverrideReforgePrice = 0;

        if (state != State.InProgress)
            return;

        score = MathHelper.Min(1, score + .009f * acceleration);
        acceleration = MathHelper.Min(acceleration + .01f, 1);
        delay = MathHelper.Lerp(delay, score, .05f);

        #region particles
        if (Main.rand.NextBool() && System.Math.Abs(delay - score) > .02f) //Add particles
        {
            var spark = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_Spark");

            particleLayer.AddParticle(new CreativeSacrificeParticle(spark, null, Main.rand.NextVector2Circular(4f, 3f), new Vector2(0, 15))
            {
                AccelerationPerFrame = new Vector2(0f, .164f),
                ScaleOffsetPerFrame = -1f / 45f
            });
        }
        #endregion

        if ((Main.mouseLeft && Main.mouseLeftRelease) || (Main.mouseRight && Main.mouseRightRelease)) //Mouse click
        {
            bool hit = false;

            for (int i = 0; i < targets.Length; i++)
            {
                if (System.Math.Abs(score - targets[i].Item1) < (targetWindow / 2))
                {
                    targets[i].Item2 = true;
                    hit = true;
                }
            }

            if (hit)
            {
                SoundEngine.PlaySound(SoundID.Item53 with { Pitch = 0 + score });
                SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/GearClick"));

                if (targets.All(x => x.Item2)) //Check if all buttons were hit
                    Complete();
            }
            else Fail();
        }

        if (score == 1)
            Fail();
    }

    public override bool Active() => base.Active() && Main.InReforgeMenu && Main.reforgeItem is not null && !Main.reforgeItem.IsAir;

    public override void OnComplete()
    {
        SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/Hammer"));

        //var prefix = UISystem.RandomizerState.Prefix;
        //if (prefix < 1)
        //    prefix = -2;

        Helpers.Reforge(/*prefix*/-2);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var source = main.GetDimensions().ToRectangle();

        #region progress bar
        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);
        Helpers.DrawResourceBar(spriteBatch, source.Center(), dimensions, score, Opacity);

        //Draw windows of opportunity
        for (int i = 0; i < targets.Length; i++)
        {
            var scrollbar = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar").Value;
            var color = (targets[i].Item2 ? new Color(200, 200, 200) : Color.White) * Opacity;
            var wPos = source.Center() + new Vector2(-(dimensions.X / 2) + (dimensions.X * targets[i].Item1), 0) + new Vector2(0, -2 * (targets[i].Item2 ? 0 : 1));

            spriteBatch.Draw(scrollbar, wPos, null, color, 0, scrollbar.Size() / 2, 1, SpriteEffects.None, 0);
        }
        #endregion

        #region item slot
        var cogA = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearA").Value;
        var cogB = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearB").Value;

        var scale = (float)System.Math.Sin(Opacity * 2);
        spriteBatch.Draw(cogB, source.TopLeft() + new Vector2(0, 30), null, Color.White, delay * -50, cogB.Size() / 2, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(cogA, source.TopLeft() + new Vector2(-5, 5), null, Color.White, delay * 50, cogA.Size() / 2, scale, SpriteEffects.None, 0);

        var invScale = .85f;
        var position = source.Left() - new Vector2(TextureAssets.InventoryBack.Width() / 2 * invScale, 0);
        var item = Main.reforgeItem;

        if (item is not null)
        {
            Helpers.DrawItemSlot(spriteBatch, ref item, ItemSlot.Context.PrefixItem, position - (TextureAssets.InventoryBack.Size() / 2 * invScale), invScale);

            if (state == State.Failed)
            {
                var x = Main.Assets.Request<Texture2D>("Images/CoolDown").Value;
                spriteBatch.Draw(x, position, null, Color.White * .75f, 0, x.Size() / 2, invScale, SpriteEffects.None, 0);
            }
        }
        #endregion
    }
}

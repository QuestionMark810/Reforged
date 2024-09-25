using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CraftingPlus.Common.UI.Minigames;

public class Workbench(Recipe recipe) : Minigame
{
    private readonly Recipe recipe = recipe;
    private readonly float target = Main.rand.NextFloat(.5f, 1f - targetWindow);
    private const float targetWindow = .1f;

    private bool releasedOnce = false;

    public override void UpdateSelf()
    {
        if (!(Main.mouseLeft || Main.mouseRight))
            releasedOnce = true;
        if (state != State.InProgress || !releasedOnce)
            return;

        delay = MathHelper.Lerp(delay, score, .05f);

        #region particles
        if (Main.rand.NextBool() && System.Math.Abs(delay - score) > .02f) //Add particles
        {
            var spark = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_Spark");

            particleLayer.AddParticle(new CreativeSacrificeParticle(spark, null, Main.rand.NextVector2Circular(4f, 3f), new Vector2(0, 18))
            {
                AccelerationPerFrame = new Vector2(0f, .164f),
                ScaleOffsetPerFrame = -1f / 45f
            });
        }
        #endregion

        if (Main.mouseLeft || Main.mouseRight) //Mouse hold
        {
            score = MathHelper.Min(1, score + .015f);

            if ((int)(score * 100f) % 5 == 0)
                SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/Gears") { Volume = .5f, Pitch = .1f });
        }
        else if (score > 0)
        {
            SoundEngine.PlaySound(new SoundStyle("CraftingPlus/Assets/Sounds/GearClick"));

            if (System.Math.Abs(score - target) > (targetWindow / 2))
                Fail();
            else
                Complete();
        }

        if (score == 1)
            Fail();
    }

    public override void OnComplete()
    {
        base.OnComplete();

        var item = recipe.createItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && !PrefixID.Sets.ReducedNaturalChance[p] && p != PrefixID.Annoying)
                allowed.Add(p);
        }

        Loader.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }

    public override void OnFail()
    {
        base.OnFail();

        var item = recipe.createItem;
        var allowed = new List<int>();

        for (int p = 1; p < (PrefixID.Count + PrefixLoader.PrefixCount); p++)
        {
            if (item.CanApplyPrefix(p) && (PrefixID.Sets.ReducedNaturalChance[p] || p == PrefixID.Annoying))
                allowed.Add(p);
        }

        Loader.CraftItemWithPrefix(recipe, (allowed.Count > 0) ? allowed[Main.rand.Next(allowed.Count)] : -1);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var source = main.GetDimensions().ToRectangle();

        #region progress bar
        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);
        Helpers.DrawResourceBar(spriteBatch, source.Center(), dimensions, score, Opacity);

        //Draw a window of opportunity
        var scrollbar = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar").Value;
        var color = Color.White;
        var Pos = source.Center() + new Vector2(-(dimensions.X / 2) + (dimensions.X * target), 0) - new Vector2(0, 2);

        if (state == State.Completed)
        {
            color = new Color(200, 200, 200);
            Pos.Y += 2;
        }
        spriteBatch.Draw(scrollbar, Pos, null, color * Opacity, 0, scrollbar.Size() / 2, 1, SpriteEffects.None, 0);
        #endregion

        #region item slot
        var cogA = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearA").Value;
        var cogB = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearB").Value;

        var scale = (float)System.Math.Sin(Opacity * 2);
        spriteBatch.Draw(cogB, source.TopLeft() + new Vector2(0, 30), null, Color.White, delay * -50, cogB.Size() / 2, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(cogA, source.TopLeft() + new Vector2(-5, 5), null, Color.White, delay * 50, cogA.Size() / 2, scale, SpriteEffects.None, 0);

        var invScale = .95f;
        var position = source.Left() - new Vector2(TextureAssets.InventoryBack.Width() / 2 * invScale, 0);
        var item = recipe.createItem;

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

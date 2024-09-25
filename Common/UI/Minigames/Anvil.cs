using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.Graphics.Renderers;

namespace CraftingPlus.Common.UI.Minigames;

public class Anvil(Recipe recipe) : Minigame
{
    private readonly Recipe recipe = recipe;
    private int timer = timerMax;

    private const int timerMax = 60 * 3;

    public override void UpdateSelf()
    {
        if (state != State.InProgress)
            return;
        if (timer - 1 == 0)
            Fail();

        timer--;
        score = MathHelper.Max(0, score - .0025f);
        delay = MathHelper.Lerp(delay, score, .05f);

        #region particles
        if (System.Math.Abs(delay - score) > .02f) //Add particles
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
            score = MathHelper.Min(1, score + .08f);
            SoundEngine.PlaySound(SoundID.Item53 with { MaxInstances = 3, Pitch = -1 + (score * 2) });

            if (score == 1)
                Complete();
        }
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

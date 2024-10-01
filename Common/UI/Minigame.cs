using CraftingPlus.Common.UI.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.UI;

namespace CraftingPlus.Common.UI;

public abstract class Minigame : AutoUI
{
    private static Asset<Texture2D> sparkTexture, cogATexture, cogBTexture;

    public enum State : byte
    {
        InProgress,
        Completed,
        Failed
    }
    public State state = State.InProgress;

    protected UIPanel main;
    private UIParticleLayer particleLayer;

    private int timer;
    private const int lingerTime = 30;

    public float Progress { get; protected set; }
    private float progressOld; //Used for visuals

    public Item displayItem;
    public float opacity;

    private void SetPosition() //Hardcode UI positions. This should be implemented better
    {
        if (Main.InReforgeMenu)
        {
            main.Left.Set(94, 0);
            main.Top.Set(267, 0);
        }
        else
        {
            main.Left.Set(70, 0);
            main.Top.Set(80, .5f);
        }

        Recalculate();
    }

    public override void OnInitialize()
    {
        sparkTexture = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_Spark");
        cogATexture = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearA");
        cogBTexture = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_GearB");

        main = new();
        SetPosition();
        main.Width.Set(250, 0);
        main.Height.Set(50, 0);
        main.SetPadding(0f);
        main.BackgroundColor = new Color(60, 80, 140, 220);

        particleLayer = new();

        main.Append(particleLayer);
        Append(main);
    }

    public void Reset() //Deprecated
    {
        state = State.InProgress;
        timer = 0;
        Progress = 0;

        OnActivate();
    }

    public override void OnActivate()
    {
        SetPosition();
        SoundEngine.PlaySound(SoundID.Research);
        progressOld = -.25f;
    }

    public virtual void OnClick() { }

    public virtual void OnRelease() { }

    public sealed override void Update(GameTime gameTime)
    {
        Update();

        particleLayer.Update(gameTime);
        opacity = MathHelper.Min(1, opacity + .1f); //Opacity effects
        main.BackgroundColor = new Color(60, 80, 140, 220) * opacity;

        if (state != State.InProgress)
            timer++;

        if (!Active())
            UISystem.GetState<Minigame>().UserInterface.SetState(null);
    }

    /// <summary> Put general update tasks here. </summary>
    public virtual void Update() { }

    public virtual bool Active() => Main.playerInventory && (timer <= lingerTime || state == State.Completed);

    protected sealed override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Main.LocalPlayer.mouseInterface = true;
        Main.hidePlayerCraftingMenu = true;
        progressOld = MathHelper.Lerp(progressOld, Progress, .05f);

        if (state == State.InProgress && Main.rand.NextBool() && System.Math.Abs(progressOld - Progress) > .02f) //Add particles
        {
            var spark = sparkTexture;
            particleLayer.AddParticle(new CreativeSacrificeParticle(spark, null, Main.rand.NextVector2Circular(4f, 3f), new Vector2(0, 15))
            {
                AccelerationPerFrame = new Vector2(0f, .164f),
                ScaleOffsetPerFrame = -1f / 45f
            });
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var source = main.GetDimensions().ToRectangle();

        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);
        DrawProgressBar(spriteBatch, source.Center(), dimensions, Progress, opacity);
        PostDrawBar(spriteBatch);

        var cogA = cogATexture.Value;
        var cogB = cogBTexture.Value;

        var scale = (float)System.Math.Sin(opacity * 2);
        spriteBatch.Draw(cogB, source.TopLeft() + new Vector2(0, 30), null, Color.White, progressOld * -50, cogB.Size() / 2, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(cogA, source.TopLeft() + new Vector2(-5, 5), null, Color.White, progressOld * 50, cogA.Size() / 2, scale, SpriteEffects.None, 0);

        var invScale = .85f;
        var position = source.Left() - new Vector2(TextureAssets.InventoryBack.Width() / 2 * invScale, 0);

        if (displayItem is not null)
        {
            Helpers.DrawItemSlot(spriteBatch, ref displayItem, ItemSlot.Context.PrefixItem, position - (TextureAssets.InventoryBack.Size() / 2 * invScale), invScale);

            if (state == State.Failed)
            {
                var x = TextureAssets.Cd.Value;
                spriteBatch.Draw(x, position, null, Color.White * .75f, 0, x.Size() / 2, invScale, SpriteEffects.None, 0);
            }
        }
    }

    private void DrawProgressBar(SpriteBatch spriteBatch, Vector2 position, Point size, float progress, float opacity = 1)
    {
        Color GetTint(Color color)
        {
            if (state == State.Completed)
                return Color.Lerp(color, Color.Transparent, timer / (float)lingerTime);

            return color;
        }

        var bar = TextureAssets.MagicPixel.Value;
        var frame = new Rectangle(0, 0, size.X, size.Y);

        //Draw the progress bar base
        spriteBatch.Draw(bar, position, frame with { Width = frame.Width + 2, Height = frame.Height + 2 }, Color.Black * opacity, 0, (frame.Size() / 2) + new Vector2(1), 1, SpriteEffects.None, 0);
        spriteBatch.Draw(bar, position, frame, Color.DarkSlateBlue * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);

        var loops = frame.Width;
        for (int i = 0; i < loops; i++)
        {
            var half = loops / 2;

            //Lerp in 2 phases to get a wider gradient
            var color = GetTint((i <= half) ? Color.Lerp(Color.Red, Color.Orange, (float)i / half) : Color.Lerp(Color.Orange, Color.White, (float)(i - half) / half));

            //Draw a progress bar endcap
            if (i >= (int)(loops * progress) - 3)
                color *= 3;
            if (i >= (int)(loops * progress))
                break;

            spriteBatch.Draw(bar, position + Vector2.UnitX * i, new Rectangle(0, 0, frame.Width / loops, frame.Height), color * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);
        }

        //Draw a highlight
        spriteBatch.Draw(bar, position + new Vector2(0, 2), new Rectangle(0, 0, (int)(frame.Width * progress), 8), GetTint(Color.White * .5f) * opacity, 0, frame.Size() / 2, 1, SpriteEffects.None, 0);
    }

    public virtual void PostDrawBar(SpriteBatch spriteBatch) { }

    public void Complete()
    {
        state = State.Completed;
        OnComplete();
    }

    public virtual void OnComplete() => SoundEngine.PlaySound(SoundID.ResearchComplete);

    public void Fail()
    {
        state = State.Failed;
        OnFail();
    }

    public virtual void OnFail() => SoundEngine.PlaySound(SoundID.NPCDeath30 with { Pitch = .5f });
}

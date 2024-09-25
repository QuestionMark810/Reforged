using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace CraftingPlus.Common.UI;

public abstract class Minigame : UIState
{
    public enum State : byte
    {
        InProgress,
        Completed,
        Failed
    }
    public State state = State.InProgress;

    protected UIPanel main;
    protected UIParticleLayer particleLayer;

    private int timer;
    private const int lingerTime = 30;

    public float Timeout => (float)timer / lingerTime;

    protected float score;
    protected float delay; //Used for visuals

    public float Opacity { get; private set; }

    public static void Enable(Minigame minigame) => UISystem.MinigameInterface?.SetState(minigame);

    public static void Disable() => UISystem.MinigameInterface?.SetState(null);

    public override void OnInitialize()
    {
        main = new();
        main.Left.Set(70, 0);
        main.Top.Set(80, .5f);
        main.Width.Set(250, 0);
        main.Height.Set(50, 0);
        main.SetPadding(0f);
        main.BackgroundColor = new Color(60, 80, 140, 220);

        particleLayer = new();

        main.Append(particleLayer);
        Append(main);
    }

    public override void OnActivate()
    {
        SoundEngine.PlaySound(SoundID.Research);
        delay = -.1f;

        //Disable some of our UIs
        //Randomizer.Disable();
    }

    public override void OnDeactivate()
    {
        timer = 0;
        Opacity = 0;
        state = State.InProgress;
        particleLayer.ClearParticles();
    }

    public sealed override void Update(GameTime gameTime)
    {
        //Opacity effects
        Opacity = MathHelper.Min(1, Opacity + .1f);
        main.BackgroundColor = new Color(60, 80, 140, 220) * Opacity;

        particleLayer.Update(gameTime);

        //if (state != State.InProgress)
        //    timer++;

        if (!Active())
            Disable();
    }

    public virtual bool Active() => Main.playerInventory && timer <= lingerTime;

    protected sealed override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Main.LocalPlayer.mouseInterface = true;
        Main.hidePlayerCraftingMenu = true;

        UpdateSelf();
    }

    public virtual void UpdateSelf() { }

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

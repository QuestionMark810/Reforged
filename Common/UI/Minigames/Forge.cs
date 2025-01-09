using ReLogic.Content;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;

namespace Reforged.Common.UI.Minigames;

public class Forge : Minigame
{
    private struct Target(float position, float size, bool bonus = false)
    {
        public readonly float position = position;
        public readonly float size = size;
        public readonly bool bonus = bonus;
        public bool hit;

        public readonly bool InTarget(float point) => Math.Abs(point - position) < (size / 2);

        public readonly void Draw(SpriteBatch spriteBatch, Vector2 position, float opacity)
        {
            var scrollbar = targetTexture.Value;
            var color = Color.White;

            if (bonus)
            {
                var star = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Rank_" + (hit ? "Dim" : "Light")).Value;
                spriteBatch.Draw(star, position - new Vector2(0, 13), null, color * opacity, 0, star.Size() / 2, 1, SpriteEffects.None, 0);
            }

            if (hit)
            {
                color = new Color(200, 200, 200);
                position.Y += 2;
            }

            var source = bonus ? new Rectangle(22, 0, 10, 16) : new Rectangle(0, 0, 20, 16);
            spriteBatch.Draw(scrollbar, position, source, color * opacity, 0, source.Size() / 2, 1, SpriteEffects.None, 0);
        }
    }

    private static Asset<Texture2D> targetTexture;
    private readonly Target[] targets;
    private float acceleration;

    public Forge()
    {
        bool bonus = Main.rand.NextBool(3); //The chance of a bonus appearing
        float bonusPos = Main.rand.Next(3); //The position of the bonus relative to other targets

        displayItem = Main.reforgeItem;
        targets = new Target[bonus ? 4 : 3];

        for (int i = 0; i < targets.Length; i++)
        {
            bool atBonusTarget = bonus && i == bonusPos;
            float targetWindow = atBonusTarget ? .055f : .11f;

            float half = targetWindow / 2f;
            float div = 1f / targets.Length;

            var lower = (div * i) + half;
            var upper = (div * (i + 1)) - half;

            targets[i] = new Target(MathHelper.Max(.2f, Main.rand.NextFloat(lower, upper)), targetWindow, atBonusTarget);
        }
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        targetTexture = ModContent.GetInstance<Reforged>().Assets.Request<Texture2D>("Assets/Textures/Targets");
    }

    public override void OnClick()
    {
        bool hitAny = false;
        for (int i = 0; i < targets.Length; i++)
            if (targets[i].InTarget(Progress))
            {
                targets[i].hit = true;
                hitAny = true;
            }

        if (hitAny)
        {
            SoundEngine.PlaySound(SoundID.Item53 with { Pitch = 0 + Progress });
            SoundEngine.PlaySound(new SoundStyle(Reforged.assetKey + "Sounds/GearClick"));

            if (targets.Where(x => !x.bonus).All(x => x.hit) || targets.Where(x => x.bonus && x.hit).Any())
                Complete();
        }
        else Fail();
    }

    public override void Update()
    {
        PrefixItem.OverrideReforgePrice = 0;
        if (state != State.InProgress)
            return;

        Complete(); //DEBUG

        float rateMult = MathHelper.Min(RepeatPrefix.Count * .05f, 1) + 1;
        Progress = MathHelper.Min(1, Progress + .009f * acceleration * rateMult);
        acceleration = MathHelper.Min(acceleration + .01f * rateMult, 1);

        if (Progress == 1)
            Fail();
    }

    public override bool CheckActive() => (base.CheckActive() || state == State.Completed) 
        && Main.InReforgeMenu && Main.reforgeItem is not null && !Main.reforgeItem.IsAir;

    public override void OnComplete()
    {
        SoundEngine.PlaySound(new SoundStyle(Reforged.assetKey + "Sounds/Hammer"));
        Helpers.Reforge(-2, false);

        if (PrefixItem.RolledRarePrefix())
        {
            SoundEngine.PlaySound(SoundID.NPCHit5 with { Pitch = .3f, Volume = .5f });
            SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -1f, Volume = .1f });

            PrefixItem.StartAnimation(50);
        }
        else
        {
            PrefixItem.StartAnimation(30);
        }
    }

    public override void OnFail()
    {
        base.OnFail();
        RepeatPrefix.Reset();
    }

    public override void PostDrawBar(SpriteBatch spriteBatch)
    {
        var source = main.GetDimensions().ToRectangle();
        var dimensions = new Point((int)(main.Width.Pixels * .75f), 14);

        foreach (var target in targets)
        {
            var pos = source.Center() + new Vector2(-(dimensions.X / 2) + (dimensions.X * target.position), -3);
            target.Draw(spriteBatch, pos, opacity);
        }
    }
}

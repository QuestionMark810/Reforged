/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Audio;
using Terraria.Localization;

namespace CraftingPlus.Common.UI;

public class Randomizer : UIState
{
    private const int hiddenPos = -10000;

    private UIElement main;
    private UIList list;
    private UIScrollbar scrollbar;

    private bool listIsUp;
    private float shaker;

    /// <summary> Don't assign to this directly - use <see cref="SetPrefix"/> instead.</summary>
    private int prefix;
    public int Prefix => prefix;

    public void ShakeButton()
    {
        foreach (var element in list)
        {
            if (element is PrefixInfo info && info.Prefix == prefix)
                info.Shake();
        }
    }

    public void SetPrefix(int prefix)
    {
        this.prefix = prefix;

        if (prefix > 0)
        {
            var dummy = Main.reforgeItem.Clone();
            dummy.Prefix(prefix);

            PrefixItem.OverrideReforgePrice = dummy.value * dummy.stack * 2;
        }
        else PrefixItem.OverrideReforgePrice = 0;
    }

    public static void Enable() => UISystem.RandomizerInterface?.SetState(UISystem.RandomizerState);

    public static void Disable() => UISystem.RandomizerInterface?.SetState(null);

    public override void OnInitialize()
    {
        main = new();
        main.Left.Set(140 - 70, 0);
        main.Top.Set(294, 0);
        main.Width.Set(130, 0);
        main.Height.Set(250, 0);
        main.SetPadding(0f);

        list = new();
        list.Top.Set(50, 0);
        list.Width.Set(130, 0);
        list.Height.Set(200, 0);
        list.MaxWidth.Set(200, 0);
        list.ListPadding = 4f;
        list.SetScrollbar(scrollbar = new());

        scrollbar.Width.Set(20, 0);
        scrollbar.Height.Set(list.Height.Pixels, 0);
        scrollbar.Left.Set(hiddenPos, 0);

        main.Append(list);
        list.Append(scrollbar);
        Append(main);
    }

    private void ToggleList(bool up, bool previewEntryUp = false)
    {
        if (up)
        {
            if (previewEntryUp && list.Count == 1)
                list.Clear();

            if (list.Count < 2)
            {
                var item = Main.reforgeItem.Clone();
                item.ResetPrefix();

                for (int p = 1; p < PrefixID.Count; p++)
                {
                    if (item.CanApplyPrefix(p))
                        list.Add(new PrefixInfo(p));
                }

                scrollbar.Left.Set(list.Width.Pixels - scrollbar.Width.Pixels - 10, 0); //Reveal the scrollbar
            }
        }
        else if (list.Count > 0)
        {
            if (!(previewEntryUp && list.Count == 1))
                list.Clear();

            scrollbar.Left.Set(hiddenPos, 0); //Hide the scrollbar
        }

        if (previewEntryUp)
        {
            if (list.Count == 0 && !up)
                list.Add(new PrefixInfo(prefix));
        }
        else if (list.Count == 1)
            list.Clear();

        listIsUp = up || previewEntryUp;
    }

    public override void OnActivate() => SetPrefix(0);

    public override void OnDeactivate() => ToggleList(false);

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (shaker > 0)
            shaker -= .05f;

        if (Main.mouseReforge)
            ToggleList(false, true);
        if (listIsUp)
        {
            ToggleList(list.IsMouseHovering, main.IsMouseHovering);

            if (list.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }

        if (!Main.InReforgeMenu || Main.reforgeItem == null || Main.reforgeItem.IsAir)
            Disable();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //Avoid calling base.Draw for flexibility
        list.Draw(spriteBatch);
        scrollbar.Draw(spriteBatch);
    }
}

internal class PrefixInfo : UIPanel
{
    public int Prefix => prefix;

    private readonly int prefix;

    private float shaker;

    public void Shake() => shaker = 1;

    public PrefixInfo(int prefix)
    {
        Width.Set(100, 0);
        Height.Set(30, 0);
        BorderColor = Color.Black * .6f;

        OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) =>
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };
        OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => 
        {
            var state = UISystem.RandomizerState;

            if (prefix == state.Prefix) //Clear prefix
                state.SetPrefix(0);
            else //Select prefix
                state.SetPrefix(prefix);
        };

        this.prefix = prefix;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        bool Selected() => UISystem.RandomizerState.Prefix == prefix;

        base.DrawSelf(spriteBatch);

        BackgroundColor = Selected() ? new Color(60, 80, 180, 220) : new Color(60, 50, 120, 220);

        var text = Lang.prefix[prefix].Value;
        if (text == string.Empty)
            text = Language.GetTextValue("Mods.CraftingPlus.UI.Random");

        var color = Selected() ? Color.Yellow : Main.MouseTextColorReal;
        var shakerOffsetX = (float)System.Math.Sin(shaker * 10f) * 5f * shaker;

        if (shaker > 0)
        {
            BackgroundColor = Color.Lerp(new Color(60, 80, 180, 220), Color.DarkRed with { A = 220 }, shaker);
            color = Color.Lerp(Main.MouseTextColorReal, Color.Yellow, shaker);

            shaker -= .05f;
        }
        if (IsMouseHovering)
        {
            BackgroundColor *= 1.5f;
            color *= 1.5f;
        }

        Utils.DrawBorderString(spriteBatch, text, GetDimensions().Center() + new Vector2(shakerOffsetX, 3), color, 1, .5f, .5f);
    }
}*/

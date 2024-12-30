using Reforged.Common.UI.Core;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace Reforged.Common.UI;

/// <summary> Replaces the Tinkerer's default reforge menu. </summary>
public class ReforgeMenu : AutoUI
{
    internal static Vector2 ButtonOffset { get; set; }
    internal static Vector2 TextPosition { get; set; } = new Vector2(0, 50);

    private static Asset<Texture2D> redoTexture, gearTexture;

    private UIElement main;
    private UIImageButton reforgeButton;
    private ReforgeSlot reforgeSlot;

    public override void OnInitialize()
    {
        var mod = ModContent.GetInstance<Reforged>();
        redoTexture = mod.Assets.Request<Texture2D>("Assets/Textures/RedoIcon");
        gearTexture = mod.Assets.Request<Texture2D>("Assets/Textures/GearIcon");

        main = new();
        main.Left.Set(50, 0);
        main.Top.Set(270, 0);
        main.Width.Set(130, 0);
        main.Height.Set(250, 0);
        main.SetPadding(0f);

        reforgeButton = new(TextureAssets.Reforge[0]);
        reforgeButton.SetHoverImage(TextureAssets.Reforge[1]);
        reforgeButton.SetVisibility(1, 1);
        reforgeButton.Left.Set(ButtonOffset.X, 0);
        ShowButton(false);
        reforgeButton.OnLeftMouseDown += OnClickReforge;

        reforgeSlot = new(.85f);
        reforgeSlot.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => RepeatPrefix.Reset();

        main.Append(reforgeButton);
        main.Append(reforgeSlot);
        Append(main);
    }

    private void OnClickReforge(UIMouseEvent evt, UIElement listeningElement)
    {
        if (UISystem.GetState<Minigame>().UserInterface.CurrentState is Minigame minigame)
        {
            if (minigame.state == Minigame.State.Completed) //Reset
            {
                UISystem.GetState<Minigame>().UserInterface.SetState(new Minigames.Forge() { opacity = 1 });
                return;
            }

            minigame.OnClick();
        }
        else
        {
            GetCost(out int value);
            if (Main.LocalPlayer.CanAfford(value) && ItemLoader.CanReforge(Main.reforgeItem))
            {
                Main.LocalPlayer.BuyItem(value);
                UISystem.GetState<Minigame>().UserInterface.SetState(new Minigames.Forge());
            }
        }
    }

    private void ShowButton(bool value)
    {
        if (value)
            reforgeButton.Top.Set(50 + ButtonOffset.Y, 0);
        else
            reforgeButton.Top.Set(-999, 0);
    }

    public override void OnDeactivate() => ShowButton(false);

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Main.LocalPlayer.chest != -1 || Main.npcShop != 0 || Main.LocalPlayer.talkNPC == -1 || Main.InGuideCraftMenu)
        {
            Main.InReforgeMenu = false;
            Main.LocalPlayer.dropItemCheck();
            Recipe.FindRecipes();
        }

        if (!Main.InReforgeMenu)
            UISystem.GetState<ReforgeMenu>().UserInterface.SetState(null);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (reforgeButton.IsMouseHovering)
        {
            Main.mouseReforge = true;
            Main.LocalPlayer.mouseInterface = true;

            var state = UISystem.GetState<Minigame>().UserInterface.CurrentState;
            if (state is not Minigame minigame || minigame.state == Minigame.State.Completed)
                Main.hoverItemName = Lang.inter[19].Value;
        }
        else
        {
            Main.mouseReforge = false;
        }

        ShowButton(!Main.reforgeItem.IsAir);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        var state = UISystem.GetState<Minigame>().UserInterface.CurrentState;
        if (state is Minigame minigame) //Draw custom icons
        {
            if (minigame.state == Minigame.State.InProgress)
            {
                var texture = gearTexture.Value;
                int frameY = reforgeButton.IsMouseHovering ? 1 : 0;
                var source = texture.Frame(1, 2, 0, frameY, 0, -2);
                float rotation = (float)Main.timeForVisualEffects * .1f;

                spriteBatch.Draw(texture, reforgeButton.GetDimensions().Position() + new Vector2(20, 22),
                    source, Color.White, rotation, source.Size() / 2, 1, SpriteEffects.None, 0);

                return;
            }

            if (minigame.state == Minigame.State.Completed)
            {
                var texture = redoTexture.Value;
                int frameY = reforgeButton.IsMouseHovering ? 1 : 0;
                var source = texture.Frame(1, 2, 0, frameY, 0, -2);

                spriteBatch.Draw(texture, reforgeButton.GetDimensions().Position() + new Vector2(20, 22),
                    source, Color.White, 0, source.Size() / 2, 1, SpriteEffects.None, 0);
            }
        }

        var pos = main.GetDimensions().Position() + ((state is null) ? Vector2.Zero : TextPosition);
        string text = Lang.inter[46].Value + ": ";

        if (Main.reforgeItem.type > ItemID.None)
        {
            ItemSlot.DrawSavings(spriteBatch, pos.X + 130, Main.instance.invBottom, horizontal: true);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, GetCost(out _),
                new Vector2(pos.X + 50 + FontAssets.MouseText.Value.MeasureString(text).X, pos.Y), Color.White, 0f, Vector2.Zero, Vector2.One);
        }
        else
            text = Lang.inter[20].Value;

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
            new Vector2(pos.X + 50, pos.Y), Main.MouseTextColorReal, 0f, Vector2.Zero, Vector2.One);
    }

    private static string GetCost(out int value) //Adapted from vanilla code 
    {
        int num69 = Main.reforgeItem.value * Main.reforgeItem.stack;
        bool canApplyDiscount = true;

        if (ItemLoader.ReforgePrice(Main.reforgeItem, ref num69, ref canApplyDiscount))
        {
            if (canApplyDiscount && Main.LocalPlayer.discountAvailable)
                num69 = (int)((double)num69 * 0.8);

            num69 = (int)((double)num69 * Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);
            num69 /= 3;
        }

        string text2 = string.Empty;
        int num70 = 0;
        int num72 = 0;
        int num73 = 0;
        int num74 = 0;
        int num75 = num69;

        if (num75 < 1)
            num75 = 1;
        if (num75 >= 1000000)
        {
            num70 = num75 / 1000000;
            num75 -= num70 * 1000000;
        }
        if (num75 >= 10000)
        {
            num72 = num75 / 10000;
            num75 -= num72 * 10000;
        }
        if (num75 >= 100)
        {
            num73 = num75 / 100;
            num75 -= num73 * 100;
        }
        if (num75 >= 1)
            num74 = num75;

        if (num70 > 0)
            text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + num70 + " " + Lang.inter[15].Value + "] ";
        if (num72 > 0)
            text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + num72 + " " + Lang.inter[16].Value + "] ";
        if (num73 > 0)
            text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + num73 + " " + Lang.inter[17].Value + "] ";
        if (num74 > 0)
            text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + num74 + " " + Lang.inter[18].Value + "] ";

        if (num69 == 0) //No value
            text2 = "[c/" + Main.MouseTextColorReal.Hex3() + ":" + Language.GetTextValue(Reforged.locKey + "Misc.None") + "] ";

        value = num69;
        return text2;
    }
}

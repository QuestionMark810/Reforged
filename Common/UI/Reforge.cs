using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CraftingPlus.Common.UI;

public class Reforge : UIState
{
    private UIElement main;
    private UIImageButton reforgeButton;
    private InventorySlot reforgeSlot;

    public static void Enable() => UISystem.ReforgeInterface?.SetState(UISystem.ReforgeState);

    public static void Disable() => UISystem.ReforgeInterface?.SetState(null);

    public override void OnInitialize()
    {
        main = new();
        main.Left.Set(50, 0);
        main.Top.Set(270, 0);
        main.Width.Set(130, 0);
        main.Height.Set(250, 0);
        main.SetPadding(0f);

        reforgeButton = new(TextureAssets.Reforge[0]);
        reforgeButton.SetHoverImage(TextureAssets.Reforge[1]);
        reforgeButton.SetVisibility(1, 1);
        reforgeButton.Left.Set(0, 0);
        reforgeButton.Top.Set(50, 0);
        reforgeButton.OnLeftClick += OnReforge;

        reforgeSlot = new(Main.reforgeItem, .85f);

        main.Append(reforgeButton);
        main.Append(reforgeSlot);
        Append(main);
    }

    private void OnReforge(UIMouseEvent evt, UIElement listeningElement)
    {
        static bool ForgeActive()
        {
            var miniState = UISystem.MinigameInterface.CurrentState;
            return miniState is not null && miniState is Minigames.Forge forge && forge.state == Minigame.State.InProgress;
        }
        GetCost(out int value);

        if (Main.LocalPlayer.CanAfford(value) && ItemLoader.CanReforge(Main.reforgeItem) && !ForgeActive())
        {
            Main.LocalPlayer.BuyItem(value);
            Minigame.Enable(new Minigames.Forge());
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!Main.InReforgeMenu || Main.LocalPlayer.TalkNPC is null || Main.LocalPlayer.TalkNPC.type != NPCID.GoblinTinkerer)
            Disable();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (reforgeButton.IsMouseHovering)
        {
            Main.mouseReforge = true;
            Main.LocalPlayer.mouseInterface = true;
            Main.hoverItemName = Lang.inter[19].Value;
        }
        else
        {
            Main.mouseReforge = false;
        }

        Main.reforgeItem = reforgeSlot.Item;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        var miniState = UISystem.MinigameInterface.CurrentState;

        if (miniState is not null && miniState is Minigame minigame && minigame.state == Minigame.State.InProgress)
            return;

        //int x = 50;
        //int y = 270;
        Vector2 pos = reforgeButton.GetDimensions().Position() - ((miniState is null) ? new Vector2(0, 50) : Vector2.Zero);
        string text = Lang.inter[46].Value + ": ";

        if (Main.reforgeItem.type > ItemID.None)
        {
            ItemSlot.DrawSavings(spriteBatch, pos.X + 130, Main.instance.invBottom, horizontal: true);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, GetCost(out _), new Vector2(pos.X + 50 + FontAssets.MouseText.Value.MeasureString(text).X, pos.Y), Color.White, 0f, Vector2.Zero, Vector2.One);
        }
        else
            text = Lang.inter[20].Value;

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2(pos.X + 50, pos.Y), Main.MouseTextColorReal, 0f, Vector2.Zero, Vector2.One);
    }

    private static string GetCost(out int value) //Adapted vanilla code 
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
            text2 = "[c/" + Main.MouseTextColorReal.Hex3() + ":" + "None" + "] ";

        value = num69;
        return text2;
    }
}

internal class InventorySlot : UIElement
{
    internal Item Item;
    private readonly float _scale;
    internal float opacity;

    public InventorySlot(Item item, float scale = 1f, float opacity = 1f)
    {
        Item = item;
        _scale = scale;
        this.opacity = opacity;

        Width.Set(TextureAssets.InventoryBack9.Value.Width * scale, 0f);
        Height.Set(TextureAssets.InventoryBack9.Value.Height * scale, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Color oldColor = Main.inventoryBack;
        Main.inventoryScale = _scale;
        Main.inventoryBack = Color.Lerp(Color.Transparent, oldColor, opacity);

        var context = ItemSlot.Context.PrefixItem;

        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.craftingHide = true;
            ItemSlot.LeftClick(ref Item, context);

            if (Main.mouseLeftRelease && Main.mouseLeft)
                Recipe.FindRecipes();

            ItemSlot.RightClick(ref Item, context);
            ItemSlot.MouseHover(ref Item, context);
        }
        ItemSlot.Draw(spriteBatch, ref Item, context, GetDimensions().ToRectangle().TopLeft());

        Main.inventoryScale = oldScale;
        Main.inventoryBack = oldColor;
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Reforged.Common.UI.Core;

public class UISystem : ModSystem
{
    private static readonly HashSet<AutoUI> UIStates = [];

    internal static T GetState<T>() where T : AutoUI => UIStates.FirstOrDefault(x => x is T) as T;
    //internal static void SetActive<T>(bool active) where T : AutoUI => GetState<T>().UserInterface.SetState(active ? GetState<T>() : null);

    public override void Load()
    {
        var uiStates = Mod.Code.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(AutoUI)));
        foreach (var state in uiStates)
        {
            var s = Activator.CreateInstance(state) as AutoUI;
            s.UserInterface = new UserInterface();

            UIStates.Add(s);
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        foreach (var state in UIStates)
            state.UserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        foreach (var state in UIStates)
        {
            int index = state.Layer(layers);
            if (index != -1)
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "Reforged: UI" + state.UniqueId,
                    delegate
                    {
                        state.UserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
        }
    }
}
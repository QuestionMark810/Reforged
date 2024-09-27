using System.Collections.Generic;
using Terraria.UI;

namespace CraftingPlus.Common.UI.Core;

public abstract class AutoUI : UIState
{
    public UserInterface UserInterface { get; set; }

    public virtual int Layer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
}

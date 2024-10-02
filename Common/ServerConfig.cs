using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Reforged.Common;

internal class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Range(0, 10)]
    [Slider]
    [DefaultValue(1f)]
    public float ReforgeCostMultiplier { get; set; }
}

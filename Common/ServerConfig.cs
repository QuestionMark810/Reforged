using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Reforged.Common;

internal class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(1f)]
    [Range(0f, 10f)]
    [Increment(.25f)]
    public float reforgeMult;
}

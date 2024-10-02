using Reforged.Common.OverDust;
using Terraria.ID;

namespace Reforged.Content.Dusts;

public class SparkDust : ModDust, IOverDust
{
    public override string Texture => "Terraria/Images/Dust";

    public override void OnSpawn(Dust dust) => dust.frame = Texture2D.Frame(100, 12, DustID.Torch, Main.rand.Next(3), -2, -2);

    public override bool Update(Dust dust)
    {
        dust.velocity *= .98f;
        dust.position += dust.velocity;

        if ((dust.scale -= .025f) <= 0)
            dust.active = false;

        return false;
    }

    public override bool PreDraw(Dust dust) => false;

    public void CustomDraw(Dust dust)
    {
        int length = 5;
        for (int i = 0; i < length; i++)
        {
            var position = dust.position - (Vector2.Normalize(dust.velocity) * i) - Main.screenPosition;
            var color = Color.Lerp(Color.Orange, Color.Red, i / (float)length) with { A = 0 };

            Main.spriteBatch.Draw(Texture2D.Value, position, dust.frame, color, 0, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0);
        }
    }
}

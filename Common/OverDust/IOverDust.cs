namespace CraftingPlus.Common.OverDust;

internal interface IOverDust
{
    public void CustomDraw(Dust dust) => DustLoader.GetDust(dust.type).PreDraw(dust);
}

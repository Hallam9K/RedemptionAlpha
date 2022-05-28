using Terraria;

namespace Redemption
{
    public static class RedeDetours
    {
        public static void Initialize()
        {
            On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
        }
        private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            if (!Main.dedServ)
                RedeSystem.TrailManager.DrawTrails(Main.spriteBatch);

            orig(self);
        }
    }
}

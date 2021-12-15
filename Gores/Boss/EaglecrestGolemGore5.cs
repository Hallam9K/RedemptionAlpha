using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Boss
{
    public class EaglecrestGolemGore5 : ModGore
    {
        public override void OnSpawn(Gore gore)
        {
            gore.Frame = new SpriteFrame(1, 3, 0, (byte)Main.rand.Next(3));
        }
    }
}
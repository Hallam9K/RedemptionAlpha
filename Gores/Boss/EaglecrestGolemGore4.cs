using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Boss
{
    public class EaglecrestGolemGore4 : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(1, 2, 0, (byte)Main.rand.Next(2));
        }
    }
}
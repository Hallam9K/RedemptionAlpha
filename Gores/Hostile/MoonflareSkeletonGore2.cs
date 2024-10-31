using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Hostile
{
    public class MoonflareSkeletonGore2 : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(5, 1, (byte)Main.rand.Next(5), 0);
        }
    }
}
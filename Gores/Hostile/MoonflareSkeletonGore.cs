using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Hostile
{
    public class MoonflareSkeletonGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(1, 4, 0, (byte)Main.rand.Next(4));
        }
    }
}
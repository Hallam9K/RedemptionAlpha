using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Hostile
{
    public class GreedySkeletonGore : ModGore
    {
        public override void OnSpawn(Gore gore)
        {
            gore.Frame = new SpriteFrame(5, 1, (byte)Main.rand.Next(5), 0);
        }
    }
}
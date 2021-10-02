using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Hostile
{
    public class CorpseWalkerGore : ModGore
    {
        public override void OnSpawn(Gore gore)
        {
            gore.Frame = new SpriteFrame(1, 4, 0, (byte)Main.rand.Next(4));
        }
    }
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ShadesteelChainTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileRope[Type] = true;
            DustType = ModContent.DustType<ShadesteelDust>();
            MinPick = 100;
            MineResist = 7f;
            HitSound = CustomSounds.ChainHit;
            AddMapEntry(new Color(83, 87, 123));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
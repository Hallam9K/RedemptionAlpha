using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class AncientSpikeTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            TileID.Sets.TouchDamageBleeding[Type] = true;
            TileID.Sets.TouchDamageImmediate[Type] = 200;
            DustType = DustID.Lead;
            MinPick = 1000;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(250, 240, 200));
		}
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
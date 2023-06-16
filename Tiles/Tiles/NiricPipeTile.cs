using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class NiricPipeTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            DustType = ModContent.DustType<NiricBrassDust>();
            MinPick = 210;
            MineResist = 5f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(149, 106, 54));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
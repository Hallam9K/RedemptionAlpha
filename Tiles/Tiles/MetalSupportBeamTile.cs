using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class MetalSupportBeamTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 6f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(105, 107, 114));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}
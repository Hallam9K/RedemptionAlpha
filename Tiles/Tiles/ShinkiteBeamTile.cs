using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ShinkiteBeamTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            DustType = ModContent.DustType<ShinkiteDust>();
            MinPick = 200;
            MineResist = 4f;
            HitSound = CustomSounds.BrickHit;
            AddMapEntry(new Color(90, 59, 62));
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
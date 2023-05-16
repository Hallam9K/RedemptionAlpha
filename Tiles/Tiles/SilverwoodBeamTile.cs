using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class SilverwoodBeamTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            DustType = DustID.Pearlwood;
            ItemDrop = ModContent.ItemType<SilverwoodBeam>();
            MinPick = 50;
            MineResist = 7f;
            AddMapEntry(new Color(116, 102, 84));
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
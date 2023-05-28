using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Ores
{
    public class DragonLeadOre2Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileStone[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            DustType = DustID.Stone;
            RegisterItemDrop(ItemID.StoneBlock);
            MinPick = 10;
            MineResist = 1.4f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(138, 138, 138));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}
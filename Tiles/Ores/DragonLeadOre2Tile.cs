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
            TileID.Sets.Ore[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileOreFinderPriority[Type] = 320;
            DustType = DustID.Stone;
			ItemDrop = ItemID.StoneBlock;
            MinPick = 10;
            MineResist = 1.4f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Stone?");
            AddMapEntry(new Color(138, 138, 138));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}
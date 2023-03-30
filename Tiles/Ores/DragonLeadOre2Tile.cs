using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
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
			ItemDrop = ItemID.StoneBlock;
            MinPick = 10;
            MineResist = 1.4f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Stone?");
            AddMapEntry(new Color(138, 138, 138));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}
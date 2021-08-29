using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class PlantMatterTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = DustID.GrassBlades;
			ItemDrop = ModContent.ItemType<PlantMatter>();
            MinPick = 0;
            MineResist = 1f;
            SoundType = SoundID.Grass;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Plant Matter");
            AddMapEntry(new Color(109, 155, 67), name);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Trophies
{
	public class KS3TrophyTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(Type);
			DustType = DustID.WoodFurniture;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("King Slayer III Trophy");
			AddMapEntry(new Color(120, 85, 60), name);
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 16, ModContent.ItemType<KS3Trophy>());
		}
	}
}
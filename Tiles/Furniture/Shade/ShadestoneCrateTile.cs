using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Shade;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneCrateTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
			name.SetDefault("Shadestone Crate");
			AddMapEntry(new Color(59, 61, 87), name);
			DustType = ModContent.DustType<ShadestoneDust>();
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<ShadestoneCrate>());
		}
	}
}
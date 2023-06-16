using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Items.Placeable.Furniture.Misc;

namespace Redemption.Tiles.Furniture.Misc
{
    public class ThornPlushTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 26 };
            TileObjectData.newTile.DrawYOffset = -8;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Thorn Plushie");
			AddMapEntry(new Color(151, 198, 146), name);
            DustType = DustID.Silk;
            RegisterItemDrop(ModContent.ItemType<ThornPlush>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}
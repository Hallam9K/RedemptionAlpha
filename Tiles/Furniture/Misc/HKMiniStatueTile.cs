using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Misc;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Misc
{
    public class HKMiniStatueTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[]{ 18, 16, 16, 18 };
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
            MinPick = 50;
            MineResist = 7f;
            AddMapEntry(new Color(104, 91, 83));
            DustType = ModContent.DustType<SlateDust>();
        }
		public override bool CanExplode(int i, int j) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 64, ModContent.ItemType<HKMiniStatue>());
	}
}
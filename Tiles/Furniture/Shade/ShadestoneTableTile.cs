using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneTableTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            // Properties
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; 
			
			DustType = ModContent.DustType<ShadestoneDust>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			AddMapEntry(new Color(59, 61, 87), Language.GetText("MapObject.Table"));
        }

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}

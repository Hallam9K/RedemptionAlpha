using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabReceptionDeskTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 7f;
			LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Laboratory Reception Desk");
            AddMapEntry(new Color(189, 191, 200), name);
            AdjTiles = new int[] { TileID.Tables };
        }
        public override bool CanExplode(int i, int j) => false;
    }
}
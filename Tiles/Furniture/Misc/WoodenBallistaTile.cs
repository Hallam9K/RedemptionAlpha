using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class WoodenBallistaTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 22 };
            TileObjectData.newTile.Origin = new Point16(4, 3);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = -4;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            MinPick = 10;
            MineResist = 2f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Trojan Chicken Replica");
            AddMapEntry(new Color(151, 107, 75), name);
        }
        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 8;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 4;
            for (int x = left; x < left + 8; x++)
            {
                for (int y = top; y < top + 4; y++)
                {
                    if (Main.tile[x, y].TileFrameY >= 78)
                        Main.tile[x, y].TileFrameY -= 78;
                    else
                        Main.tile[x, y].TileFrameY += 78;
                }
            }
            return true;
        }
    }
}
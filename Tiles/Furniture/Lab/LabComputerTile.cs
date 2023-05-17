using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabComputerTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 3f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Laboratory Computer");
            AddMapEntry(new Color(189, 191, 200), name);
        }
        public override bool CanExplode(int i, int j) => false;
    }
}
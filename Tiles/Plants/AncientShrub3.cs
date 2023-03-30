using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;

namespace Redemption.Tiles.Plants
{
    public class AncientShrub3 : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<AncientGrassTile>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            DustType = DustID.GrassBlades;
            HitSound = SoundID.Grass;
            ItemDrop = -1;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 10;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.rand.NextBool(50))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<AncientGrassSeeds>());
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
    }
}
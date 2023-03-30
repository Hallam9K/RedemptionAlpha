using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Metadata;

namespace Redemption.Tiles.Plants
{
    public class AncientStoneFoliage : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<GathicGladestoneBrickTile>(), ModContent.TileType<GathicGladestoneTile>(), ModContent.TileType<AncientHallBrickTile>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 7;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.addTile(Type);
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 10;
        }
    }
    public class AncientStoneFoliageC : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<GathicGladestoneBrickTile>(), ModContent.TileType<GathicGladestoneTile>(), ModContent.TileType<AncientHallBrickTile>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 7;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.addTile(Type);
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 10;
        }
    }
}
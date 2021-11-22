using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedSandstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedSandTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedHardenedSandTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedHardenedSandTile>()][Type] = true;
            Main.tileMerge[Type][TileID.Sandstone] = true;
            Main.tileMerge[TileID.Sandstone][Type] = true;
            Main.tileMerge[Type][TileID.CorruptSandstone] = true;
            Main.tileMerge[TileID.CorruptSandstone][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonSandstone] = true;
            Main.tileMerge[TileID.CrimsonSandstone][Type] = true;
            Main.tileMerge[Type][TileID.HallowSandstone] = true;
            Main.tileMerge[TileID.HallowSandstone][Type] = true;
            TileID.Sets.Conversion.Sandstone[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.isDesertBiomeSand[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			AddMapEntry(new Color(137, 120, 112));
            MineResist = 2.5f;
            DustType = DustID.Ash;
            ItemDrop = ModContent.ItemType<IrradiatedSandstone>();
		}
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j - 2);

            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<StarliteGemTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<StarliteGemTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<GrubNestTile>());
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<GrubNestTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && !tileAbove2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<RadioactiveSandstoneStalagmitesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<RadioactiveSandstoneStalagmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<RadioactiveSandstoneStalagmites2Tile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<RadioactiveSandstoneStalagmites2Tile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && !tileBelow2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveSandstoneStalacmitesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<RadioactiveSandstoneStalacmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<RadioactiveSandstoneStalacmites2Tile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<RadioactiveSandstoneStalacmites2Tile>(), 0, 0, -1, -1);
            }
        }
    }
}


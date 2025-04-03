using Redemption.BaseExtension;
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
            Main.tileMerge[Type][TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedSandTile>()] = true;
            Main.tileMerge[TileType<IrradiatedSandTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedHardenedSandTile>()] = true;
            Main.tileMerge[TileType<IrradiatedHardenedSandTile>()][Type] = true;
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
        }
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, DustType, 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (!fail && dist <= 4)
                player.RedemptionRad().Irradiate(.05f, 0, 2, 1, 6);
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j - 2);

            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, TileType<GrubNestTile>());
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<GrubNestTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.HasTile && !tileAbove2.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, TileType<RadioactiveSandstoneStalagmitesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<RadioactiveSandstoneStalagmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j - 1, TileType<RadioactiveSandstoneStalagmites2Tile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<RadioactiveSandstoneStalagmites2Tile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && !tileBelow2.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, TileType<RadioactiveSandstoneStalacmitesTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, TileType<RadioactiveSandstoneStalacmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(300))
            {
                WorldGen.PlaceObject(i, j + 1, TileType<RadioactiveSandstoneStalacmites2Tile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j + 1, TileType<RadioactiveSandstoneStalacmites2Tile>(), 0, 0, -1, -1);
            }
        }
    }
}


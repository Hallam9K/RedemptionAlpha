using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedJungleGrassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.GrassSpecial[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Mud;
            TileID.Sets.Conversion.JungleGrass[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            AddMapEntry(new Color(125, 115, 97));
            MinPick = 10;
            MineResist = 0.1f;
            DustType = DustID.Ash;
            RegisterItemDrop(ItemType<IrradiatedMud>(), 0);
        }
        public override bool CanExplode(int i, int j)
        {
            WorldGen.KillTile(i, j, false, false, true);
            return true;
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
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).TileType = (ushort)TileType<IrradiatedMudTile>();
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tile = Framing.GetTileSafely(i, j);
            if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                if (tile.Slope != SlopeType.SlopeUpLeft && tile.Slope != SlopeType.SlopeUpRight)
                {
                    tileBelow.TileType = (ushort)TileType<PurityWastelandVine>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }

            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(15) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                int rand = Main.rand.Next(14);
                WorldGen.PlaceObject(i, j - 1, TileType<PurityWastelandFoliage>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<PurityWastelandFoliage>(), rand, 0, -1, -1);
            }
            if (Main.rand.NextBool(4))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), TileType<IrradiatedMudTile>(), Type, false, tile.BlockColorAndCoating());

            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(100))
            {
                int rand = Main.rand.Next(4);
                WorldGen.PlaceObject(i, j - 1, TileType<XenomiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<XenomiteCrystalTile>(), rand, 0, -1, -1);
            }
            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacement(-1, i, j - 1, TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }
    }
}
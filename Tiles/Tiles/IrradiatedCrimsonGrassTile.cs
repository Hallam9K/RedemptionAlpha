using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedCrimsonGrassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileSolid[Type] = true;

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<AncientDirtTile>();
            TileID.Sets.ForcedDirtMerging[Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.Conversion.MergesWithDirtInASpecialWay[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            AddMapEntry(new Color(106, 93, 102));
            MinPick = 10;
            MineResist = 0.1f;
            DustType = DustID.Ash;
            RegisterItemDrop(ModContent.ItemType<IrradiatedDirt>(), 0);
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
                Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<IrradiatedDirtTile>();
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tile = Framing.GetTileSafely(i, j);

            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(15) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                int rand = Main.rand.Next(22);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<CrimsonWastelandFoliage>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<CrimsonWastelandFoliage>(), rand, 0, -1, -1);
            }
            if (Main.rand.NextBool(4))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<IrradiatedDirtTile>(), Type, false, tile.BlockColorAndCoating());

            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(100))
            {
                int rand = Main.rand.Next(4);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), rand, 0, -1, -1);
            }
            if (NPC.downedMechBossAny && !tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.04f;
            b = 0.0f;
        }
    }
}


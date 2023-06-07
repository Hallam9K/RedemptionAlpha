using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Dusts;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Critters;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Tiles;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalTile : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            Tile topperTile = Framing.GetTileSafely(i, --j);

            if (closer && (Main.LocalPlayer.InModBiome<WastelandPurityBiome>() || Main.LocalPlayer.InModBiome<LabBiome>()) &&
                topperTile.LiquidAmount > 0 && topperTile.LiquidType == LiquidID.Water)
            {
                for (; j > 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidType == LiquidID.Water; --j);

                if (Main.rand.NextBool(200))
                    Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<XenoWaterDust>(), Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, -2f));
            }
        }
        public override bool CanDrop(int i, int j, int type)
        {
            if (type == ModContent.TileType<IrradiatedDirtTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<IrradiatedDirtTile>()])
                return false;
            if (type == ModContent.TileType<AncientDirtTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<AncientDirtTile>()])
                return false;
            if (type == ModContent.TileType<ShadestoneBrickTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneBrickTile>()])
                return false;
            if (type == ModContent.TileType<ShadestoneTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneTile>()])
                return false;
            return base.CanDrop(i, j, type);
        }
        public override void Drop(int i, int j, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (type == TileID.Trees && Main.tile[i, j + 1].TileType == TileID.Grass)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<TreeBugFall>(), 0, 0);
                }
                if (type == TileID.PalmTree && Main.tile[i, j + 1].TileType == TileID.Sand)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<CoastScarabFall>(), 0, 0);
                }
            }
            if ((type == TileID.LeafBlock || type == TileID.LivingMahoganyLeaves) && Main.rand.NextBool(4))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<LivingTwig>());
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (type == TileID.Grass && !Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.tile[i, j].HasTile && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].WallType == 0)
                {
                    if (Main.rand.NextBool(300))
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<NightshadeTile>(), true);
                }
            }
            if (type == TileID.Grass && Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 1).HasTile && Main.tile[i, j].HasTile && Main.tile[i + 1, j].HasTile && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].WallType == 0)
                {
                    if (Main.rand.NextBool(6000))
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<AnglonicMysticBlossomTile>(), true);
                }
            }
            if (type == ModContent.TileType<IrradiatedCorruptGrassTile>() || type == ModContent.TileType<IrradiatedCrimsonGrassTile>() || type == ModContent.TileType<IrradiatedGrassTile>())
            {
                if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.tile[i, j].HasTile && Main.tile[i, j - 1].LiquidAmount == 0)
                {
                    if (Main.rand.NextBool(300))
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<RadRootTile>(), true);
                }
            }
            if (RedeGen.cryoCrystalSpawn && TileID.Sets.Conversion.Ice[type])
            {
                bool tileUp = !Framing.GetTileSafely(i, j - 1).HasTile;
                bool tileDown = !Framing.GetTileSafely(i, j + 1).HasTile;
                bool tileLeft = !Framing.GetTileSafely(i - 1, j).HasTile;
                bool tileRight = !Framing.GetTileSafely(i + 1, j).HasTile;
                if (Main.rand.NextBool(1200) && j > (int)(Main.maxTilesY * .25f))
                {
                    if (tileUp)
                    {
                        WorldGen.PlaceObject(i, j - 1, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileDown)
                    {
                        WorldGen.PlaceObject(i, j + 1, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileLeft)
                    {
                        WorldGen.PlaceObject(i - 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i - 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileRight)
                    {
                        WorldGen.PlaceObject(i + 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i + 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                }
            }
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.CanExplode(i, j, type);
        }

        public override bool Slope(int i, int j, int type)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.Slope(i, j, type);
        }
    }
    public static class RedeTileHelper
    {
        public static bool[] CannotMineTileBelow = TileID.Sets.Factory.CreateBoolSet();
        public static bool[] CannotMineTileAbove = TileID.Sets.Factory.CreateBoolSet();
        public static bool[] CannotTeleportInFront = WallID.Sets.Factory.CreateBoolSet();
    }
}

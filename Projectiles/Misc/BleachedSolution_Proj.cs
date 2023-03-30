using System;
using Redemption.Dusts;
using Redemption.Tiles.Tiles;
using Redemption.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class BleachedSolution_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bleached Spray");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int dustType = ModContent.DustType<BleachedSolutionDust>();
            if (Projectile.owner == Main.myPlayer)
                Convert((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 2);

            if (Projectile.timeLeft > 133)
                Projectile.timeLeft = 133;

            if (Projectile.ai[0] > 7f)
            {
                float dustScale = 1f;
                if (Projectile.ai[0] == 8f)
                    dustScale = 0.2f;
                else if (Projectile.ai[0] == 9f)
                    dustScale = 0.4f;
                else if (Projectile.ai[0] == 10f)
                    dustScale = 0.6f;
                else if (Projectile.ai[0] == 11f)
                    dustScale = 0.8f;
                Projectile.ai[0] += 1f;
                for (int i = 0; i < 1; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
                    Dust dust = Main.dust[dustIndex];
                    dust.noGravity = true;
                    dust.scale *= 1.75f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                    dust.scale *= dustScale;
                }
            }
            else
                Projectile.ai[0] += 1f;

            Projectile.rotation += 0.3f * Projectile.direction;
        }

        public static void Convert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
                    {
                        int type = Main.tile[k, l].TileType;
                        int wall = Main.tile[k, l].WallType;
                        if (type == TileID.LeafBlock)
                            WorldGen.KillTile(k, l, false, false, true);
                        if (wall == WallID.LivingLeaf)
                            WorldGen.KillWall(k, l, false);

                        #region Conversion
                        if ((TileID.Sets.Conversion.Stone[type] && type != TileID.Ebonstone && type != TileID.Crimstone &&
                            type != ModContent.TileType<IrradiatedCrimstoneTile>() && type != ModContent.TileType<IrradiatedEbonstoneTile>()) ||
                            TileID.Sets.Conversion.Moss[type])
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedStoneTile>());
                        else if (type == TileID.Ebonstone)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedEbonstoneTile>());
                        else if (type == TileID.Crimstone)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedCrimstoneTile>());
                        else if (TileID.Sets.Conversion.Grass[type] && type != TileID.CorruptGrass && type != TileID.CrimsonGrass &&
                            type != ModContent.TileType<IrradiatedCorruptGrassTile>() && type != ModContent.TileType<IrradiatedCrimsonGrassTile>())
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedGrassTile>());
                        else if (type == TileID.CorruptGrass)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedCorruptGrassTile>());
                        else if (type == TileID.CrimsonGrass)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedCrimsonGrassTile>());
                        else if (type == TileID.Dirt)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedDirtTile>());
                        else if (TileID.Sets.Conversion.Ice[type] && type != TileID.SnowBlock && type != ModContent.TileType<IrradiatedSnowTile>())
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedIceTile>());
                        else if (type == TileID.SnowBlock)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedSnowTile>());
                        else if (TileID.Sets.Conversion.Sand[type])
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedSandTile>());
                        else if (TileID.Sets.Conversion.HardenedSand[type])
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedHardenedSandTile>());
                        else if (TileID.Sets.Conversion.Sandstone[type])
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedSandstoneTile>());
                        else if (type == TileID.LivingWood)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<IrradiatedLivingWoodTile>());
                        else if (type == TileID.WoodBlock)
                            ConversionHandler.ConvertTile(k, l, (ushort)ModContent.TileType<PetrifiedWoodTile>());         

                        if (WallID.Sets.Conversion.Stone[wall] && wall != WallID.EbonstoneUnsafe && wall != WallID.CrimstoneUnsafe &&
                            wall != ModContent.WallType<IrradiatedEbonstoneWallTile>() && wall != ModContent.WallType<IrradiatedCrimstoneWallTile>())
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedStoneWallTile>());
                        else if (wall == WallID.EbonstoneUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedEbonstoneWallTile>());
                        else if (wall == WallID.CrimstoneUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedCrimstoneWallTile>());
                        else if (WallID.Sets.Conversion.HardenedSand[wall])
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedHardenedSandWallTile>());
                        else if (WallID.Sets.Conversion.Sandstone[wall])
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedSandstoneWallTile>());
                        else if (wall == WallID.IceUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedIceWallTile>());
                        else if (wall == WallID.SnowWallUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedSnowWallTile>());
                        else if (wall == WallID.LivingWood)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedLivingWoodWallTile>());
                        else if (wall == WallID.DirtUnsafe || wall == WallID.DirtUnsafe1 || wall == WallID.GrassUnsafe || wall == WallID.FlowerUnsafe ||
                            wall == WallID.CorruptGrassUnsafe || wall == WallID.CrimsonGrassUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedDirtWallTile>());
                        else if (wall == WallID.MudUnsafe)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<IrradiatedMudWallTile>());
                        else if (wall == WallID.Wood)
                            ConversionHandler.ConvertWall(k, l, (ushort)ModContent.WallType<PetrifiedWoodWallTile>());
                        #endregion
                    }
                }
            }
        }
    }
}

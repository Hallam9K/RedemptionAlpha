using Redemption.Dusts;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Tiles;
using Redemption.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class BleachedSolution_Proj : ModProjectile
    {
        public static int ConversionType;

        public ref float Progress => ref Projectile.ai[0];
        // Solutions shot by the terraformer get an increase in conversion area size, indicated by the second AI parameter being set to 1
        public bool ShotFromTerraformer => Projectile.ai[1] == 1f;

        public override void SetStaticDefaults()
        {
            // Cache the conversion type here instead of repeately fetching it every frame
            ConversionType = GetInstance<WastelandSolutionConversion>().Type;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the projectile properties to match other sprays.
            Projectile.DefaultToSpray();
            Projectile.aiStyle = 0; // Here we set aiStyle back to 0 because we have custom AI code
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Projectile.timeLeft > 133)
                Projectile.timeLeft = 133;

            if (Projectile.owner == Main.myPlayer)
            {
                int size = ShotFromTerraformer ? 3 : 2;
                Point tileCenter = Projectile.Center.ToTileCoordinates();
                WorldGen.Convert(tileCenter.X, tileCenter.Y, ConversionType, size);
            }

            int spawnDustTreshold = 7;
            if (ShotFromTerraformer)
                spawnDustTreshold = 3;

            if (Progress > (float)spawnDustTreshold)
            {
                float dustScale = 1f;
                int dustType = DustType<BleachedSolutionDust>();

                if (Progress == spawnDustTreshold + 1)
                    dustScale = 0.2f;
                else if (Progress == spawnDustTreshold + 2)
                    dustScale = 0.4f;
                else if (Progress == spawnDustTreshold + 3)
                    dustScale = 0.6f;
                else if (Progress == spawnDustTreshold + 4)
                    dustScale = 0.8f;

                int dustArea = 0;
                if (ShotFromTerraformer)
                {
                    dustScale *= 1.2f;
                    dustArea = (int)(12f * dustScale);
                }

                Dust sprayDust = Dust.NewDustDirect(new Vector2(Projectile.position.X - dustArea, Projectile.position.Y - dustArea), Projectile.width + dustArea * 2, Projectile.height + dustArea * 2, dustType, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100);
                sprayDust.noGravity = true;
                sprayDust.scale *= 1.75f * dustScale;
            }

            Progress++;
            Projectile.rotation += 0.3f * Projectile.direction;
        }
    }
    public class WastelandSolutionConversion : ModBiomeConversion
    {
        public static int GrassType;
        public static int CorruptGrassType;
        public static int CrimsonGrassType;
        public static int PlantsType;
        public static int CorruptPlantsType;
        public static int CrimsonPlantsType;
        public static int JungleGrassType;
        public static int DirtType;
        public static int StoneType;
        public static int CorruptStoneType;
        public static int CrimsonStoneType;
        public static int SandType;
        public static int SnowType;
        public static int IceType;
        public static int HardenedSandType;
        public static int SandstoneType;
        public static int LivingWoodType;
        public static int WoodType;
        public static int MudType;

        public static int DirtWallType;
        public static int StoneWallType;
        public static int CorruptStoneWallType;
        public static int CrimsonStoneWallType;
        public static int HardenedSandWallType;
        public static int SnowWallType;
        public static int IceWallType;
        public static int SandstoneWallType;
        public static int LivingWoodWallType;
        public static int MudWallType;
        public static int WoodWallType;

        public override void PostSetupContent()
        {
            GrassType = TileType<IrradiatedGrassTile>();
            CorruptGrassType = TileType<IrradiatedCorruptGrassTile>();
            CrimsonGrassType = TileType<IrradiatedCrimsonGrassTile>();
            PlantsType = TileType<PurityWastelandFoliage>();
            CorruptPlantsType = TileType<CorruptionWastelandFoliage>();
            CrimsonPlantsType = TileType<CrimsonWastelandFoliage>();
            JungleGrassType = TileType<IrradiatedJungleGrassTile>();
            DirtType = TileType<IrradiatedDirtTile>();
            StoneType = TileType<IrradiatedStoneTile>();
            CorruptStoneType = TileType<IrradiatedEbonstoneTile>();
            CrimsonStoneType = TileType<IrradiatedCrimstoneTile>();
            SandType = TileType<IrradiatedSandTile>();
            SnowType = TileType<IrradiatedSnowTile>();
            IceType = TileType<IrradiatedIceTile>();
            HardenedSandType = TileType<IrradiatedHardenedSandTile>();
            SandstoneType = TileType<IrradiatedSandstoneTile>();
            LivingWoodType = TileType<IrradiatedLivingWoodTile>();
            WoodType = TileType<PetrifiedWoodTile>();
            MudType = TileType<IrradiatedMudTile>();

            DirtWallType = WallType<IrradiatedDirtWallTileSafe>();
            StoneWallType = WallType<IrradiatedStoneWallTile>();
            CorruptStoneWallType = WallType<IrradiatedEbonstoneWallTileSafe>();
            CrimsonStoneWallType = WallType<IrradiatedCrimstoneWallTileSafe>();
            HardenedSandWallType = WallType<IrradiatedHardenedSandWallTileSafe>();
            SnowWallType = WallType<IrradiatedSnowWallTileSafe>();
            IceWallType = WallType<IrradiatedIceWallTileSafe>();
            SandstoneWallType = WallType<IrradiatedSandstoneWallTileSafe>();
            LivingWoodWallType = WallType<IrradiatedLivingWoodWallTile>();
            MudWallType = WallType<IrradiatedMudWallTileSafe>();
            WoodWallType = WallType<PetrifiedWoodWallTile>();

            for (int i = 0; i < TileLoader.TileCount; i++)
            {

                // Evil grass has the same issue as evil stone where it converts, then immediately converts again into normal Irradiated Grass
                if ((TileID.Sets.Conversion.Grass[i] ||
                    TileID.Sets.Conversion.GolfGrass[i]) && i is not TileID.CorruptGrass and not TileID.CrimsonGrass && i != CorruptGrassType && i != CrimsonGrassType)
                    TileLoader.RegisterConversion(i, Type, ConvertGrass);

                // Jungle grass is all good
                if (TileID.Sets.Conversion.JungleGrass[i] ||
                    TileID.Sets.Conversion.MushroomGrass[i])
                    TileLoader.RegisterConversion(i, Type, ConvertJungleGrass);

                if (TileID.Sets.Conversion.Dirt[i])
                    TileLoader.RegisterConversion(i, Type, ConvertDirt);

                // Evil stone converts correctly then immediately seems to change to normal stone, which then converts into Irradiated Stone
                if ((TileID.Sets.Conversion.Stone[i] || TileID.Sets.Conversion.Moss[i]) && i is not TileID.Ebonstone and not TileID.Crimstone && i != CorruptStoneType && i != CrimsonStoneType)
                    TileLoader.RegisterConversion(i, Type, ConvertStone);

                if (TileID.Sets.Conversion.Sand[i])
                    TileLoader.RegisterConversion(i, Type, ConvertSand);
                if (TileID.Sets.Conversion.Snow[i])
                    TileLoader.RegisterConversion(i, Type, ConvertSnow);
                if (TileID.Sets.Conversion.Ice[i])
                    TileLoader.RegisterConversion(i, Type, ConvertIce);
                if (TileID.Sets.Conversion.HardenedSand[i])
                    TileLoader.RegisterConversion(i, Type, ConvertHardenedSand);
                if (TileID.Sets.Conversion.Sandstone[i])
                    TileLoader.RegisterConversion(i, Type, ConvertSandstone);
            }

            TileLoader.RegisterConversion(TileID.Ebonstone, Type, ConvertCorruptStone);
            TileLoader.RegisterConversion(TileID.Crimstone, Type, ConvertCrimsonStone);
            TileLoader.RegisterConversion(TileID.CorruptGrass, Type, ConvertCorruptGrass);
            TileLoader.RegisterConversion(TileID.CrimsonGrass, Type, ConvertCrimsonGrass);

            TileLoader.RegisterConversion(TileID.LeafBlock, Type, ConvertLeaf);
            TileLoader.RegisterConversion(TileID.LivingWood, Type, ConvertLivingWood);
            TileLoader.RegisterConversion(TileID.WoodBlock, Type, ConvertWood);
            TileLoader.RegisterConversion(TileID.Mud, Type, ConvertMud);

            // Walls

            for (int i = 0; i < WallLoader.WallCount; i++)
            {
                if (WallID.Sets.Conversion.Dirt[i])
                    WallLoader.RegisterConversion(i, Type, ConvertDirtWall);
                if (WallID.Sets.Conversion.Grass[i])
                    WallLoader.RegisterConversion(i, Type, ConvertLeafWall);

                // Same issue applies with evil stone walls
                if (WallID.Sets.Conversion.Stone[i] || WallID.Sets.Conversion.NewWall1[i] || WallID.Sets.Conversion.NewWall2[i] || WallID.Sets.Conversion.NewWall3[i] || WallID.Sets.Conversion.NewWall4[i] && i is not WallID.EbonstoneUnsafe and not WallID.CrimstoneUnsafe and not WallID.EbonstoneEcho and not WallID.CrimstoneEcho && i != CorruptStoneWallType && i != CrimsonStoneWallType)
                    WallLoader.RegisterConversion(i, Type, ConvertStoneWall);

                if (WallID.Sets.Conversion.HardenedSand[i])
                    WallLoader.RegisterConversion(i, Type, ConvertHardenedSandWall);
                if (WallID.Sets.Conversion.Snow[i])
                    WallLoader.RegisterConversion(i, Type, ConvertSnowWall);
                if (WallID.Sets.Conversion.Ice[i])
                    WallLoader.RegisterConversion(i, Type, ConvertIceWall);
                if (WallID.Sets.Conversion.Sandstone[i])
                    WallLoader.RegisterConversion(i, Type, ConvertSandstoneWall);
            }

            WallLoader.RegisterConversion(WallID.EbonstoneUnsafe, Type, ConvertCorruptStoneWall);
            WallLoader.RegisterConversion(WallID.CrimstoneUnsafe, Type, ConvertCrimsonStoneWall);
            WallLoader.RegisterConversion(WallID.EbonstoneEcho, Type, ConvertCorruptStoneWall);
            WallLoader.RegisterConversion(WallID.CrimstoneEcho, Type, ConvertCrimsonStoneWall);

            WallLoader.RegisterConversion(WallID.LivingLeaf, Type, ConvertLeafWall);
            WallLoader.RegisterConversion(WallID.LivingWood, Type, ConvertLivingWoodWall);
            WallLoader.RegisterConversion(WallID.LivingWoodUnsafe, Type, ConvertLivingWoodWall);
            WallLoader.RegisterConversion(WallID.MudUnsafe, Type, ConvertMudWall);
            WallLoader.RegisterConversion(WallID.MudWallEcho, Type, ConvertMudWall);
            WallLoader.RegisterConversion(WallID.Wood, Type, ConvertWoodWall);
        }

        public bool ConvertDirt(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, DirtType, true);
            return false;
        }
        public bool ConvertStone(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, StoneType, true);
            return false;
        }
        public bool ConvertCorruptStone(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, CorruptStoneType, true);
            return false;
        }
        public bool ConvertCrimsonStone(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, CrimsonStoneType, true);
            return false;
        }
        public bool ConvertSand(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, SandType, true);
            return false;
        }
        public bool ConvertSnow(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, SnowType, true);
            return false;
        }
        public bool ConvertIce(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, IceType, true);
            return false;
        }
        public bool ConvertHardenedSand(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, HardenedSandType, true);
            return false;
        }
        public bool ConvertSandstone(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, SandstoneType, true);
            return false;
        }
        public bool ConvertLeaf(int i, int j, int type, int conversionType)
        {
            WorldGen.KillTile(i, j, false, false, true);
            return false;
        }
        public bool ConvertLivingWood(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, LivingWoodType, true);
            return false;
        }
        public bool ConvertWood(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, WoodType, true);
            return false;
        }
        public bool ConvertMud(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertTile(i, j, MudType, true);
            return false;
        }

        public bool ConvertGrass(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            // To handle conversion from your modded plants back to vanilla ones, you can include checks for floor tile in ModTile.TileFrame() and then convert it to the appropriate vanilla plant from there
            if (tileTypeAbove == TileID.Plants ||
                tileTypeAbove == TileID.CorruptPlants ||
                tileTypeAbove == TileID.CrimsonPlants ||
                tileTypeAbove == TileID.HallowedPlants ||
                tileTypeAbove == TileID.Plants2 ||
                tileTypeAbove == TileID.HallowedPlants2)
            {

                Tile tileAbove = Main.tile[i, j - 1];

                if (j > 2 && (tileTypeAbove == TileID.Plants2 || tileTypeAbove == TileID.HallowedPlants2))
                {
                    Tile tileTwiceAbove = Main.tile[i, j - 2];
                    if (tileTwiceAbove.TileType == tileTypeAbove)
                    {
                        tileTwiceAbove.HasTile = false;
                        tileTwiceAbove.TileType = 0;
                    }

                    tileAbove.TileFrameY = 0;
                }

                tileAbove.TileType = (ushort)PlantsType;
                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(14) * 18);
            }

            WorldGen.ConvertTile(i, j, GrassType);
            return false;
        }
        public bool ConvertCorruptGrass(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            // To handle conversion from your modded plants back to vanilla ones, you can include checks for floor tile in ModTile.TileFrame() and then convert it to the appropriate vanilla plant from there
            if (tileTypeAbove == TileID.Plants ||
                tileTypeAbove == TileID.CorruptPlants ||
                tileTypeAbove == TileID.CrimsonPlants ||
                tileTypeAbove == TileID.HallowedPlants ||
                tileTypeAbove == TileID.Plants2 ||
                tileTypeAbove == TileID.HallowedPlants2)
            {

                Tile tileAbove = Main.tile[i, j - 1];

                if (j > 2 && (tileTypeAbove == TileID.Plants2 || tileTypeAbove == TileID.HallowedPlants2))
                {
                    Tile tileTwiceAbove = Main.tile[i, j - 2];
                    if (tileTwiceAbove.TileType == tileTypeAbove)
                    {
                        tileTwiceAbove.HasTile = false;
                        tileTwiceAbove.TileType = 0;
                    }

                    tileAbove.TileFrameY = 0;
                }

                tileAbove.TileType = (ushort)CorruptPlantsType;
                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(22) * 18);
            }

            WorldGen.ConvertTile(i, j, CorruptGrassType);
            return false;
        }
        public bool ConvertCrimsonGrass(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            // To handle conversion from your modded plants back to vanilla ones, you can include checks for floor tile in ModTile.TileFrame() and then convert it to the appropriate vanilla plant from there
            if (tileTypeAbove == TileID.Plants ||
                tileTypeAbove == TileID.CorruptPlants ||
                tileTypeAbove == TileID.CrimsonPlants ||
                tileTypeAbove == TileID.HallowedPlants ||
                tileTypeAbove == TileID.Plants2 ||
                tileTypeAbove == TileID.HallowedPlants2)
            {

                Tile tileAbove = Main.tile[i, j - 1];

                if (j > 2 && (tileTypeAbove == TileID.Plants2 || tileTypeAbove == TileID.HallowedPlants2))
                {
                    Tile tileTwiceAbove = Main.tile[i, j - 2];
                    if (tileTwiceAbove.TileType == tileTypeAbove)
                    {
                        tileTwiceAbove.HasTile = false;
                        tileTwiceAbove.TileType = 0;
                    }

                    tileAbove.TileFrameY = 0;
                }

                tileAbove.TileType = (ushort)CrimsonPlantsType;
                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(22) * 18);
            }

            WorldGen.ConvertTile(i, j, CrimsonGrassType);
            return false;
        }
        public bool ConvertJungleGrass(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            if (tileTypeAbove == TileID.JunglePlants ||
                tileTypeAbove == TileID.JunglePlants2)
            {

                Tile tileAbove = Main.tile[i, j - 1];

                if (j > 2 && (tileTypeAbove == TileID.Plants2 || tileTypeAbove == TileID.HallowedPlants2))
                {
                    Tile tileTwiceAbove = Main.tile[i, j - 2];
                    if (tileTwiceAbove.TileType == tileTypeAbove)
                    {
                        tileTwiceAbove.HasTile = false;
                        tileTwiceAbove.TileType = 0;
                    }

                    tileAbove.TileFrameY = 0;
                }

                tileAbove.TileType = (ushort)PlantsType;
                tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(14) * 18);
            }
            WorldGen.ConvertTile(i, j, (ushort)JungleGrassType);
            return false;
        }

        public bool ConvertLeafWall(int i, int j, int type, int conversionType)
        {
            WorldGen.KillWall(i, j, false);
            return false;
        }
        public bool ConvertDirtWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, DirtWallType);
            return false;
        }
        public bool ConvertStoneWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, StoneWallType);
            return false;
        }
        public bool ConvertCorruptStoneWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, CorruptStoneWallType);
            return false;
        }
        public bool ConvertCrimsonStoneWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, CrimsonStoneWallType);
            return false;
        }
        public bool ConvertHardenedSandWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, HardenedSandWallType);
            return false;
        }
        public bool ConvertSnowWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, SnowWallType);
            return false;
        }
        public bool ConvertIceWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, IceWallType);
            return false;
        }
        public bool ConvertSandstoneWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, SandstoneWallType);
            return false;
        }
        public bool ConvertLivingWoodWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, LivingWoodWallType);
            return false;
        }
        public bool ConvertMudWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, MudWallType);
            return false;
        }
        public bool ConvertWoodWall(int i, int j, int type, int conversionType)
        {
            WorldGen.ConvertWall(i, j, WoodWallType);
            return false;
        }
    }
}
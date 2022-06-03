using System.Collections.Generic;
using Redemption.NPCs.Critters;
using Redemption.Tags;
using Terraria.ID;
using Terraria.ModLoader;
using GroupProj = Redemption.Tags.ProjectileTags;
using GroupTile = Redemption.Tags.TileTags;
using GroupItem = Redemption.Tags.ItemTags;
using Redemption.NPCs.PreHM;
using Redemption.Projectiles.Hostile;
using Redemption.NPCs.Friendly;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Projectiles.Ranged;
using Redemption.Items.Weapons.PreHM.Ammo;
using Redemption.Projectiles.Magic;
using Terraria;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.Projectiles.Misc;
using Redemption.Projectiles.Melee;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Projectiles.Minions;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.Tiles.Tiles;
using Redemption.NPCs.Lab;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Wasteland;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Volt;
using Redemption.NPCs.Lab.MACE;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.Tiles.Furniture.ElderWood;
using Redemption.Tiles.Furniture.PetrifiedWood;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Bosses.Obliterator;

namespace Redemption.Globals
{
    public static class TileLists
    {
        #region Tile Lists

        public static List<int> CorruptTiles = new()
        {
            23,
            25,
            112,
            163,
            398,
            400
        };

        public static List<int> CrimsonTiles = new()
        {
            199,
            203,
            234,
            200,
            399,
            401,
            205
        };

        public static List<int> EvilTiles = new()
        {
            23,
            25,
            112,
            163,
            398,
            400,
            199,
            203,
            234,
            200,
            399,
            401,
            205
        };

        public static List<int> HallowTiles = new()
        {
            109,
            117,
            116,
            164,
            402,
            403,
            115
        };

        public static List<int> ForestTiles = new()
        {
            2
        };

        public static List<int> GloomTiles = new()
        {
            59,
            70,
            194,
        };

        public static List<int> GlowingMushTiles = new()
        {
            59,
            70,
            190
        };

        public static List<int> CloudTiles = new()
        {
            189,
            196,
            460
        };

        public static List<int> HellTiles = new()
        {
            57,
            198,
            58,
            76,
            75
        };

        public static List<int> SnowTiles = new()
        {
            161,
            206,
            164,
            200,
            163,
            162,
            147,
            148
        };

        public static List<int> DesertTiles = new()
        {
            53,
            396,
            397,
            403,
            402,
            401,
            399,
            400,
            398
        };

        public static List<int> JungleTiles = new()
        {
            59,
            120,
            60,
        };

        public static List<int> DirtTiles = new()
        {
            TileID.Dirt,
            59,
            40
        };

        public static List<int> OreTiles = new()
        {
            408,
            7,
            166,
            6,
            167,
            9,
            168,
            8,
            169,
            22,
            204,
            37,
            58,
            107,
            221,
            108,
            222,
            111,
            223,
            211,
            //ModContent.TileType<DragonLeadOreTile>(),
            //ModContent.TileType<KaniteOreTile>(),
            //ModContent.TileType<SapphironOreTile>(),
            //ModContent.TileType<ScarlionOreTile>(),
            //ModContent.TileType<StarliteOreTile>(),
            //ModContent.TileType<XenomiteOreBlock>()
        };

        public static List<int> HotTiles = new()
        {
            53,
            396,
            397,
            403,
            402,
            401,
            399,
            400,
            398,
            57,
            198,
            58,
            76,
            75
        };

        public static List<int> NatureTiles = new()
        {
            2,
            59,
            120,
            60
        };

        public static List<int> BlacklistTiles = new()
        {
            TileID.BlueDungeonBrick,
            TileID.GreenDungeonBrick,
            TileID.PinkDungeonBrick,
            TileID.LihzahrdBrick,
            TileID.BeeHive,
            TileID.Granite,
            TileID.Marble,
            ModContent.TileType<GathicStoneBrickTile>(),
            ModContent.TileType<GathicGladestoneBrickTile>(),
            ModContent.TileType<AncientHallBrickTile>(),
            ModContent.TileType<SlayerShipPanelTile>(),
            ModContent.TileType<LabPlatingTileUnsafe>()
        };

        public static List<int> ModdedChests = new();

        public static List<int> ModdedDoors = new()
        {
            ModContent.TileType<ElderWoodDoorClosed>(),
            ModContent.TileType<LabDoorClosed>(),
            ModContent.TileType<PetrifiedWoodDoorClosed>()
        };
        #endregion
    }

    public static class NPCLists
    {
        #region NPC Lists

        public static List<int> HasLostSoul = new() { ModContent.NPCType<LostSoulNPC>(), ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<VagrantSpirit>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<RaveyardSkeleton>(), 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635 };

        #region Skeleton
        public static List<int> Skeleton = new() { 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635, NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle, NPCID.BoneSerpentHead, NPCID.BoneSerpentBody, NPCID.BoneSerpentTail, NPCID.DarkCaster, NPCID.CursedSkull, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DiabolistRed, NPCID.DiabolistWhite, NPCID.GiantCursedSkull, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.DungeonGuardian, NPCID.SkeletronHead, NPCID.SkeletronHand, NPCID.PirateGhost, ModContent.NPCType<BoneSpider>(), ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<RaveyardSkeleton>(), ModContent.NPCType<BobTheBlob>() };


        public static List<int> SkeletonHumanoid = new() { 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635, NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle, NPCID.DarkCaster, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DiabolistRed, NPCID.DiabolistWhite, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.PirateGhost, ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<RaveyardSkeleton>(), ModContent.NPCType<BobTheBlob>() };
        #endregion

        #region Undead

        public static List<int> Undead = new() { 3, 132, 186, 187, 188, 189, 200, 223, 161, 254, 255, 52, 53, 536, 319, 320, 321, 332, 436, 431, 432, 433, 434, 435, 331, 430, 590, NPCID.TorchZombie, NPCID.ArmedTorchZombie, NPCID.MaggotZombie, NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.BloodZombie, NPCID.ZombieMerman, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.Eyezor, NPCID.Frankenstein, NPCID.Vampire, NPCID.VampireBat, NPCID.HeadlessHorseman, NPCID.ZombieElf, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, ModContent.NPCType<RaggedZombie>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<Keeper>(), ModContent.NPCType<SkullDigger>(), ModContent.NPCType<Fallen>(), ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<HazmatZombie>() };

        #endregion

        #region Spirit

        public static List<int> Spirit = new() { NPCID.EnchantedSword, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.DesertDjinn, NPCID.DungeonSpirit, NPCID.FloatyGross, NPCID.Ghost, NPCID.PossessedArmor, NPCID.Wraith, NPCID.Reaper, NPCID.Poltergeist, NPCID.PirateGhost, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<VagrantSpirit>(), ModContent.NPCType<KeeperSpirit>(), ModContent.NPCType<ErhanSpirit>(), ModContent.NPCType<LostSoulNPC>(), ModContent.NPCType<NuclearShadow>(), ModContent.NPCType<WraithSlayer_Samurai>() };

        #endregion

        #region Plantlike

        public static List<int> Plantlike = new() { NPCID.FungiBulb, NPCID.AnomuraFungus, NPCID.MushiLadybug, NPCID.ManEater, NPCID.Snatcher, NPCID.AngryTrapper, NPCID.FungoFish, NPCID.GiantFungiBulb, NPCID.HoppinJack, NPCID.Dandelion, NPCID.Plantera, NPCID.MourningWood, NPCID.Splinterling, NPCID.Pumpking, NPCID.Everscream, NPCID.PlanterasTentacle, ModContent.NPCType<LivingBloom>(), ModContent.NPCType<DevilsTongue>(), ModContent.NPCType<Thorn>(), ModContent.NPCType<TreebarkDryad>(), ModContent.NPCType<MutatedLivingBloom>() };

        #endregion

        #region Demon
        public static List<int> Demon = new() { NPCID.Demon, NPCID.VoodooDemon, NPCID.FireImp, NPCID.RedDevil };
        #endregion

        #region Cold
        public static List<int> Cold = new() { NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.IceBat, NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.SnowFlinx, NPCID.IceElemental, NPCID.IceMimic, NPCID.IceTortoise, NPCID.IcyMerman, NPCID.MisterStabby, NPCID.Wolf, NPCID.IceGolem, NPCID.SnowBalla, NPCID.SnowmanGangsta, NPCID.Flocko, NPCID.Yeti, NPCID.IceQueen, NPCID.Deerclops, NPCID.DeerclopsLeg, ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>() };
        #endregion

        #region Dragonlike
        public static List<int> Dragonlike = new() { NPCID.DD2Betsy, NPCID.DD2WyvernT1, NPCID.DD2WyvernT2, NPCID.DD2WyvernT3, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.DukeFishron, NPCID.WyvernHead, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernLegs, NPCID.WyvernTail, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail };
        #endregion

        #region Inorganic
        public static List<int> Inorganic = new() { NPCID.GraniteFlyer, NPCID.GraniteGolem, NPCID.MeteorHead, NPCID.Mimic, NPCID.BigMimicCorruption, NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle, NPCID.IceMimic, NPCID.PresentMimic, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.EnchantedSword, NPCID.IceElemental, NPCID.MartianProbe, NPCID.PossessedArmor, NPCID.Pixie, NPCID.Paladin, NPCID.RockGolem, NPCID.ChatteringTeethBomb, NPCID.AngryNimbus, NPCID.IceGolem, NPCID.Tumbleweed, NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.SnowBalla, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Flocko, NPCID.GingerbreadMan, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.SolarCorite, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.PirateShipCannon, NPCID.IceQueen, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<EaglecrestGolem>(), ModContent.NPCType<EaglecrestGolem_Sleep>(), ModContent.NPCType<EaglecrestRockPile>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>() };
        #endregion

        #region Robotic
        public static List<int> Robotic = new() { NPCID.MartianProbe, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>(), ModContent.NPCType<OO>() };
        #endregion

        #region Infected
        public static List<int> Infected = new() { ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<OozeBlob>(), ModContent.NPCType<SeedGrowth>(), ModContent.NPCType<SoI>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<RadioactiveSlime>(), ModContent.NPCType<NuclearSlime>(), ModContent.NPCType<IrradiatedBehemoth>(), ModContent.NPCType<Blisterface>(), ModContent.NPCType<BlisteredFish>(), ModContent.NPCType<BlisteredFish2>(), ModContent.NPCType<SickenedDemonEye>(), ModContent.NPCType<SickenedBunny>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>(), ModContent.NPCType<PZ>(), ModContent.NPCType<PZ_Kari>() };
        #endregion

        #region Armed
        public static List<int> Armed = new() { NPCID.RedDevil, NPCID.Paladin, NPCID.GoblinThief, NPCID.DD2GoblinT1, NPCID.DD2GoblinT2, NPCID.DD2GoblinT3, NPCID.MisterStabby, NPCID.PirateCorsair, NPCID.PirateGhost, NPCID.Butcher, NPCID.Psycho, NPCID.Reaper, NPCID.SolarDrakomireRider, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.Pumpking, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesSword, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSword, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<WraithSlayer_Samurai>() };
        #endregion

        public static List<int> IsSlime = new() { NPCID.GreenSlime, NPCID.BlueSlime, NPCID.RedSlime, NPCID.PurpleSlime, NPCID.YellowSlime, NPCID.BlackSlime, NPCID.IceSlime, NPCID.SandSlime, NPCID.JungleSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, NPCID.MotherSlime, NPCID.BabySlime, NPCID.LavaSlime, NPCID.DungeonSlime, NPCID.Pinky, NPCID.GoldenSlime, NPCID.KingSlime, NPCID.SlimeSpiked, NPCID.UmbrellaSlime, NPCID.SlimeMasked, NPCID.SlimeRibbonGreen, NPCID.SlimeRibbonRed, NPCID.SlimeRibbonWhite, NPCID.SlimeRibbonYellow, NPCID.ToxicSludge, NPCID.CorruptSlime, NPCID.Slimeling, NPCID.Slimer, NPCID.Slimer2, NPCID.Crimslime, NPCID.Gastropod, NPCID.IlluminantSlime, NPCID.RainbowSlime, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, NPCID.HoppinJack, ModContent.NPCType<Blobble>(), ModContent.NPCType<SeedGrowth>(), ModContent.NPCType<OozeBlob>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<RadioactiveSlime>(), ModContent.NPCType<NuclearSlime>(), ModContent.NPCType<IrradiatedBehemoth>() };

        public static List<int> IsBunny = new()
        {
            NPCID.Bunny,
            NPCID.GoldBunny,
            NPCID.BunnySlimed,
            NPCID.BunnyXmas,
            NPCID.PartyBunny
        };

        #endregion
    }

    public static class ProjectileLists
    {
        #region Projectile Lists

        public static List<int> IsTechnicallyMelee = new() { ProjectileID.EnchantedBoomerang, ProjectileID.Flamarang, ProjectileID.BallOHurt, ProjectileID.BlueMoon, ProjectileID.ThornChakram, ProjectileID.Sunfury, ProjectileID.DarkLance, ProjectileID.Trident, ProjectileID.Spear, ProjectileID.WoodenBoomerang, ProjectileID.TheDaoofPow, ProjectileID.CobaltChainsaw, ProjectileID.CobaltDrill, ProjectileID.MythrilChainsaw, ProjectileID.MythrilDrill, ProjectileID.AdamantiteChainsaw, ProjectileID.AdamantiteDrill, ProjectileID.MythrilHalberd, ProjectileID.AdamantiteGlaive, ProjectileID.CobaltNaginata, ProjectileID.Gungnir, ProjectileID.LightDisc, ProjectileID.Hamdrax, ProjectileID.IceBoomerang, ProjectileID.MushroomSpear, ProjectileID.TheRottedFork, ProjectileID.TheMeatball, ProjectileID.PossessedHatchet, ProjectileID.PalladiumPike, ProjectileID.PalladiumDrill, ProjectileID.PalladiumChainsaw, ProjectileID.OrichalcumHalberd, ProjectileID.OrichalcumDrill, ProjectileID.OrichalcumChainsaw, ProjectileID.TitaniumTrident, ProjectileID.TitaniumDrill, ProjectileID.TitaniumChainsaw, ProjectileID.ChlorophytePartisan, ProjectileID.ChlorophyteDrill, ProjectileID.ChlorophyteChainsaw, ProjectileID.FlowerPow, ProjectileID.ChlorophyteJackhammer, ProjectileID.GolemFist, ProjectileID.PaladinsHammerFriendly, ProjectileID.BloodyMachete, ProjectileID.FruitcakeChakram, ProjectileID.NorthPoleWeapon, ProjectileID.ObsidianSwordfish, ProjectileID.Swordfish, ProjectileID.SawtoothShark, ProjectileID.Anchor, ProjectileID.Flairon, ProjectileID.ChainKnife, ProjectileID.ChainGuillotine, ProjectileID.ButchersChainsaw, ProjectileID.Code1, ProjectileID.WoodYoyo, ProjectileID.CorruptYoyo, ProjectileID.CrimsonYoyo, ProjectileID.JungleYoyo, ProjectileID.Cascade, ProjectileID.Chik, ProjectileID.Code2, ProjectileID.Rally, ProjectileID.Yelets, ProjectileID.RedsYoyo, ProjectileID.ValkyrieYoyo, ProjectileID.Amarok, ProjectileID.HelFire, ProjectileID.Kraken, ProjectileID.TheEyeOfCthulhu, ProjectileID.BlackCounterweight, ProjectileID.BlueCounterweight, ProjectileID.GreenCounterweight, ProjectileID.PurpleCounterweight, ProjectileID.RedCounterweight, ProjectileID.YellowCounterweight, ProjectileID.FormatC, ProjectileID.Gradient, ProjectileID.Valor, ProjectileID.MechanicWrench, ProjectileID.Arkhalis, ProjectileID.Terrarian, ProjectileID.SolarWhipSword, ProjectileID.MonkStaffT1, ProjectileID.MonkStaffT2, ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.ThunderSpear, ProjectileID.Terragrim, ProjectileID.DripplerFlail, ProjectileID.GladiusStab, ProjectileID.BlandWhip, ProjectileID.RulerStab, ProjectileID.SwordWhip, ProjectileID.MaceWhip, ProjectileID.ScytheWhip, ProjectileID.RulerStab, ProjectileID.BouncingShield, ProjectileID.Shroomerang, ProjectileID.JoustingLance, ProjectileID.ShadowJoustingLance, ProjectileID.HallowJoustingLance, ProjectileID.CombatWrench, ProjectileID.CoolWhip, ProjectileID.FireWhip, ProjectileID.ThornWhip, ProjectileID.RainbowWhip, ProjectileID.FinalFractal, ProjectileID.CopperShortswordStab, ProjectileID.TinShortswordStab, ProjectileID.IronShortswordStab, ProjectileID.LeadShortswordStab, ProjectileID.SilverShortswordStab, ProjectileID.TungstenShortswordStab, ProjectileID.GoldShortswordStab, ProjectileID.PlatinumShortswordStab, ProjectileID.Mace, ProjectileID.FlamingMace, ProjectileID.BoneWhip };

        #endregion
    }

    public sealed class ItemTags
    {
        public static readonly TagData BluntSwing = ContentTags.Get<GroupItem>(nameof(BluntSwing));
        public static readonly TagData Arcane = ContentTags.Get<GroupItem>(nameof(Arcane));
        public static readonly TagData Fire = ContentTags.Get<GroupItem>(nameof(Fire));
        public static readonly TagData Water = ContentTags.Get<GroupItem>(nameof(Water));
        public static readonly TagData Ice = ContentTags.Get<GroupItem>(nameof(Ice));
        public static readonly TagData Earth = ContentTags.Get<GroupItem>(nameof(Earth));
        public static readonly TagData Wind = ContentTags.Get<GroupItem>(nameof(Wind));
        public static readonly TagData Thunder = ContentTags.Get<GroupItem>(nameof(Thunder));
        public static readonly TagData Holy = ContentTags.Get<GroupItem>(nameof(Holy));
        public static readonly TagData Shadow = ContentTags.Get<GroupItem>(nameof(Shadow));
        public static readonly TagData Nature = ContentTags.Get<GroupItem>(nameof(Nature));
        public static readonly TagData Poison = ContentTags.Get<GroupItem>(nameof(Poison));
        public static readonly TagData Blood = ContentTags.Get<GroupItem>(nameof(Blood));
        public static readonly TagData Psychic = ContentTags.Get<GroupItem>(nameof(Psychic));
        public static readonly TagData Celestial = ContentTags.Get<GroupItem>(nameof(Celestial));
        public static readonly TagData NoElement = ContentTags.Get<GroupItem>(nameof(NoElement));

        public static void SetItemTags()
        {
            #region Item Tags

            BluntSwing.SetMultiple(ItemID.BreathingReed, ItemID.ZombieArm, ItemID.PurpleClubberfish, ItemID.TaxCollectorsStickOfDoom, ItemID.SlapHand, ItemID.Keybrand, ItemID.HamBat, ItemID.BatBat, ItemID.StaffofRegrowth);

            Arcane.SetMultiple(ItemID.EnchantedSword, ItemID.SpectrePickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe, ItemID.SpectreHamaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeStardust);
            Fire.SetMultiple(ItemID.FieryGreatsword, ItemID.TheHorsemansBlade, ItemID.DD2SquireBetsySword, ItemID.MoltenPickaxe, ItemID.SolarFlarePickaxe, ItemID.MeteorHamaxe, ItemID.MoltenHamaxe, ItemID.LunarHamaxeSolar, ModContent.ItemType<DragonCleaver>(), ModContent.ItemType<Firebreak>());
            Water.SetMultiple(ItemID.Muramasa);
            Ice.SetMultiple(ItemID.IceBlade, ItemID.IceSickle, ItemID.Frostbrand, ModContent.ItemType<PureIronSword>(), ModContent.ItemType<BladeOfTheMountain>());
            Earth.SetMultiple(ItemID.Seedler, ItemID.FossilPickaxe, ItemID.Picksaw);
            Thunder.SetMultiple(ItemID.InfluxWaver, ItemID.VortexPickaxe, ItemID.LunarHamaxeVortex, ModContent.ItemType<SlayerGun>(), ModContent.ItemType<XeniumLance>());
            Holy.SetMultiple(ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.PickaxeAxe, ItemID.Pwnhammer, ModContent.ItemType<Bindeklinge>(), ModContent.ItemType<BlindJustice>());
            Shadow.SetMultiple(ItemID.LightsBane, ItemID.PurpleClubberfish, ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.DeathSickle, ItemID.NightmarePickaxe, ItemID.WarAxeoftheNight, ItemID.TheBreaker);
            Nature.SetMultiple(ItemID.CactusSword, ItemID.BladeofGrass, ItemID.Seedler, ItemID.ChlorophyteSaber, ItemID.ChristmasTreeSword, ItemID.ChlorophyteClaymore, ItemID.TerraBlade, ItemID.CactusPickaxe, ItemID.ChlorophytePickaxe, ItemID.ChlorophyteGreataxe, ItemID.Hammush, ItemID.ChlorophyteWarhammer, ModContent.ItemType<CursedGrassBlade>());
            Poison.SetMultiple(ItemID.BeeKeeper, ModContent.ItemType<CursedGrassBlade>());
            Blood.SetMultiple(ItemID.BloodButcherer, ItemID.Bladetongue, ItemID.DeathbringerPickaxe, ItemID.BloodLustCluster, ItemID.BloodHamaxe, ItemID.FleshGrinder);
            Celestial.SetMultiple(ItemID.Starfury, ItemID.PiercingStarlight, ItemID.StarWrath, ItemID.Meowmere, ItemID.SolarFlarePickaxe, ItemID.NebulaPickaxe, ItemID.VortexPickaxe, ItemID.StardustPickaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeSolar, ItemID.LunarHamaxeStardust, ItemID.LunarHamaxeVortex);
            NoElement.SetMultiple(ItemID.BlueSolution, ItemID.DarkBlueSolution, ItemID.GreenSolution, ItemID.PurpleSolution, ItemID.RedSolution, ItemID.RocketI, ItemID.RocketII, ItemID.RocketIII, ItemID.RocketIV, ModContent.ItemType<BleachedSolution>());

            #endregion
        }
    }

    public sealed class TileTags
    {
        public static readonly TagData WoodLeaf = ContentTags.Get<GroupTile>(nameof(WoodLeaf));

        public static void SetTileTags()
        {
            #region Tile Tags

            WoodLeaf.PopulateFromSets(TileID.Sets.Leaves);
            WoodLeaf.SetMultiple(TileID.WoodBlock, TileID.BorealWood, TileID.DynastyWood, TileID.LivingWood, TileID.PalmWood, TileID.SpookyWood, TileID.Ebonwood, TileID.Pearlwood, TileID.Shadewood, TileID.LivingMahogany, TileID.RichMahogany);

            #endregion
        }
    }

    public sealed class ProjectileTags
    {
        public static readonly TagData Arcane = ContentTags.Get<GroupProj>(nameof(Arcane));
        public static readonly TagData Fire = ContentTags.Get<GroupProj>(nameof(Fire));
        public static readonly TagData Water = ContentTags.Get<GroupProj>(nameof(Water));
        public static readonly TagData Ice = ContentTags.Get<GroupProj>(nameof(Ice));
        public static readonly TagData Earth = ContentTags.Get<GroupProj>(nameof(Earth));
        public static readonly TagData Wind = ContentTags.Get<GroupProj>(nameof(Wind));
        public static readonly TagData Thunder = ContentTags.Get<GroupProj>(nameof(Thunder));
        public static readonly TagData Holy = ContentTags.Get<GroupProj>(nameof(Holy));
        public static readonly TagData Shadow = ContentTags.Get<GroupProj>(nameof(Shadow));
        public static readonly TagData Nature = ContentTags.Get<GroupProj>(nameof(Nature));
        public static readonly TagData Poison = ContentTags.Get<GroupProj>(nameof(Poison));
        public static readonly TagData Blood = ContentTags.Get<GroupProj>(nameof(Blood));
        public static readonly TagData Psychic = ContentTags.Get<GroupProj>(nameof(Psychic));
        public static readonly TagData Celestial = ContentTags.Get<GroupProj>(nameof(Celestial));
        public static readonly TagData Unparryable = ContentTags.Get<GroupProj>(nameof(Unparryable));
        public static readonly TagData NoElement = ContentTags.Get<GroupProj>(nameof(NoElement));

        public static void SetProjTags()
        {
            #region Arcane

            Arcane.SetMultiple(ProjectileID.EnchantedBoomerang, ProjectileID.Starfury, ProjectileID.MagicMissile, ProjectileID.EighthNote, ProjectileID.QuarterNote, ProjectileID.TiedEighthNote, ProjectileID.RainbowRodBullet, ProjectileID.EyeLaser, ProjectileID.PinkLaser, ProjectileID.PurpleLaser, ProjectileID.MagicDagger, ProjectileID.CrystalStorm, ProjectileID.DeathLaser, ProjectileID.SwordBeam, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt, ProjectileID.RuneBlast, ProjectileID.TerraBeam, ProjectileID.LightBeam, ProjectileID.NightBeam, ProjectileID.EnchantedBeam, ProjectileID.FrostBeam, ProjectileID.EyeBeam, ProjectileID.Skull, ProjectileID.DeathSickle, ProjectileID.LostSoulFriendly, ProjectileID.LostSoulHostile, ProjectileID.Shadowflames, ProjectileID.VampireKnife, ProjectileID.SpectreWrath, ProjectileID.PulseBolt, ProjectileID.MiniRetinaLaser, ProjectileID.InfluxWaver, ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardShaft, ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.MedusaHeadRay, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaLaser, ProjectileID.VortexLaser, ProjectileID.ClothiersCurse, ProjectileID.MinecartMechLaser, ProjectileID.TerrarianBeam, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.StardustGuardianExplosion, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.PhantasmArrow, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.LunarFlare, ProjectileID.SkyFracture, ProjectileID.DD2DarkMageBolt, ProjectileID.BookOfSkullsSkull, ProjectileID.SparkleGuitar, ProjectileID.TitaniumStormShard, ProjectileID.StardustPunch, ProjectileID.NebulaDrill, ProjectileID.StardustDrill, ProjectileID.JestersArrow, ModContent.ProjectileType<KeeperSoulCharge>(), ModContent.ProjectileType<SoulScepterCharge>(), ModContent.ProjectileType<SoulScepterChargeS>(), ModContent.ProjectileType<GiantMask>(), ModContent.ProjectileType<SpectralScythe_Proj>(), ModContent.ProjectileType<WraithSlayer_Proj>());

            #endregion

            #region Fire

            Fire.SetMultiple(ProjectileID.FireArrow, ProjectileID.BallofFire, ProjectileID.Flamarang, ProjectileID.Flamelash, ProjectileID.Sunfury, ProjectileID.HellfireArrow, ProjectileID.FlamingArrow, ProjectileID.Flames, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.RuneBlast, ProjectileID.FrostburnArrow, ProjectileID.FlamethrowerTrap, ProjectileID.FlamesTrap, ProjectileID.Fireball, ProjectileID.HeatRay, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.FlamingWood, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.ImpFireball, ProjectileID.MolotovCocktail, ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.CultistBossFireBall, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.Hellwing, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.Spark, ProjectileID.HelFire, ProjectileID.ClothiersCurse, ProjectileID.DesertDjinnCurse, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.GeyserTrap, ProjectileID.SpiritFlame, ProjectileID.DD2FlameBurstTowerT1Shot, ProjectileID.DD2FlameBurstTowerT2Shot, ProjectileID.DD2FlameBurstTowerT3Shot, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.MonkStaffT2, ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2PhoenixBowShot, ProjectileID.DD2BetsyArrow, ProjectileID.ApprenticeStaffT3Shot, ProjectileID.FireWhipProj, ProjectileID.FlamingMace, ProjectileID.TorchGod, ProjectileID.WandOfSparkingSpark, ProjectileID.SolarFlareDrill, ProjectileID.HoundiusShootiusFireball, ProjectileID.Flare, ProjectileID.BlueFlare, ProjectileID.FlyingImp, ModContent.ProjectileType<FlintAndSteelSpark>(), ModContent.ProjectileType<LunarShot_Proj>(), ModContent.ProjectileType<MoonflareBatIllusion>(), ModContent.ProjectileType<MoonflareArrow_Proj>(), ModContent.ProjectileType<CantripEmber>(), ModContent.ProjectileType<CantripEmberS>(), ModContent.ProjectileType<DragonCleaver_Proj>(), ModContent.ProjectileType<FireSlash_Proj>(), ModContent.ProjectileType<DragonSkull_Proj>(), ModContent.ProjectileType<HeatRay>(), ModContent.ProjectileType<ScorchingRay>(), ModContent.ProjectileType<RayOfGuidance>(), ModContent.ProjectileType<MagnifyingGlassRay>(), ModContent.ProjectileType<MagmaCube>(), ModContent.ProjectileType<Firebreak_Proj>(), ModContent.ProjectileType<MACE_FireBlast>(), ModContent.ProjectileType<MACE_Miniblast>(), ModContent.ProjectileType<Hacksaw_Heat_Proj>(), ModContent.ProjectileType<DragonSkullFlames_Proj>());

            #endregion

            #region Water

            Water.SetMultiple(ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.BlueMoon, ProjectileID.HolyWater, ProjectileID.UnholyWater, ProjectileID.IcewaterSpit, ProjectileID.RainFriendly, ProjectileID.BloodRain, ProjectileID.RainNimbus, ProjectileID.WaterGun, ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.FlaironBubble, ProjectileID.SlimeGun, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.Bubble, ProjectileID.Xenopopper, ProjectileID.ToxicBubble, ProjectileID.Kraken, ProjectileID.BloodWater, ProjectileID.Ale, ProjectileID.DD2OgreSpit, ProjectileID.QueenSlimeGelAttack, ProjectileID.GelBalloon, ProjectileID.VolatileGelatinBall, ModContent.ProjectileType<WaterOrb>(), ModContent.ProjectileType<WaterOrbS>(), ModContent.ProjectileType<BucketSplash>(), ModContent.ProjectileType<Blisterface_Bubble>(), ModContent.ProjectileType<DigestiveVat_Proj>());

            #endregion

            #region Ice

            Ice.SetMultiple(ProjectileID.IceBlock, ProjectileID.IceBoomerang, ProjectileID.IceBolt, ProjectileID.FrostBoltSword, ProjectileID.FrostArrow, ProjectileID.FrostBlastHostile, ProjectileID.SnowBallFriendly, ProjectileID.FrostburnArrow, ProjectileID.IceSpike, ProjectileID.IcewaterSpit, ProjectileID.BallofFrost, ProjectileID.FrostBeam, ProjectileID.IceSickle, ProjectileID.FrostBlastFriendly, ProjectileID.Blizzard, ProjectileID.NorthPoleWeapon, ProjectileID.NorthPoleSpear, ProjectileID.NorthPoleSnowflake, ProjectileID.FrostWave, ProjectileID.FrostShard, ProjectileID.FrostBoltStaff, ProjectileID.CultistBossIceMist, ProjectileID.FrostDaggerfish, ProjectileID.Amarok, ProjectileID.CoolWhip, ProjectileID.CoolWhipProj, ProjectileID.DeerclopsIceSpike, ProjectileID.DeerclopsRangedProjectile,  ProjectileID.FlinxMinion, ModContent.ProjectileType<IceBolt>(), ModContent.ProjectileType<PureIronSword_Proj>(), ModContent.ProjectileType<BladeOfTheMountain_Slash>(), ModContent.ProjectileType<Icefall_Mist>(), ModContent.ProjectileType<Icefall_Proj>());

            #endregion

            #region Earth

            Earth.SetMultiple(ProjectileID.Boulder, ProjectileID.BoulderStaffOfEarth, ProjectileID.GolemFist, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2OgreStomp, ProjectileID.DD2OgreSmash, ProjectileID.MonkStaffT1Explosion, ProjectileID.RollingCactus, ProjectileID.RockGolemRock, ModContent.ProjectileType<AncientGladestonePillar>(), ModContent.ProjectileType<EaglecrestSling_Throw>(), ModContent.ProjectileType<EaglecrestJavelin_Proj>(), ModContent.ProjectileType<CalciteWand_Proj>(), ModContent.ProjectileType<Rockslide_Proj>(), ModContent.ProjectileType<RockslidePebble_Proj>());

            #endregion

            #region Wind

            Wind.SetMultiple(ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2ApprenticeStorm, ProjectileID.BookStaffShot, ProjectileID.WeatherPainShot, ModContent.ProjectileType<FireSlash_Proj>(), ModContent.ProjectileType<RockSlash_Proj>(), ModContent.ProjectileType<KS3_Wave>(), ModContent.ProjectileType<GreenGas_Proj>(), ModContent.ProjectileType<Icefall_Mist>());

            #endregion

            #region Thunder

            Thunder.SetMultiple(ProjectileID.RuneBlast, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.UFOLaser, ProjectileID.ScutlixLaser, ProjectileID.ScutlixLaserFriendly, ProjectileID.MartianTurretBolt, ProjectileID.BrainScramblerBolt, ProjectileID.GigaZapperSpear, ProjectileID.RayGunnerLaser, ProjectileID.LaserMachinegunLaser, ProjectileID.Electrosphere, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerDeathray, ProjectileID.SaucerLaser, ProjectileID.InfluxWaver, ProjectileID.ChargedBlasterLaser, ProjectileID.ChargedBlasterOrb, ProjectileID.PhantasmalBolt, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.DeadlySphere, ProjectileID.VortexVortexLightning, ProjectileID.VortexLightning, ProjectileID.MartianWalkerLaser, ProjectileID.VortexBeaterRocket, ProjectileID.DD2LightningBugZap, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.MonkStaffT3_AltShot, ProjectileID.ThunderSpear, ProjectileID.ThunderStaffShot, ProjectileID.ThunderSpearShot, ProjectileID.ZapinatorLaser, ProjectileID.VortexDrill, ModContent.ProjectileType<KS3_BeamCell>(), ModContent.ProjectileType<KS3_MagnetBeam>(), ModContent.ProjectileType<Electronade_TeslaField>(), ModContent.ProjectileType<Electronade_Proj>(), ModContent.ProjectileType<EaglecrestJavelin_Thunder>(), ModContent.ProjectileType<Volt_OrbProj>(), ModContent.ProjectileType<TeslaBeam>(), ModContent.ProjectileType<TeslaZapBeam>(), ModContent.ProjectileType<BigElectronade_TeslaField>(), ModContent.ProjectileType<GravityHammer_Proj>(), ModContent.ProjectileType<LightningRod_Proj>(), ModContent.ProjectileType<XeniumLance_Proj>(), ModContent.ProjectileType<KS3_EnergyBolt>(), ModContent.ProjectileType<GlobalDischarge_Sphere>(), ModContent.ProjectileType<OO_StunBeam>(), ModContent.ProjectileType<OmegaPlasmaBall>());

            #endregion

            #region Holy

            Holy.SetMultiple(ProjectileID.TheDaoofPow, ProjectileID.HolyWater, ProjectileID.HolyArrow, ProjectileID.HallowStar, ProjectileID.LightBeam, ProjectileID.Hamdrax, ProjectileID.PaladinsHammerHostile, ProjectileID.PaladinsHammerFriendly, ProjectileID.SkyFracture, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.BatOfLight, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.HallowJoustingLance, ProjectileID.RainbowWhip, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ModContent.ProjectileType<CorpseWalkerBolt>(), ModContent.ProjectileType<CorpseWalkerSkull>(), ModContent.ProjectileType<CorpseWalkerSkull_Proj>(), ModContent.ProjectileType<Sunshard>(), ModContent.ProjectileType<SunshardRay>(), ModContent.ProjectileType<Lightmass>(), ModContent.ProjectileType<ScorchingRay>(), ModContent.ProjectileType<RayOfGuidance>(), ModContent.ProjectileType<HolySpear_Proj>(), ModContent.ProjectileType<HolyBible_Proj>(), ModContent.ProjectileType<HolyBible_Ray>(), ModContent.ProjectileType<MagnifyingGlassRay>(), ModContent.ProjectileType<HallowedHandGrenade_Proj>(), ModContent.ProjectileType<CrystalGlaive_Proj>(), ModContent.ProjectileType<BlindJustice_Proj>(), ModContent.ProjectileType<HolyPhalanx_Proj>(), ModContent.ProjectileType<HolyPhalanx_Proj2>(), ModContent.ProjectileType<Bible_SeedSpear>());

            #endregion

            #region Shadow

            Shadow.SetMultiple(ProjectileID.UnholyArrow, ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.BallOHurt, ProjectileID.DemonSickle, ProjectileID.DemonScythe, ProjectileID.DarkLance, ProjectileID.TheDaoofPow, ProjectileID.UnholyWater, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.NightBeam, ProjectileID.DeathSickle, ProjectileID.ShadowBeamHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.Shadowflames, ProjectileID.EatersBite, ProjectileID.TinyEater, ProjectileID.CultistBossFireBallClone, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.CorruptYoyo, ProjectileID.ClothiersCurse, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.SpiritFlame, ProjectileID.BlackBolt, ProjectileID.DD2DrakinShot, ProjectileID.DD2DarkMageBolt, ProjectileID.ShadowJoustingLance, ProjectileID.ScytheWhipProj, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ModContent.ProjectileType<KeeperDreadCoil>(), ModContent.ProjectileType<ShadowBolt>(), ModContent.ProjectileType<GiantMask>(), ModContent.ProjectileType<WraithSlayer_Proj>());

            #endregion

            #region Nature

            Nature.SetMultiple(ProjectileID.ThornChakram, ProjectileID.Seed, ProjectileID.Mushroom, ProjectileID.TerraBeam, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.JungleSpike, ProjectileID.Leaf, ProjectileID.FlowerPetal, ProjectileID.CrystalLeafShot, ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.FlowerPow, ProjectileID.FlowerPowPetal, ProjectileID.SeedPlantera, ProjectileID.PoisonSeedPlantera, ProjectileID.ThornBall, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.PineNeedleFriendly, ProjectileID.PineNeedleHostile, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn, ProjectileID.JungleYoyo, ProjectileID.SporeTrap, ProjectileID.SporeTrap2, ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.TruffleSpore, ProjectileID.Terrarian, ProjectileID.TerrarianBeam, ProjectileID.Terragrim, ProjectileID.DandelionSeed, ProjectileID.Shroomerang, ProjectileID.ThornWhip,ProjectileID.BabyBird, ModContent.ProjectileType<LivingBloomRoot>(), ModContent.ProjectileType<LunarShot_Proj>(), ModContent.ProjectileType<MoonflareBatIllusion>(), ModContent.ProjectileType<MoonflareArrow_Proj>(), ModContent.ProjectileType<CursedThornVile>(), ModContent.ProjectileType<LeechingThornSeed>(), ModContent.ProjectileType<ThornTrap>(), ModContent.ProjectileType<ThornArrow>(), ModContent.ProjectileType<ThornTrapSmall_Proj>(), ModContent.ProjectileType<MutatedLivingBloomRoot>(), ModContent.ProjectileType<RootTendril_Proj>(), ModContent.ProjectileType<LogStaff_Proj>());

            #endregion

            #region Poison

            Poison.SetMultiple(ProjectileID.ThornChakram, ProjectileID.PoisonedKnife, ProjectileID.Stinger, ProjectileID.PoisonDart, ProjectileID.JungleSpike, ProjectileID.PoisonDartTrap, ProjectileID.PygmySpear, ProjectileID.PoisonFang, ProjectileID.PoisonDartBlowgun, ProjectileID.PoisonSeedPlantera, ProjectileID.VenomArrow, ProjectileID.VenomBullet, ProjectileID.VenomFang, ProjectileID.HornetStinger, ProjectileID.VenomSpider, ProjectileID.ToxicFlask, ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.ToxicBubble, ProjectileID.SalamanderSpit, ProjectileID.VortexAcid, ProjectileID.DD2OgreSpit, ProjectileID.QueenBeeStinger, ProjectileID.RollingCactusSpike, ModContent.ProjectileType<DevilsTongueCloud>(), ModContent.ProjectileType<StingerFriendly>(), ModContent.ProjectileType<StingerFriendlyMelee>(), ModContent.ProjectileType<FanOShivsPoison_Proj>(), ModContent.ProjectileType<Cystling>(), ModContent.ProjectileType<XenomiteGlaive_Proj>(), ModContent.ProjectileType<SeedLaser>(), ModContent.ProjectileType<SoI_ShardShot>(), ModContent.ProjectileType<SoI_ToxicSludge>(), ModContent.ProjectileType<SoI_XenomiteShot>(), ModContent.ProjectileType<OozeBall_Proj>(), ModContent.ProjectileType<GreenGas_Proj>(), ModContent.ProjectileType<GreenGloop_Proj>(), ModContent.ProjectileType<Blisterface_Bubble>(), ModContent.ProjectileType<CausticTear>(), ModContent.ProjectileType<CausticTearBall>(), ModContent.ProjectileType<PZ_Blast>(), ModContent.ProjectileType<PZ_Miniblast>(), ModContent.ProjectileType<TearOfInfection>(), ModContent.ProjectileType<TearOfInfectionBall>(), ModContent.ProjectileType<TearOfPain>(), ModContent.ProjectileType<TearOfPainBall>(), ModContent.ProjectileType<DigestiveVat_Proj>());

            #endregion

            #region Blood

            Blood.SetMultiple(ProjectileID.TheRottedFork, ProjectileID.TheMeatball, ProjectileID.BloodRain, ProjectileID.IchorArrow, ProjectileID.IchorBullet, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.VampireKnife, ProjectileID.SoulDrain, ProjectileID.IchorDart, ProjectileID.IchorSplash, ProjectileID.CrimsonYoyo, ProjectileID.BloodWater, ProjectileID.BatOfLight, ProjectileID.SharpTears, ProjectileID.DripplerFlail, ProjectileID.VampireFrog, ProjectileID.BloodShot, ProjectileID.BloodNautilusTears, ProjectileID.BloodNautilusShot, ProjectileID.BloodArrow, ProjectileID.DripplerFlailExtraBall, ProjectileID.BloodCloudRaining, ModContent.ProjectileType<LeechingThornSeed>(), ModContent.ProjectileType<KeeperBloodWave>(), ModContent.ProjectileType<BloodstainedPike_Proj>(), ModContent.ProjectileType<BloodstainedPike_Proj2>());

            #endregion

            #region Psychic

            Psychic.SetMultiple(ProjectileID.BrainScramblerBolt, ProjectileID.MedusaHeadRay, ProjectileID.BookStaffShot, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ModContent.ProjectileType<Cystling>(), ModContent.ProjectileType<Rockslide_Proj>());

            #endregion

            #region Celestial

            Celestial.SetMultiple(ProjectileID.Starfury, ProjectileID.FallingStar, ProjectileID.RainbowRodBullet, ProjectileID.HallowStar, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.PhantasmalEye, ProjectileID.PhantasmalSphere, ProjectileID.PhantasmalDeathray, ProjectileID.Meowmere, ProjectileID.StarWrath, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaBolt, ProjectileID.NebulaEye, ProjectileID.NebulaSphere, ProjectileID.NebulaLaser, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.RainbowCrystalExplosion, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.SparkleGuitar, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FairyQueenRangedItemShot, ProjectileID.FinalFractal, ProjectileID.EmpressBlade, ProjectileID.PrincessWeapon, ProjectileID.StarCannonStar, ProjectileID.SolarFlareDrill, ProjectileID.NebulaDrill, ProjectileID.VortexDrill, ProjectileID.StardustDrill, ProjectileID.MoonlordArrow, ModContent.ProjectileType<Midnight_SlashProj>(), ModContent.ProjectileType<NebulaSpark>(), ModContent.ProjectileType<NebulaStar>());

            #endregion

            #region Unparryable

            Unparryable.PopulateFromSets(ProjectileID.Sets.LightPet);
            Unparryable.PopulateFromSets(Main.projPet);
            Unparryable.SetMultiple(ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.Starfury, ProjectileID.PurificationPowder, ProjectileID.VilePowder, ProjectileID.FallingStar, ProjectileID.BallofFire, ProjectileID.MagicMissile, ProjectileID.GreenLaser, ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.Flamelash, ProjectileID.DemonScythe, ProjectileID.DemonSickle, ProjectileID.EighthNote, ProjectileID.QuarterNote, ProjectileID.TiedEighthNote, ProjectileID.RainbowRodBullet, ProjectileID.EyeLaser, ProjectileID.PinkLaser, ProjectileID.Flames, ProjectileID.PurpleLaser, ProjectileID.HallowStar, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.DeathLaser, ProjectileID.EyeFire, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.SwordBeam, ProjectileID.IceBolt, ProjectileID.FrostBoltSword, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt, ProjectileID.FrostBlastFriendly, ProjectileID.FrostBlastHostile, ProjectileID.RuneBlast, ProjectileID.TerraBeam, ProjectileID.PureSpray, ProjectileID.HallowSpray, ProjectileID.MushroomSpray, ProjectileID.CorruptSpray, ProjectileID.CrimsonSpray, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.LightBeam, ProjectileID.NightBeam, ProjectileID.EnchantedBeam, ProjectileID.IcewaterSpit, ProjectileID.FlamethrowerTrap, ProjectileID.FlamesTrap, ProjectileID.CrystalLeafShot, ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.RainFriendly, ProjectileID.BloodRain, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.BallofFrost, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.FrostBeam, ProjectileID.Fireball, ProjectileID.EyeBeam, ProjectileID.HeatRay, ProjectileID.IceSickle, ProjectileID.RainNimbus, ProjectileID.Skull, ProjectileID.DeathSickle, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.ShadowBeamHostile, ProjectileID.InfernoHostileBolt, ProjectileID.InfernoHostileBlast, ProjectileID.LostSoulHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoFriendlyBlast, ProjectileID.LostSoulFriendly, ProjectileID.SpiritHeal, ProjectileID.Shadowflames, ProjectileID.FrostBlastFriendly, ProjectileID.FlamingJack, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.NorthPoleSnowflake, ProjectileID.FrostWave, ProjectileID.SpectreWrath, ProjectileID.PulseBolt, ProjectileID.WaterGun, ProjectileID.FrostBoltStaff, ProjectileID.ImpFireball, ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.MiniRetinaLaser, ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3, ProjectileID.SlimeGun, ProjectileID.Typhoon, ProjectileID.UFOLaser, ProjectileID.ScutlixLaserFriendly, ProjectileID.MartianTurretBolt, ProjectileID.BrainScramblerBolt, ProjectileID.GigaZapperSpear, ProjectileID.RayGunnerLaser, ProjectileID.LaserMachinegunLaser, ProjectileID.ScutlixLaserCrosshair, ProjectileID.Electrosphere, ProjectileID.LaserDrill, ProjectileID.SaucerDeathray, ProjectileID.SaucerLaser, ProjectileID.InfluxWaver, ProjectileID.DrillMountCrosshair, ProjectileID.PhantasmalSphere, ProjectileID.PhantasmalDeathray, ProjectileID.ChargedBlasterOrb, ProjectileID.ChargedBlasterLaser, ProjectileID.PhantasmalBolt, ProjectileID.ViciousPowder, ProjectileID.CultistBossIceMist, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.CultistBossFireBall, ProjectileID.CultistBossFireBallClone, ProjectileID.SoulDrain, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.CultistRitual, ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardShaft, ProjectileID.ShadowFlame, ProjectileID.StarWrath, ProjectileID.Spark, ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.CoinPortal, ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.IchorSplash, ProjectileID.MedusaHeadRay, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.StardustTowerMark, ProjectileID.SporeTrap, ProjectileID.SporeTrap2, ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.NebulaBolt, ProjectileID.NebulaSphere, ProjectileID.NebulaLaser, ProjectileID.VortexLaser, ProjectileID.VortexVortexLightning, ProjectileID.VortexVortexPortal, ProjectileID.VortexLightning, ProjectileID.VortexAcid, ProjectileID.ClothiersCurse, ProjectileID.DryadsWardCircle, ProjectileID.PainterPaintball, ProjectileID.MinecartMechLaser, ProjectileID.MartianWalkerLaser, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.PortalGunBolt, ProjectileID.PortalGunGate, ProjectileID.TerrarianBeam, ProjectileID.ScutlixLaser, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSwordExplosion, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumSubshot, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.StardustGuardianExplosion, ProjectileID.PhantasmArrow, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordArrowTrail, ProjectileID.MoonlordTurretLaser, ProjectileID.RainbowCrystalExplosion, ProjectileID.LunarFlare, ProjectileID.WireKite, ProjectileID.GeyserTrap, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.SandnadoHostileMark, ProjectileID.SpiritFlame, ProjectileID.SkyFracture, ProjectileID.DD2FlameBurstTowerT1Shot, ProjectileID.DD2FlameBurstTowerT2Shot, ProjectileID.DD2FlameBurstTowerT3Shot, ProjectileID.DD2OgreStomp, ProjectileID.DD2DrakinShot, ProjectileID.DD2DarkMageBolt, ProjectileID.DD2LightningBugZap, ProjectileID.DD2OgreSmash, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.MonkStaffT1Explosion, ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2ApprenticeStorm, ProjectileID.DD2PhoenixBowShot, ProjectileID.MonkStaffT3_AltShot, ProjectileID.ApprenticeStaffT3Shot, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.ThunderStaffShot, ProjectileID.ThunderSpearShot, ProjectileID.VoidLens, ProjectileID.SharpTears, ProjectileID.BloodShot, ProjectileID.BloodNautilusTears, ProjectileID.BloodNautilusShot, ProjectileID.BookOfSkullsSkull, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.ZapinatorLaser, ProjectileID.ScytheWhipProj, ProjectileID.CoolWhipProj, ProjectileID.FireWhipProj, ProjectileID.FairyQueenLance, ProjectileID.QueenSlimeSmash, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.StardustPunch, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FinalFractal, ProjectileID.TorchGod, ProjectileID.PrincessWeapon, ProjectileID.DaybreakExplosion, ProjectileID.WandOfSparkingSpark, ProjectileID.StarCannonStar, ProjectileID.DeerclopsIceSpike, ProjectileID.InsanityShadowFriendly, ProjectileID.InsanityShadowHostile, ProjectileID.HoundiusShootiusFireball, ProjectileID.WeatherPainShot, ProjectileID.AbigailCounter);

            #endregion

            #region NoElement

            NoElement.SetMultiple(ProjectileID.CorruptSpray, ProjectileID.CrimsonSpray, ProjectileID.HallowSpray, ProjectileID.MushroomSpray, ProjectileID.PureSpray, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ModContent.ProjectileType<BleachedSolution_Proj>());

            #endregion
        }
    }
}

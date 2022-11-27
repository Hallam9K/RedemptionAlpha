using System.Collections.Generic;
using Redemption.NPCs.Critters;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.PreHM;
using Redemption.Projectiles.Hostile;
using Redemption.NPCs.Friendly;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Projectiles.Ranged;
using Redemption.Items.Weapons.PreHM.Ammo;
using Redemption.Projectiles.Magic;
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
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.NPCs.HM;
using Redemption.Projectiles.Ritualist;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.ADD;
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Items.Weapons.HM.Ammo;
using Redemption.Items.Weapons.PostML.Summon;

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
            ModContent.TileType<GathicFroststoneBrickTile>(),
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

        public static List<int> AncientTileArray = new()
        {
            ModContent.TileType<GathicStoneTile>(), ModContent.TileType<GathicStoneBrickTile>(), ModContent.TileType<GathicGladestoneTile>(), ModContent.TileType<GathicGladestoneBrickTile>(), ModContent.TileType<GathicFroststoneTile>(), ModContent.TileType<GathicFroststoneBrickTile>(), ModContent.TileType<GathicColdstoneTile>(), ModContent.TileType<GathicColdstoneBrickTile>()
        };
        public static List<int> WoodLeaf = new()
        {
            TileID.WoodBlock, TileID.BorealWood, TileID.DynastyWood, TileID.LivingWood, TileID.PalmWood, TileID.SpookyWood, TileID.Ebonwood, TileID.Pearlwood, TileID.Shadewood, TileID.LivingMahogany, TileID.RichMahogany, TileID.LeafBlock, TileID.LivingMahoganyLeaves
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

        public static List<int> Undead = new() { 3, 132, 186, 187, 188, 189, 200, 223, 161, 254, 255, 52, 53, 536, 319, 320, 321, 332, 436, 431, 432, 433, 434, 435, 331, 430, 590, NPCID.TorchZombie, NPCID.ArmedTorchZombie, NPCID.MaggotZombie, NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.BloodZombie, NPCID.ZombieMerman, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.Eyezor, NPCID.Frankenstein, NPCID.Vampire, NPCID.VampireBat, NPCID.HeadlessHorseman, NPCID.ZombieElf, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, ModContent.NPCType<JollyMadman>(), ModContent.NPCType<Keeper>(), ModContent.NPCType<SkullDigger>(), ModContent.NPCType<Fallen>(), ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<IrradiatedBehemoth>() };

        #endregion

        #region Spirit

        public static List<int> Spirit = new() { NPCID.EnchantedSword, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.DesertDjinn, NPCID.DungeonSpirit, NPCID.FloatyGross, NPCID.Ghost, NPCID.PossessedArmor, NPCID.Wraith, NPCID.Reaper, NPCID.Poltergeist, NPCID.PirateGhost, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<VagrantSpirit>(), ModContent.NPCType<KeeperSpirit>(), ModContent.NPCType<ErhanSpirit>(), ModContent.NPCType<LostSoulNPC>(), ModContent.NPCType<NuclearShadow>(), ModContent.NPCType<WraithSlayer_Samurai>() };

        #endregion

        #region Plantlike

        public static List<int> Plantlike = new() { NPCID.FungiBulb, NPCID.AnomuraFungus, NPCID.MushiLadybug, NPCID.ManEater, NPCID.Snatcher, NPCID.AngryTrapper, NPCID.FungoFish, NPCID.GiantFungiBulb, NPCID.HoppinJack, NPCID.Dandelion, NPCID.Plantera, NPCID.MourningWood, NPCID.Splinterling, NPCID.Pumpking, NPCID.Everscream, NPCID.PlanterasTentacle, ModContent.NPCType<LivingBloom>(), ModContent.NPCType<DevilsTongue>(), ModContent.NPCType<Thorn>(), ModContent.NPCType<TreebarkDryad>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<ForestNymph>(), ModContent.NPCType<Akka>() };

        #endregion

        #region Demon
        public static List<int> Demon = new() { NPCID.Demon, NPCID.VoodooDemon, NPCID.FireImp, NPCID.RedDevil, NPCID.WallofFlesh, NPCID.WallofFleshEye };
        #endregion

        #region Cold
        public static List<int> Cold = new() { NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.IceBat, NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.SnowFlinx, NPCID.IceElemental, NPCID.IceMimic, NPCID.IceTortoise, NPCID.IcyMerman, NPCID.MisterStabby, NPCID.Wolf, NPCID.IceGolem, NPCID.SnowBalla, NPCID.SnowmanGangsta, NPCID.Flocko, NPCID.Yeti, NPCID.IceQueen, NPCID.Deerclops, NPCID.DeerclopsLeg, ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>() };
        #endregion

        #region Hot
        public static List<int> Hot = new() { NPCID.Antlion, NPCID.FlyingAntlion, NPCID.GiantFlyingAntlion, NPCID.GiantWalkingAntlion, NPCID.LarvaeAntlion, NPCID.WalkingAntlion, NPCID.FireImp, NPCID.Hellbat, NPCID.LavaSlime, NPCID.MeteorHead, NPCID.SandSlime, NPCID.TombCrawlerHead, NPCID.TombCrawlerBody, NPCID.TombCrawlerTail, NPCID.Vulture, NPCID.DesertBeast, NPCID.DuneSplicerHead, NPCID.DuneSplicerBody, NPCID.DuneSplicerTail, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.DesertDjinn, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.HoppinJack, NPCID.Lavabat, NPCID.DesertLamiaDark, NPCID.DesertLamiaLight, NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.LightMummy, NPCID.Tumbleweed, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.SandElemental, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.DD2Betsy, NPCID.Pumpking, NPCID.MourningWood, NPCID.LunarTowerSolar, ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<DevilsTongue>() };
        #endregion

        #region Wet
        public static List<int> Wet = new() { NPCID.BlueJellyfish, NPCID.PinkJellyfish, NPCID.Piranha, NPCID.Shark, NPCID.SeaSnail, NPCID.Squid, NPCID.AnglerFish, NPCID.Arapaima, NPCID.BloodJelly, NPCID.BloodFeeder, NPCID.FungoFish, NPCID.FloatyGross, NPCID.GreenJellyfish, NPCID.IcyMerman, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.BloodSquid, NPCID.GoblinShark, NPCID.Drippler, NPCID.BloodZombie, NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.ZombieMerman, NPCID.FlyingFish, NPCID.AngryNimbus, NPCID.CreatureFromTheDeep, NPCID.SwampThing, NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2, ModContent.NPCType<BlisteredFish>(), ModContent.NPCType<BlisteredFish2>(), ModContent.NPCType<Blisterface>(), ModContent.NPCType<BloatedGoldfish>(), ModContent.NPCType<RadioactiveJelly>() };
        #endregion

        #region Dragonlike
        public static List<int> Dragonlike = new() { NPCID.DD2Betsy, NPCID.DD2WyvernT1, NPCID.DD2WyvernT2, NPCID.DD2WyvernT3, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.DukeFishron, NPCID.WyvernHead, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernLegs, NPCID.WyvernTail, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail };
        #endregion

        #region Inorganic
        public static List<int> Inorganic = new() { NPCID.GraniteFlyer, NPCID.GraniteGolem, NPCID.MeteorHead, NPCID.Mimic, NPCID.BigMimicCorruption, NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle, NPCID.IceMimic, NPCID.PresentMimic, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.EnchantedSword, NPCID.IceElemental, NPCID.MartianProbe, NPCID.PossessedArmor, NPCID.Pixie, NPCID.Paladin, NPCID.RockGolem, NPCID.ChatteringTeethBomb, NPCID.AngryNimbus, NPCID.IceGolem, NPCID.Tumbleweed, NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.SnowBalla, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Flocko, NPCID.GingerbreadMan, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.SolarCorite, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.PirateShipCannon, NPCID.IceQueen, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<EaglecrestGolem>(), ModContent.NPCType<EaglecrestGolem_Sleep>(), ModContent.NPCType<EaglecrestRockPile>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>(), ModContent.NPCType<OO>(), ModContent.NPCType<Android>(), ModContent.NPCType<PrototypeSilver>(), ModContent.NPCType<SpacePaladin>(), ModContent.NPCType<Akka>(), ModContent.NPCType<Ukko>(), ModContent.NPCType<EaglecrestGolem2>(), ModContent.NPCType<EaglecrestRockPile2>() };
        #endregion

        #region Robotic
        public static List<int> Robotic = new() { NPCID.MartianProbe, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>(), ModContent.NPCType<OO>(), ModContent.NPCType<Android>(), ModContent.NPCType<PrototypeSilver>(), ModContent.NPCType<SpacePaladin>() };
        #endregion

        #region Infected
        public static List<int> Infected = new() { ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<OozeBlob>(), ModContent.NPCType<SeedGrowth>(), ModContent.NPCType<SoI>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<RadioactiveSlime>(), ModContent.NPCType<NuclearSlime>(), ModContent.NPCType<IrradiatedBehemoth>(), ModContent.NPCType<Blisterface>(), ModContent.NPCType<BlisteredFish>(), ModContent.NPCType<BlisteredFish2>(), ModContent.NPCType<SickenedDemonEye>(), ModContent.NPCType<SickenedBunny>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>(), ModContent.NPCType<PZ>(), ModContent.NPCType<PZ_Kari>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<BloatedGoldfish>(), ModContent.NPCType<RadioactiveJelly>() };
        #endregion

        #region Armed
        public static List<int> Armed = new() { NPCID.RedDevil, NPCID.Paladin, NPCID.GoblinThief, NPCID.DD2GoblinT1, NPCID.DD2GoblinT2, NPCID.DD2GoblinT3, NPCID.MisterStabby, NPCID.PirateCorsair, NPCID.PirateGhost, NPCID.Butcher, NPCID.Psycho, NPCID.Reaper, NPCID.SolarDrakomireRider, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.Pumpking, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesSword, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSword, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<WraithSlayer_Samurai>(), ModContent.NPCType<SpacePaladin>() };
        #endregion

        #region Hallowed
        public static List<int> Hallowed = new() { NPCID.ChaosElemental, NPCID.DesertGhoulHallow, NPCID.EnchantedSword, NPCID.BigMimicHallow, NPCID.IlluminantBat, NPCID.IlluminantSlime, NPCID.LightMummy, NPCID.Pixie, NPCID.Paladin, NPCID.Unicorn, NPCID.RainbowSlime, NPCID.SandsharkHallow, NPCID.HallowBoss, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, ModContent.NPCType<Erhan>(), ModContent.NPCType<ErhanSpirit>() };
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

        public static List<int> Arcane = new() { ProjectileID.EnchantedBoomerang, ProjectileID.Starfury, ProjectileID.MagicMissile, ProjectileID.EighthNote, ProjectileID.QuarterNote, ProjectileID.TiedEighthNote, ProjectileID.RainbowRodBullet, ProjectileID.EyeLaser, ProjectileID.PinkLaser, ProjectileID.PurpleLaser, ProjectileID.MagicDagger, ProjectileID.CrystalStorm, ProjectileID.DeathLaser, ProjectileID.SwordBeam, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt, ProjectileID.RuneBlast, ProjectileID.TerraBeam, ProjectileID.LightBeam, ProjectileID.NightBeam, ProjectileID.EnchantedBeam, ProjectileID.FrostBeam, ProjectileID.EyeBeam, ProjectileID.Skull, ProjectileID.DeathSickle, ProjectileID.LostSoulFriendly, ProjectileID.LostSoulHostile, ProjectileID.Shadowflames, ProjectileID.VampireKnife, ProjectileID.SpectreWrath, ProjectileID.PulseBolt, ProjectileID.MiniRetinaLaser, ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardShaft, ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.MedusaHeadRay, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaLaser, ProjectileID.VortexLaser, ProjectileID.ClothiersCurse, ProjectileID.MinecartMechLaser, ProjectileID.TerrarianBeam, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.StardustGuardianExplosion, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.PhantasmArrow, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.LunarFlare, ProjectileID.SkyFracture, ProjectileID.DD2DarkMageBolt, ProjectileID.BookOfSkullsSkull, ProjectileID.SparkleGuitar, ProjectileID.TitaniumStormShard, ProjectileID.StardustPunch, ProjectileID.NebulaDrill, ProjectileID.StardustDrill, ProjectileID.JestersArrow, ProjectileID.MonkStaffT2Ghast, ModContent.ProjectileType<KeeperSoulCharge>(), ModContent.ProjectileType<SoulScepterCharge>(), ModContent.ProjectileType<SoulScepterChargeS>(), ModContent.ProjectileType<GiantMask>(), ModContent.ProjectileType<SpectralScythe_Proj>(), ModContent.ProjectileType<WraithSlayer_Proj>(), ModContent.ProjectileType<SpiritArrow_Proj>(), ModContent.ProjectileType<SpiritArrow_Shard>(), ModContent.ProjectileType<NoidanNuoli>(), ModContent.ProjectileType<Moonbeam>(), ModContent.ProjectileType<XenomiteScepter_Proj>(), ModContent.ProjectileType<Dusksong_Proj>(), ModContent.ProjectileType<Dusksong_Proj2>() };

        public static List<int> Fire = new() { ProjectileID.FireArrow, ProjectileID.BallofFire, ProjectileID.Flamarang, ProjectileID.Flamelash, ProjectileID.Sunfury, ProjectileID.HellfireArrow, ProjectileID.FlamingArrow, ProjectileID.Flames, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.RuneBlast, ProjectileID.FrostburnArrow, ProjectileID.FlamethrowerTrap, ProjectileID.FlamesTrap, ProjectileID.Fireball, ProjectileID.HeatRay, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.FlamingWood, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.ImpFireball, ProjectileID.MolotovCocktail, ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.CultistBossFireBall, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.Hellwing, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.Spark, ProjectileID.HelFire, ProjectileID.ClothiersCurse, ProjectileID.DesertDjinnCurse, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.GeyserTrap, ProjectileID.SpiritFlame, ProjectileID.DD2FlameBurstTowerT1Shot, ProjectileID.DD2FlameBurstTowerT2Shot, ProjectileID.DD2FlameBurstTowerT3Shot, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.MonkStaffT2, ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2PhoenixBowShot, ProjectileID.DD2BetsyArrow, ProjectileID.ApprenticeStaffT3Shot, ProjectileID.FireWhipProj, ProjectileID.FireWhip, ProjectileID.FlamingMace, ProjectileID.TorchGod, ProjectileID.WandOfSparkingSpark, ProjectileID.SolarFlareDrill, ProjectileID.HoundiusShootiusFireball, ProjectileID.Flare, ProjectileID.BlueFlare, ProjectileID.FlyingImp, ModContent.ProjectileType<FlintAndSteelSpark>(), ModContent.ProjectileType<LunarShot_Proj>(), ModContent.ProjectileType<MoonflareBatIllusion>(), ModContent.ProjectileType<MoonflareArrow_Proj>(), ModContent.ProjectileType<CantripEmber>(), ModContent.ProjectileType<CantripEmberS>(), ModContent.ProjectileType<DragonCleaver_Proj>(), ModContent.ProjectileType<FireSlash_Proj>(), ModContent.ProjectileType<DragonSkull_Proj>(), ModContent.ProjectileType<HeatRay>(), ModContent.ProjectileType<ScorchingRay>(), ModContent.ProjectileType<RayOfGuidance>(), ModContent.ProjectileType<MagnifyingGlassRay>(), ModContent.ProjectileType<MagmaCube>(), ModContent.ProjectileType<Firebreak_Proj>(), ModContent.ProjectileType<MACE_FireBlast>(), ModContent.ProjectileType<MACE_Miniblast>(), ModContent.ProjectileType<Hacksaw_Heat_Proj>(), ModContent.ProjectileType<DragonSkullFlames_Proj>(), ModContent.ProjectileType<BlazingBalisong_Slash>(), ModContent.ProjectileType<HellfireCharge_Proj>(), ModContent.ProjectileType<Gigapora_Flame>(), ModContent.ProjectileType<Gigapora_Fireball>(), ModContent.ProjectileType<Divinity_Ball>(), ModContent.ProjectileType<Divinity_Sun>(), ModContent.ProjectileType<ForgottenSword_Proj>(), ModContent.ProjectileType<ForgottenGreatsword_Proj>(), ModContent.ProjectileType<Firestorm_Proj>(), ModContent.ProjectileType<SunInThePalm_EnergyBall>() };

        public static List<int> Water = new() { ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.BlueMoon, ProjectileID.HolyWater, ProjectileID.UnholyWater, ProjectileID.IcewaterSpit, ProjectileID.RainFriendly, ProjectileID.BloodRain, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.WaterGun, ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.FlaironBubble, ProjectileID.SlimeGun, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.Bubble, ProjectileID.Xenopopper, ProjectileID.ToxicBubble, ProjectileID.Kraken, ProjectileID.BloodWater, ProjectileID.Ale, ProjectileID.DD2OgreSpit, ProjectileID.QueenSlimeGelAttack, ProjectileID.GelBalloon, ProjectileID.VolatileGelatinBall, ProjectileID.Flairon, ProjectileID.MaceWhip, ProjectileID.Trident, ModContent.ProjectileType<WaterOrb>(), ModContent.ProjectileType<WaterOrbS>(), ModContent.ProjectileType<BucketSplash>(), ModContent.ProjectileType<Blisterface_Bubble>(), ModContent.ProjectileType<DigestiveVat_Proj>(), ModContent.ProjectileType<TidalWake_Proj>(), ModContent.ProjectileType<AkkaBubble>(), ModContent.ProjectileType<AkkaPoisonBubble>(), ModContent.ProjectileType<UkkoRain>(), ModContent.ProjectileType<UkkoRainHealing>(), ModContent.ProjectileType<UkkoRainPoison>(), ModContent.ProjectileType<AquaArrow_Proj>() };

        public static List<int> Ice = new() { ProjectileID.IceBlock, ProjectileID.IceBoomerang, ProjectileID.IceBolt, ProjectileID.FrostBoltSword, ProjectileID.FrostArrow, ProjectileID.FrostBlastHostile, ProjectileID.SnowBallFriendly, ProjectileID.FrostburnArrow, ProjectileID.IceSpike, ProjectileID.IcewaterSpit, ProjectileID.BallofFrost, ProjectileID.FrostBeam, ProjectileID.IceSickle, ProjectileID.FrostBlastFriendly, ProjectileID.Blizzard, ProjectileID.NorthPoleWeapon, ProjectileID.NorthPoleSpear, ProjectileID.NorthPoleSnowflake, ProjectileID.FrostWave, ProjectileID.FrostShard, ProjectileID.FrostBoltStaff, ProjectileID.CultistBossIceMist, ProjectileID.FrostDaggerfish, ProjectileID.Amarok, ProjectileID.CoolWhip, ProjectileID.CoolWhipProj, ProjectileID.DeerclopsIceSpike, ProjectileID.DeerclopsRangedProjectile, ProjectileID.FlinxMinion, ModContent.ProjectileType<IceBolt>(), ModContent.ProjectileType<PureIronSword_Proj>(), ModContent.ProjectileType<BladeOfTheMountain_Slash>(), ModContent.ProjectileType<Icefall_Mist>(), ModContent.ProjectileType<Icefall_Proj>(), ModContent.ProjectileType<UkkoBlizzard>(), ModContent.ProjectileType<UkkoHail>() };

        public static List<int> Earth = new() { ProjectileID.Boulder, ProjectileID.BoulderStaffOfEarth, ProjectileID.GolemFist, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2OgreStomp, ProjectileID.DD2OgreSmash, ProjectileID.MonkStaffT1Explosion, ProjectileID.RollingCactus, ProjectileID.RockGolemRock, ModContent.ProjectileType<AncientGladestonePillar>(), ModContent.ProjectileType<EaglecrestSling_Throw>(), ModContent.ProjectileType<EaglecrestJavelin_Proj>(), ModContent.ProjectileType<CalciteWand_Proj>(), ModContent.ProjectileType<Rockslide_Proj>(), ModContent.ProjectileType<RockslidePebble_Proj>(), ModContent.ProjectileType<AkkaIslandHitbox>(), ModContent.ProjectileType<Jyrina>() };

        public static List<int> Wind = new() { ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2ApprenticeStorm, ProjectileID.BookStaffShot, ProjectileID.WeatherPainShot, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.LightDisc, ProjectileID.FlyingKnife, ModContent.ProjectileType<FireSlash_Proj>(), ModContent.ProjectileType<RockSlash_Proj>(), ModContent.ProjectileType<KS3_Wave>(), ModContent.ProjectileType<GreenGas_Proj>(), ModContent.ProjectileType<Icefall_Mist>(), ModContent.ProjectileType<XenomiteGas_Proj>(), ModContent.ProjectileType<UkkoGust>(), ModContent.ProjectileType<MythrilsBaneSlash_Proj>(), ModContent.ProjectileType<ForgottenGreatsword_Proj>(), ModContent.ProjectileType<Firestorm_Proj>(), ModContent.ProjectileType<GiantDandelionSeed>() };

        public static List<int> Thunder = new() { ProjectileID.RuneBlast, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.UFOLaser, ProjectileID.ScutlixLaser, ProjectileID.ScutlixLaserFriendly, ProjectileID.MartianTurretBolt, ProjectileID.BrainScramblerBolt, ProjectileID.GigaZapperSpear, ProjectileID.RayGunnerLaser, ProjectileID.LaserMachinegunLaser, ProjectileID.Electrosphere, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerDeathray, ProjectileID.SaucerLaser, ProjectileID.InfluxWaver, ProjectileID.ChargedBlasterLaser, ProjectileID.ChargedBlasterOrb, ProjectileID.PhantasmalBolt, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.DeadlySphere, ProjectileID.VortexVortexLightning, ProjectileID.VortexLightning, ProjectileID.MartianWalkerLaser, ProjectileID.VortexBeaterRocket, ProjectileID.DD2LightningBugZap, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.MonkStaffT3_AltShot, ProjectileID.ThunderSpear, ProjectileID.ThunderStaffShot, ProjectileID.ThunderSpearShot, ProjectileID.ZapinatorLaser, ProjectileID.VortexDrill, ProjectileID.InfluxWaver, ModContent.ProjectileType<KS3_BeamCell>(), ModContent.ProjectileType<KS3_MagnetBeam>(), ModContent.ProjectileType<Electronade_TeslaField>(), ModContent.ProjectileType<Electronade_Proj>(), ModContent.ProjectileType<EaglecrestJavelin_Thunder>(), ModContent.ProjectileType<Volt_OrbProj>(), ModContent.ProjectileType<TeslaBeam>(), ModContent.ProjectileType<TeslaZapBeam>(), ModContent.ProjectileType<BigElectronade_TeslaField>(), ModContent.ProjectileType<GravityHammer_Proj>(), ModContent.ProjectileType<LightningRod_Proj>(), ModContent.ProjectileType<XeniumLance_Proj>(), ModContent.ProjectileType<KS3_EnergyBolt>(), ModContent.ProjectileType<GlobalDischarge_Sphere>(), ModContent.ProjectileType<OO_StunBeam>(), ModContent.ProjectileType<OmegaPlasmaBall>(), ModContent.ProjectileType<PrototypeSilver_Beam>(), ModContent.ProjectileType<CorruptedDoubleRifle_Beam>(), ModContent.ProjectileType<CyberChakram_Proj>(), ModContent.ProjectileType<CyberChakram_Proj2>(), ModContent.ProjectileType<PlutoniumBeam>(), ModContent.ProjectileType<GirusDischarge>(), ModContent.ProjectileType<AkkaBubble>(), ModContent.ProjectileType<DualcastBall>(), ModContent.ProjectileType<UkkoCloud_Thunder>(), ModContent.ProjectileType<UkkoLightning>(), ModContent.ProjectileType<UkkoThunderwave>(), ModContent.ProjectileType<UkkoStrikeZap>(), ModContent.ProjectileType<UkkoStrike>(), ModContent.ProjectileType<TeslaCoil_Proj>(), ModContent.ProjectileType<XeniumElectrolaser_Beam>(), ModContent.ProjectileType<XeniumElectrolaser_Beam2>(), ModContent.ProjectileType<UkonvasaraArrow>(), ModContent.ProjectileType<UkonArrowStrike>() };

        public static List<int> Holy = new() { ProjectileID.TheDaoofPow, ProjectileID.HolyWater, ProjectileID.HolyArrow, ProjectileID.HallowStar, ProjectileID.LightBeam, ProjectileID.Hamdrax, ProjectileID.PaladinsHammerHostile, ProjectileID.PaladinsHammerFriendly, ProjectileID.SkyFracture, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.BatOfLight, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.HallowJoustingLance, ProjectileID.RainbowWhip, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.LightDisc, ProjectileID.SwordWhip, ProjectileID.Gungnir, ModContent.ProjectileType<CorpseWalkerBolt>(), ModContent.ProjectileType<CorpseWalkerSkull>(), ModContent.ProjectileType<CorpseWalkerSkull_Proj>(), ModContent.ProjectileType<Sunshard>(), ModContent.ProjectileType<SunshardRay>(), ModContent.ProjectileType<Lightmass>(), ModContent.ProjectileType<ScorchingRay>(), ModContent.ProjectileType<RayOfGuidance>(), ModContent.ProjectileType<HolySpear_Proj>(), ModContent.ProjectileType<HolyBible_Proj>(), ModContent.ProjectileType<HolyBible_Ray>(), ModContent.ProjectileType<MagnifyingGlassRay>(), ModContent.ProjectileType<HallowedHandGrenade_Proj>(), ModContent.ProjectileType<CrystalGlaive_Proj>(), ModContent.ProjectileType<BlindJustice_Proj>(), ModContent.ProjectileType<HolyPhalanx_Proj>(), ModContent.ProjectileType<HolyPhalanx_Proj2>(), ModContent.ProjectileType<Bible_SeedSpear>(), ModContent.ProjectileType<UkkoDancingLights>(), ModContent.ProjectileType<Divinity_Ball>(), ModContent.ProjectileType<Divinity_Sun>() };

        public static List<int> Shadow = new() { ProjectileID.UnholyArrow, ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.BallOHurt, ProjectileID.DemonSickle, ProjectileID.DemonScythe, ProjectileID.DarkLance, ProjectileID.TheDaoofPow, ProjectileID.UnholyWater, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.NightBeam, ProjectileID.DeathSickle, ProjectileID.ShadowBeamHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.Shadowflames, ProjectileID.EatersBite, ProjectileID.TinyEater, ProjectileID.CultistBossFireBallClone, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.CorruptYoyo, ProjectileID.ClothiersCurse, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.SpiritFlame, ProjectileID.BlackBolt, ProjectileID.DD2DrakinShot, ProjectileID.DD2DarkMageBolt, ProjectileID.ShadowJoustingLance, ProjectileID.ScytheWhipProj, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ModContent.ProjectileType<KeeperDreadCoil>(), ModContent.ProjectileType<ShadowBolt>(), ModContent.ProjectileType<GiantMask>(), ModContent.ProjectileType<WraithSlayer_Proj>(), ModContent.ProjectileType<SoulSkewer_Slash>(), ModContent.ProjectileType<DarkSteelArrow>(), ModContent.ProjectileType<DarkSteelArrow_Tendril>(), ModContent.ProjectileType<Dusksong_Proj>(), ModContent.ProjectileType<Dusksong_Proj2>() };

        public static List<int> Nature = new() { ProjectileID.ThornChakram, ProjectileID.Seed, ProjectileID.Mushroom, ProjectileID.TerraBeam, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.JungleSpike, ProjectileID.Leaf, ProjectileID.FlowerPetal, ProjectileID.CrystalLeafShot, ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.FlowerPow, ProjectileID.FlowerPowPetal, ProjectileID.SeedPlantera, ProjectileID.PoisonSeedPlantera, ProjectileID.ThornBall, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.PineNeedleFriendly, ProjectileID.PineNeedleHostile, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn, ProjectileID.JungleYoyo, ProjectileID.SporeTrap, ProjectileID.SporeTrap2, ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.TruffleSpore, ProjectileID.Terrarian, ProjectileID.TerrarianBeam, ProjectileID.Terragrim, ProjectileID.DandelionSeed, ProjectileID.Shroomerang, ProjectileID.ThornWhip, ProjectileID.BabyBird, ProjectileID.MushroomSpear, ProjectileID.ChlorophyteArrow, ProjectileID.ChlorophyteBullet, ProjectileID.ChlorophyteChainsaw, ProjectileID.ChlorophyteJackhammer, ProjectileID.ChlorophyteDrill, ProjectileID.ChlorophytePartisan, ModContent.ProjectileType<LivingBloomRoot>(), ModContent.ProjectileType<LunarShot_Proj>(), ModContent.ProjectileType<MoonflareBatIllusion>(), ModContent.ProjectileType<MoonflareArrow_Proj>(), ModContent.ProjectileType<CursedThornVile>(), ModContent.ProjectileType<LeechingThornSeed>(), ModContent.ProjectileType<ThornTrap>(), ModContent.ProjectileType<ThornArrow>(), ModContent.ProjectileType<ThornTrapSmall_Proj>(), ModContent.ProjectileType<MutatedLivingBloomRoot>(), ModContent.ProjectileType<RootTendril_Proj>(), ModContent.ProjectileType<LogStaff_Proj>(), ModContent.ProjectileType<BuddingBoline_Slash>(), ModContent.ProjectileType<BlightedBoline_Slash>(), ModContent.ProjectileType<BolineFlower>(), ModContent.ProjectileType<CursedThornVileF>(), ModContent.ProjectileType<GiantDandelionSeed>() };

        public static List<int> Poison = new() { ProjectileID.ThornChakram, ProjectileID.PoisonedKnife, ProjectileID.Stinger, ProjectileID.PoisonDart, ProjectileID.JungleSpike, ProjectileID.PoisonDartTrap, ProjectileID.PygmySpear, ProjectileID.PoisonFang, ProjectileID.PoisonDartBlowgun, ProjectileID.PoisonSeedPlantera, ProjectileID.VenomArrow, ProjectileID.VenomBullet, ProjectileID.VenomFang, ProjectileID.HornetStinger, ProjectileID.VenomSpider, ProjectileID.ToxicFlask, ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.ToxicBubble, ProjectileID.SalamanderSpit, ProjectileID.VortexAcid, ProjectileID.DD2OgreSpit, ProjectileID.QueenBeeStinger, ProjectileID.RollingCactusSpike, ModContent.ProjectileType<DevilsTongueCloud>(), ModContent.ProjectileType<StingerFriendly>(), ModContent.ProjectileType<StingerFriendlyMelee>(), ModContent.ProjectileType<FanOShivsPoison_Proj>(), ModContent.ProjectileType<Cystling>(), ModContent.ProjectileType<XenoXyston_Proj>(), ModContent.ProjectileType<SeedLaser>(), ModContent.ProjectileType<SoI_ShardShot>(), ModContent.ProjectileType<SoI_ToxicSludge>(), ModContent.ProjectileType<SoI_XenomiteShot>(), ModContent.ProjectileType<OozeBall_Proj>(), ModContent.ProjectileType<GreenGas_Proj>(), ModContent.ProjectileType<GreenGloop_Proj>(), ModContent.ProjectileType<Blisterface_Bubble>(), ModContent.ProjectileType<CausticTear>(), ModContent.ProjectileType<CausticTearBall>(), ModContent.ProjectileType<PZ_Blast>(), ModContent.ProjectileType<PZ_Miniblast>(), ModContent.ProjectileType<TearOfInfection>(), ModContent.ProjectileType<TearOfInfectionBall>(), ModContent.ProjectileType<TearOfPain>(), ModContent.ProjectileType<TearOfPainBall>(), ModContent.ProjectileType<DigestiveVat_Proj>(), ModContent.ProjectileType<Chernobyl_Proj>(), ModContent.ProjectileType<XenomiteGas_Proj>(), ModContent.ProjectileType<AkkaPoisonBubble>(), ModContent.ProjectileType<XenomiteScepter_Proj>(), ModContent.ProjectileType<BileLauncher_Gloop>(), ModContent.ProjectileType<ToxicGrenade_Proj>(), ModContent.ProjectileType<PZGauntlet_Proj>(), ModContent.ProjectileType<PZGauntlet_Proj2>(), ModContent.ProjectileType<XeniumBubble_Proj>(), ModContent.ProjectileType<XeniumStaff_Proj>(), ModContent.ProjectileType<HiveCyst_Proj>(), ModContent.ProjectileType<InfectiousGlaive_Proj>(), ModContent.ProjectileType<InfectedTentacle_Proj>(), ModContent.ProjectileType<XenomiteBulletProj>(), ModContent.ProjectileType<BileBullet_Proj>(), ModContent.ProjectileType<BileArrow_Proj>() };

        public static List<int> Blood = new() { ProjectileID.TheRottedFork, ProjectileID.TheMeatball, ProjectileID.BloodRain, ProjectileID.IchorArrow, ProjectileID.IchorBullet, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.VampireKnife, ProjectileID.SoulDrain, ProjectileID.IchorDart, ProjectileID.IchorSplash, ProjectileID.CrimsonYoyo, ProjectileID.BloodWater, ProjectileID.BatOfLight, ProjectileID.SharpTears, ProjectileID.DripplerFlail, ProjectileID.VampireFrog, ProjectileID.BloodShot, ProjectileID.BloodNautilusTears, ProjectileID.BloodNautilusShot, ProjectileID.BloodArrow, ProjectileID.DripplerFlailExtraBall, ProjectileID.BloodCloudRaining, ProjectileID.ButchersChainsaw, ModContent.ProjectileType<LeechingThornSeed>(), ModContent.ProjectileType<KeeperBloodWave>(), ModContent.ProjectileType<BloodstainedPike_Proj>(), ModContent.ProjectileType<BloodstainedPike_Proj2>(), ModContent.ProjectileType<BloodLetter_Slash>() };

        public static List<int> Psychic = new() { ProjectileID.BrainScramblerBolt, ProjectileID.MedusaHeadRay, ProjectileID.BookStaffShot, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ModContent.ProjectileType<Cystling>(), ModContent.ProjectileType<Rockslide_Proj>(), ModContent.ProjectileType<Nanite_Proj>() };

        public static List<int> Celestial = new() { ProjectileID.Starfury, ProjectileID.FallingStar, ProjectileID.RainbowRodBullet, ProjectileID.HallowStar, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.PhantasmalEye, ProjectileID.PhantasmalSphere, ProjectileID.PhantasmalDeathray, ProjectileID.Meowmere, ProjectileID.StarWrath, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaBolt, ProjectileID.NebulaEye, ProjectileID.NebulaSphere, ProjectileID.NebulaLaser, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.RainbowCrystalExplosion, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.SparkleGuitar, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FairyQueenRangedItemShot, ProjectileID.FinalFractal, ProjectileID.EmpressBlade, ProjectileID.PrincessWeapon, ProjectileID.StarCannonStar, ProjectileID.SolarFlareDrill, ProjectileID.NebulaDrill, ProjectileID.VortexDrill, ProjectileID.StardustDrill, ProjectileID.MoonlordArrow, ProjectileID.MoonlordBullet, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ModContent.ProjectileType<Midnight_SlashProj>(), ModContent.ProjectileType<NebulaSpark>(), ModContent.ProjectileType<NebulaStar>(), ModContent.ProjectileType<CosmicEye_Beam>(), ModContent.ProjectileType<CrystalStar_Proj>(), ModContent.ProjectileType<CrystalStarShard_Proj>(), ModContent.ProjectileType<CurvingStar>(), ModContent.ProjectileType<CurvingStar2>(), ModContent.ProjectileType<GiantStar_Proj>(), ModContent.ProjectileType<PNebula1>(), ModContent.ProjectileType<PNebula2>(), ModContent.ProjectileType<PNebula3>(), ModContent.ProjectileType<StarFall_Proj>(), ModContent.ProjectileType<StationaryStar>(), ModContent.ProjectileType<CosmicEye_Beam2>(), ModContent.ProjectileType<StarBolt>(), ModContent.ProjectileType<StarFall_Proj2>(), ModContent.ProjectileType<SpellsongMirage_Proj>(), ModContent.ProjectileType<SpellsongSlash_Proj>(), ModContent.ProjectileType<Spellsong_Proj>(), ModContent.ProjectileType<Moonbeam>(), ModContent.ProjectileType<PNebula1_Friendly>(), ModContent.ProjectileType<PNebula2_Friendly>(), ModContent.ProjectileType<PNebula3_Friendly>(), ModContent.ProjectileType<Constellations_Star>(), ModContent.ProjectileType<CosmosChains_Proj>(), ModContent.ProjectileType<ChainsCosmicEye_Beam>() };

        public static List<int> NoElement = new() { ProjectileID.CorruptSpray, ProjectileID.CrimsonSpray, ProjectileID.HallowSpray, ProjectileID.MushroomSpray, ProjectileID.PureSpray, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ModContent.ProjectileType<BleachedSolution_Proj>() };

        public static List<int> Explosive = new() { ProjectileID.Bomb, ProjectileID.BombFish, ProjectileID.Grenade, ProjectileID.Dynamite, ProjectileID.StickyBomb, ProjectileID.StickyDynamite, ProjectileID.StickyGrenade, ProjectileID.HellfireArrow, ProjectileID.HappyBomb, ProjectileID.BombSkeletronPrime, ProjectileID.Explosives, ProjectileID.GrenadeI, ProjectileID.GrenadeII, ProjectileID.GrenadeIII, ProjectileID.GrenadeIV, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ProjectileID.ProximityMineI, ProjectileID.ProximityMineII, ProjectileID.ProximityMineIII, ProjectileID.ProximityMineIV, ProjectileID.Landmine, ProjectileID.Beenade, ProjectileID.ExplosiveBunny, ProjectileID.ExplosiveBullet, ProjectileID.RocketSkeleton, ProjectileID.JackOLantern, ProjectileID.OrnamentFriendly, ProjectileID.RocketSnowmanI, ProjectileID.RocketSnowmanII, ProjectileID.RocketSnowmanIII, ProjectileID.RocketSnowmanIV, ProjectileID.Missile, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerMissile, ProjectileID.SeedlerNut, ProjectileID.BouncyBomb, ProjectileID.BouncyDynamite, ProjectileID.BouncyGrenade, ProjectileID.PartyGirlGrenade, ProjectileID.SolarWhipSwordExplosion, ProjectileID.VortexBeaterRocket, ProjectileID.LunarFlare, ProjectileID.DD2GoblinBomb, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.ScarabBomb, ProjectileID.ClusterRocketI, ProjectileID.ClusterRocketII, ProjectileID.ClusterGrenadeI, ProjectileID.ClusterGrenadeII, ProjectileID.ClusterMineI, ProjectileID.ClusterMineII, ProjectileID.MiniNukeRocketI, ProjectileID.MiniNukeRocketII, ProjectileID.MiniNukeGrenadeI, ProjectileID.MiniNukeGrenadeII, ProjectileID.MiniNukeMineI, ProjectileID.MiniNukeMineII, ProjectileID.ClusterSnowmanRocketI, ProjectileID.ClusterSnowmanRocketII, ProjectileID.MiniNukeSnowmanRocketI, ProjectileID.MiniNukeSnowmanRocketII, ProjectileID.SantankMountRocket, ProjectileID.DaybreakExplosion, ModContent.ProjectileType<AcornBomb_Proj>(), ModContent.ProjectileType<AcornBomb_Proj>(), ModContent.ProjectileType<Hardlight_SlayerMissile>(), ModContent.ProjectileType<PlasmaRound_Blast>(), ModContent.ProjectileType<EggBomb_Proj>(), ModContent.ProjectileType<HallowedHandGrenade_Proj>(), ModContent.ProjectileType<BlastBattery_MissileBlast>(), ModContent.ProjectileType<Electronade_Proj>(), ModContent.ProjectileType<Hardlight_SoSMissile>(), ModContent.ProjectileType<Hardlight_MissileBlast>(), ModContent.ProjectileType<KS3_Fist>(), ModContent.ProjectileType<KS3_SoSMissile>(), ModContent.ProjectileType<SlayerMissile>(), ModContent.ProjectileType<OO_MissileBlast>(), ModContent.ProjectileType<OO_BarrageMissile>(), ModContent.ProjectileType<Android_Proj>(), ModContent.ProjectileType<FlakGrenade>(), ModContent.ProjectileType<FlakGrenade_Bouncy>(), ModContent.ProjectileType<FlakGrenade_Sticky>(), ModContent.ProjectileType<DAN_Rocket>(), ModContent.ProjectileType<XeniumBubble_Proj>(), ModContent.ProjectileType<Uranium_Proj>(), ModContent.ProjectileType<BlastBattery_Missile>(), ModContent.ProjectileType<PlutoniumNuke_Proj>() };

        #endregion
    }
    public static class ItemLists
    {
        #region Item Lists

        public static List<int> BluntSwing = new()
        { ItemID.BreathingReed, ItemID.ZombieArm, ItemID.PurpleClubberfish, ItemID.TaxCollectorsStickOfDoom, ItemID.SlapHand, ItemID.Keybrand, ItemID.HamBat, ItemID.BatBat, ItemID.StaffofRegrowth };

        public static List<int> Arcane = new()
        { ItemID.EnchantedSword, ItemID.SpectrePickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe, ItemID.SpectreHamaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeStardust };

        public static List<int> Fire = new()
        { ItemID.FieryGreatsword, ItemID.TheHorsemansBlade, ItemID.DD2SquireBetsySword, ItemID.MoltenPickaxe, ItemID.SolarFlarePickaxe, ItemID.MeteorHamaxe, ItemID.MoltenHamaxe, ItemID.LunarHamaxeSolar, ModContent.ItemType<DragonCleaver>(), ModContent.ItemType<Firebreak>() };

        public static List<int> Water = new()
        { ItemID.Muramasa };

        public static List<int> Ice = new()
        { ItemID.IceBlade, ItemID.IceSickle, ItemID.Frostbrand, ModContent.ItemType<PureIronSword>(), ModContent.ItemType<BladeOfTheMountain>() };

        public static List<int> Earth = new()
        { ItemID.Seedler, ItemID.FossilPickaxe, ItemID.Picksaw };

        public static List<int> Wind = new()
        { };

        public static List<int> Thunder = new()
        { ItemID.InfluxWaver, ItemID.VortexPickaxe, ItemID.LunarHamaxeVortex, ModContent.ItemType<SlayerGun>(), ModContent.ItemType<XeniumLance>() };

        public static List<int> Holy = new()
        { ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.PickaxeAxe, ItemID.Pwnhammer, ModContent.ItemType<Bindeklinge>(), ModContent.ItemType<BlindJustice>() };

        public static List<int> Shadow = new()
        { ItemID.LightsBane, ItemID.PurpleClubberfish, ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.DeathSickle, ItemID.NightmarePickaxe, ItemID.WarAxeoftheNight, ItemID.TheBreaker };

        public static List<int> Nature = new()
        { ItemID.CactusSword, ItemID.BladeofGrass, ItemID.Seedler, ItemID.ChlorophyteSaber, ItemID.ChristmasTreeSword, ItemID.ChlorophyteClaymore, ItemID.TerraBlade, ItemID.CactusPickaxe, ItemID.ChlorophytePickaxe, ItemID.ChlorophyteGreataxe, ItemID.Hammush, ItemID.ChlorophyteWarhammer, ModContent.ItemType<CursedGrassBlade>() };

        public static List<int> Poison = new()
        { ItemID.BeeKeeper, ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<SwarmerCannon>() };

        public static List<int> Blood = new()
        { ItemID.BloodButcherer, ItemID.Bladetongue, ItemID.DeathbringerPickaxe, ItemID.BloodLustCluster, ItemID.BloodHamaxe, ItemID.FleshGrinder, ItemID.PsychoKnife };

        public static List<int> Psychic = new()
        { };

        public static List<int> Celestial = new()
        { ItemID.Starfury, ItemID.PiercingStarlight, ItemID.StarWrath, ItemID.Meowmere, ItemID.SolarFlarePickaxe, ItemID.NebulaPickaxe, ItemID.VortexPickaxe, ItemID.StardustPickaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeSolar, ItemID.LunarHamaxeStardust, ItemID.LunarHamaxeVortex };

        public static List<int> NoElement = new()
        { ItemID.BlueSolution, ItemID.DarkBlueSolution, ItemID.GreenSolution, ItemID.PurpleSolution, ItemID.RedSolution, ItemID.RocketI, ItemID.RocketII, ItemID.RocketIII, ItemID.RocketIV, ModContent.ItemType<BleachedSolution>() };

        #endregion
    }
}
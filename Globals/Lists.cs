using System.Collections.Generic;
using Redemption.NPCs.Critters;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.Projectiles.Misc;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.Tiles.Tiles;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Wasteland;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Volt;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.Items.Usable;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.Tiles.Furniture.ElderWood;
using Redemption.Tiles.Furniture.PetrifiedWood;
using Redemption.Tiles.Furniture.Lab;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.NPCs.HM;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Friendly.SpiritSummons;
using Redemption.NPCs.FowlMorning;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.Tiles.Furniture.Misc;

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
            TileID.CrackedBlueDungeonBrick,
            TileID.CrackedGreenDungeonBrick,
            TileID.CrackedPinkDungeonBrick,
            TileID.LihzahrdBrick,
            TileID.BeeHive,
            TileID.Granite,
            TileID.Marble,
            ModContent.TileType<AncientHallBrickTile>(),
            ModContent.TileType<SlayerShipPanelTile>(),
            ModContent.TileType<LabPlatingTileUnsafe>(),
            ModContent.TileType<HangingTiedTile>()
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

        public static List<int> DisablesSpawnsWhenNear = new() { ModContent.NPCType<Calavia_Intro>(), ModContent.NPCType<Calavia_NPC>() };

        public static List<int> HasLostSoul = new() { ModContent.NPCType<LostSoulNPC>(), ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<VagrantSpirit>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<RaveyardSkeleton>(), 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635 };

        #region Skeleton
        public static List<int> Skeleton = new() { 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635, NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle, NPCID.BoneSerpentHead, NPCID.BoneSerpentBody, NPCID.BoneSerpentTail, NPCID.DarkCaster, NPCID.CursedSkull, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DiabolistRed, NPCID.DiabolistWhite, NPCID.GiantCursedSkull, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.DungeonGuardian, NPCID.SkeletronHead, NPCID.SkeletronHand, NPCID.PirateGhost, ModContent.NPCType<BoneSpider>(), ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<RaveyardSkeleton>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<EpidotrianSkeleton_SS>(), ModContent.NPCType<SkeletonAssassin_SS>(), ModContent.NPCType<SkeletonFlagbearer_SS>(), ModContent.NPCType<SkeletonNoble_SS>(), ModContent.NPCType<SkeletonWarden_SS>(), ModContent.NPCType<SkeletonDuelist_SS>(), ModContent.NPCType<SkeletonWanderer_SS>(), ModContent.NPCType<Asher_SS>() };


        public static List<int> SkeletonHumanoid = new() { 77, 449, 450, 451, 452, 481, 201, 202, 203, 21, 324, 110, 323, 293, 291, 322, 292, 197, 167, 44, 635, NPCID.DD2SkeletonT1, NPCID.DD2SkeletonT3, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle, NPCID.DarkCaster, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.BoneLee, NPCID.DiabolistRed, NPCID.DiabolistWhite, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.RuneWizard, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, NPCID.PirateGhost, ModContent.NPCType<EpidotrianSkeleton>(), ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonFlagbearer>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<SkeletonWarden>(), ModContent.NPCType<RaveyardSkeleton>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<EpidotrianSkeleton_SS>(), ModContent.NPCType<SkeletonAssassin_SS>(), ModContent.NPCType<SkeletonFlagbearer_SS>(), ModContent.NPCType<SkeletonNoble_SS>(), ModContent.NPCType<SkeletonWarden_SS>(), ModContent.NPCType<SkeletonDuelist_SS>(), ModContent.NPCType<SkeletonWanderer_SS>(), ModContent.NPCType<Asher_SS>() };
        #endregion

        public static List<int> Humanoid = new() { NPCID.Demon, NPCID.VoodooDemon, NPCID.DoctorBones, NPCID.FaceMonster, NPCID.ArmedZombieEskimo, NPCID.ZombieEskimo, NPCID.FireImp, NPCID.Gnome, NPCID.GoblinArcher, NPCID.GoblinPeon, NPCID.GoblinScout, NPCID.GoblinSorcerer, NPCID.GoblinSummoner, NPCID.GoblinThief, NPCID.GoblinWarrior, NPCID.GraniteGolem, NPCID.Harpy, NPCID.MaggotZombie, NPCID.Nymph, NPCID.Salamander, NPCID.Salamander2, NPCID.Salamander3, NPCID.Salamander4, NPCID.Salamander5, NPCID.Salamander6, NPCID.Salamander7, NPCID.Salamander8, NPCID.Salamander9, NPCID.ZombieMushroom, NPCID.ZombieMushroomHat, NPCID.Zombie, NPCID.ZombieDoctor, NPCID.ZombieElf, NPCID.ZombieElfBeard, NPCID.ZombieElfGirl, NPCID.ZombieMerman, NPCID.ZombiePixie, NPCID.ZombieRaincoat, NPCID.ZombieSuperman, NPCID.ZombieSweater, NPCID.ZombieXmas, NPCID.ArmedTorchZombie, NPCID.ArmedZombie, NPCID.ArmedZombieCenx, NPCID.ArmedZombiePincussion, NPCID.ArmedZombieSlimed, NPCID.ArmedZombieSwamp, NPCID.ArmedZombieTwiggy, NPCID.BaldZombie, NPCID.BloodZombie, NPCID.FemaleZombie, NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TorchZombie, NPCID.TwiggyZombie, NPCID.CultistArcherBlue, NPCID.BloodMummy, NPCID.ChaosElemental, NPCID.DarkMummy, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.DesertDjinn, NPCID.FloatyGross, NPCID.IcyMerman, NPCID.DesertLamiaDark, NPCID.DesertLamiaLight, NPCID.Medusa, NPCID.Mummy, NPCID.RedDevil, NPCID.PossessedArmor, NPCID.Paladin, NPCID.RockGolem, NPCID.DemonTaxCollector, NPCID.Werewolf, NPCID.Clown, NPCID.CorruptPenguin, NPCID.CrimsonPenguin, NPCID.GoblinShark, NPCID.TheGroom, NPCID.TheBride, NPCID.IceGolem, NPCID.SandElemental, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.DD2GoblinBomberT1, NPCID.DD2GoblinBomberT2, NPCID.DD2GoblinBomberT3, NPCID.DD2GoblinT1, NPCID.DD2GoblinT2, NPCID.DD2GoblinT3, NPCID.DD2JavelinstT1, NPCID.DD2JavelinstT2, NPCID.DD2JavelinstT3, NPCID.DD2KoboldWalkerT2, NPCID.DD2KoboldWalkerT3, NPCID.SnowmanGangsta, NPCID.SnowBalla, NPCID.MisterStabby, NPCID.PirateCaptain, NPCID.PirateCorsair, NPCID.PirateCrossbower, NPCID.PirateDeadeye, NPCID.PirateDeckhand, NPCID.Butcher, NPCID.CreatureFromTheDeep, NPCID.DrManFly, NPCID.Eyezor, NPCID.Frankenstein, NPCID.Fritz, NPCID.Nailhead, NPCID.Psycho, NPCID.SwampThing, NPCID.ThePossessed, NPCID.Vampire, NPCID.BrainScrambler, NPCID.GigaZapper, NPCID.GrayGrunt, NPCID.MartianEngineer, NPCID.MartianOfficer, NPCID.RayGunner, NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10, NPCID.Splinterling, NPCID.GingerbreadMan, NPCID.Krampus, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.Yeti, NPCID.NebulaSoldier, NPCID.SolarSpearman, NPCID.SolarSolenian, NPCID.StardustSoldier, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.CultistBoss, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.Lihzahrd, NPCID.LihzahrdCrawler, ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<ForestNymph>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<LivingBloom>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<SicklyPenguin>() };

        #region Undead

        public static List<int> Undead = new() { 3, 132, 186, 187, 188, 189, 200, 223, 161, 254, 255, 52, 53, 536, 319, 320, 321, 332, 436, 431, 432, 433, 434, 435, 331, 430, 590, NPCID.TorchZombie, NPCID.ArmedTorchZombie, NPCID.MaggotZombie, NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.BloodZombie, NPCID.ZombieMerman, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.Eyezor, NPCID.Frankenstein, NPCID.Vampire, NPCID.VampireBat, NPCID.HeadlessHorseman, NPCID.ZombieElf, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, NPCID.HeadlessHorseman, ModContent.NPCType<JollyMadman>(), ModContent.NPCType<Keeper>(), ModContent.NPCType<SkullDigger>(), ModContent.NPCType<Fallen>(), ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<IrradiatedBehemoth>(), ModContent.NPCType<HazmatZombie_SS>() };

        #endregion

        #region Spirit

        public static List<int> Spirit = new() { NPCID.EnchantedSword, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.DesertDjinn, NPCID.DungeonSpirit, NPCID.FloatyGross, NPCID.Ghost, NPCID.PossessedArmor, NPCID.Wraith, NPCID.Reaper, NPCID.Poltergeist, NPCID.PirateGhost, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<VagrantSpirit>(), ModContent.NPCType<KeeperSpirit>(), ModContent.NPCType<ErhanSpirit>(), ModContent.NPCType<LostSoulNPC>(), ModContent.NPCType<NuclearShadow>(), ModContent.NPCType<WraithSlayer_Samurai>(), ModContent.NPCType<EpidotrianSkeleton_SS>(), ModContent.NPCType<ForestNymph_SS>(), ModContent.NPCType<SkeletonAssassin_SS>(), ModContent.NPCType<SkeletonFlagbearer_SS>(), ModContent.NPCType<SkeletonNoble_SS>(), ModContent.NPCType<SkeletonWarden_SS>(), ModContent.NPCType<HazmatZombie_SS>(), ModContent.NPCType<SkeletonDuelist_SS>(), ModContent.NPCType<SkeletonWanderer_SS>(), ModContent.NPCType<Asher_SS>(), ModContent.NPCType<AncientGladestoneGolem_SS>(), ModContent.NPCType<MossyGoliath_SS>(), ModContent.NPCType<HeadlessChicken>(), ModContent.NPCType<GhostfireChicken>() };

        #endregion

        #region Plantlike

        public static List<int> Plantlike = new() { NPCID.FungiBulb, NPCID.AnomuraFungus, NPCID.MushiLadybug, NPCID.ManEater, NPCID.Snatcher, NPCID.AngryTrapper, NPCID.FungoFish, NPCID.GiantFungiBulb, NPCID.HoppinJack, NPCID.Dandelion, NPCID.Plantera, NPCID.MourningWood, NPCID.Splinterling, NPCID.Pumpking, NPCID.Everscream, NPCID.PlanterasTentacle, ModContent.NPCType<LivingBloom>(), ModContent.NPCType<DevilsTongue>(), ModContent.NPCType<Thorn>(), ModContent.NPCType<TreebarkDryad>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<ForestNymph>(), ModContent.NPCType<Akka>(), ModContent.NPCType<ForestNymph_Friendly>(), ModContent.NPCType<ForestNymph_SS>() };

        #endregion

        #region Demon
        public static List<int> Demon = new() { NPCID.Demon, NPCID.VoodooDemon, NPCID.FireImp, NPCID.RedDevil, NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.Krampus };
        #endregion

        #region Cold
        public static List<int> Cold = new() { NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.IceBat, NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.SnowFlinx, NPCID.IceElemental, NPCID.IceMimic, NPCID.IceTortoise, NPCID.IcyMerman, NPCID.MisterStabby, NPCID.Wolf, NPCID.IceGolem, NPCID.SnowBalla, NPCID.SnowmanGangsta, NPCID.Flocko, NPCID.Yeti, NPCID.IceQueen, NPCID.Deerclops, ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>() };
        #endregion

        #region Hot
        public static List<int> Hot = new() { NPCID.Antlion, NPCID.FlyingAntlion, NPCID.GiantFlyingAntlion, NPCID.GiantWalkingAntlion, NPCID.LarvaeAntlion, NPCID.WalkingAntlion, NPCID.FireImp, NPCID.Hellbat, NPCID.LavaSlime, NPCID.MeteorHead, NPCID.SandSlime, NPCID.TombCrawlerHead, NPCID.TombCrawlerBody, NPCID.TombCrawlerTail, NPCID.Vulture, NPCID.DesertBeast, NPCID.DuneSplicerHead, NPCID.DuneSplicerBody, NPCID.DuneSplicerTail, NPCID.DesertGhoul, NPCID.DesertGhoulCorruption, NPCID.DesertGhoulCrimson, NPCID.DesertGhoulHallow, NPCID.DesertDjinn, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.HoppinJack, NPCID.Lavabat, NPCID.DesertLamiaDark, NPCID.DesertLamiaLight, NPCID.Mummy, NPCID.BloodMummy, NPCID.DarkMummy, NPCID.LightMummy, NPCID.Tumbleweed, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.SandElemental, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.DD2Betsy, NPCID.Pumpking, NPCID.MourningWood, NPCID.LunarTowerSolar, ModContent.NPCType<CorpseWalkerPriest>(), ModContent.NPCType<DevilsTongue>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<BloatedSwarmer>() };
        #endregion

        #region Wet
        public static List<int> Wet = new() { NPCID.BlueJellyfish, NPCID.PinkJellyfish, NPCID.Piranha, NPCID.Shark, NPCID.SeaSnail, NPCID.Squid, NPCID.AnglerFish, NPCID.Arapaima, NPCID.BloodJelly, NPCID.BloodFeeder, NPCID.FungoFish, NPCID.FloatyGross, NPCID.GreenJellyfish, NPCID.IcyMerman, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.BloodSquid, NPCID.GoblinShark, NPCID.Drippler, NPCID.BloodZombie, NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.ZombieMerman, NPCID.FlyingFish, NPCID.AngryNimbus, NPCID.CreatureFromTheDeep, NPCID.SwampThing, NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2, ModContent.NPCType<BlisteredFish>(), ModContent.NPCType<BlisteredFish2>(), ModContent.NPCType<Blisterface>(), ModContent.NPCType<BloatedGoldfish>(), ModContent.NPCType<RadioactiveJelly>() };
        #endregion

        #region Dragonlike
        public static List<int> Dragonlike = new() { NPCID.DD2Betsy, NPCID.DD2WyvernT1, NPCID.DD2WyvernT2, NPCID.DD2WyvernT3, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.DukeFishron, NPCID.WyvernHead, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernLegs, NPCID.WyvernTail, NPCID.CultistDragonHead, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonTail, ModContent.NPCType<Cockatrice>() };
        #endregion

        #region Inorganic
        public static List<int> Inorganic = new() { NPCID.GraniteFlyer, NPCID.GraniteGolem, NPCID.MeteorHead, NPCID.Mimic, NPCID.BigMimicCorruption, NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle, NPCID.IceMimic, NPCID.PresentMimic, NPCID.CrimsonAxe, NPCID.CursedHammer, NPCID.EnchantedSword, NPCID.IceElemental, NPCID.MartianProbe, NPCID.PossessedArmor, NPCID.Pixie, NPCID.Paladin, NPCID.RockGolem, NPCID.ChatteringTeethBomb, NPCID.AngryNimbus, NPCID.IceGolem, NPCID.Tumbleweed, NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.SnowBalla, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Flocko, NPCID.GingerbreadMan, NPCID.Nutcracker, NPCID.NutcrackerSpinning, NPCID.SolarCorite, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.PirateShipCannon, NPCID.IceQueen, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<EaglecrestGolem>(), ModContent.NPCType<EaglecrestGolem_Sleep>(), ModContent.NPCType<EaglecrestRockPile>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>(), ModContent.NPCType<OO>(), ModContent.NPCType<Android>(), ModContent.NPCType<PrototypeSilver>(), ModContent.NPCType<SpacePaladin>(), ModContent.NPCType<Akka>(), ModContent.NPCType<Ukko>(), ModContent.NPCType<EaglecrestGolem2>(), ModContent.NPCType<EaglecrestRockPile2>(), ModContent.NPCType<TreebarkDryad>() };
        #endregion

        #region Robotic
        public static List<int> Robotic = new() { NPCID.MartianProbe, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Spazmatism, NPCID.Retinazer, NPCID.SantaNK1, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.Probe, ModContent.NPCType<AncientGladestoneGolem>(), ModContent.NPCType<KS3>(), ModContent.NPCType<KS3_Clone>(), ModContent.NPCType<KS3_Magnet>(), ModContent.NPCType<KS3_MissileDrone>(), ModContent.NPCType<KS3_ScannerDrone>(), ModContent.NPCType<SpaceKeeper>(), ModContent.NPCType<Wielder>(), ModContent.NPCType<OmegaCleaver>(), ModContent.NPCType<JanitorBot>(), ModContent.NPCType<ProtectorVolt>(), ModContent.NPCType<MACEProject>(), ModContent.NPCType<Gigapora>(), ModContent.NPCType<Gigapora_BodySegment>(), ModContent.NPCType<Gigapora_ShieldCore>(), ModContent.NPCType<OO>(), ModContent.NPCType<Android>(), ModContent.NPCType<PrototypeSilver>(), ModContent.NPCType<SpacePaladin>(), ModContent.NPCType<TBot>(), ModContent.NPCType<CraneOperator>() };
        #endregion

        #region Infected
        public static List<int> Infected = new() { ModContent.NPCType<BlisteredScientist>(), ModContent.NPCType<BloatedScientist>(), ModContent.NPCType<OozingScientist>(), ModContent.NPCType<OozeBlob>(), ModContent.NPCType<SeedGrowth>(), ModContent.NPCType<SoI>(), ModContent.NPCType<HazmatZombie>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<RadioactiveSlime>(), ModContent.NPCType<NuclearSlime>(), ModContent.NPCType<IrradiatedBehemoth>(), ModContent.NPCType<Blisterface>(), ModContent.NPCType<BlisteredFish>(), ModContent.NPCType<BlisteredFish2>(), ModContent.NPCType<SickenedDemonEye>(), ModContent.NPCType<SickenedBunny>(), ModContent.NPCType<MutatedLivingBloom>(), ModContent.NPCType<SneezyFlinx>(), ModContent.NPCType<SicklyPenguin>(), ModContent.NPCType<SicklyWolf>(), ModContent.NPCType<PZ>(), ModContent.NPCType<PZ_Kari>(), ModContent.NPCType<BloatedGhoul>(), ModContent.NPCType<BloatedGoldfish>(), ModContent.NPCType<RadioactiveJelly>(), ModContent.NPCType<BloatedSwarmer>(), ModContent.NPCType<BloatedGrub>() };
        #endregion

        #region Armed
        public static List<int> Armed = new() { NPCID.RedDevil, NPCID.Paladin, NPCID.GoblinThief, NPCID.DD2GoblinT1, NPCID.DD2GoblinT2, NPCID.DD2GoblinT3, NPCID.MisterStabby, NPCID.PirateCorsair, NPCID.PirateGhost, NPCID.Butcher, NPCID.Psycho, NPCID.Reaper, NPCID.SolarDrakomireRider, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.DD2OgreT2, NPCID.DD2OgreT3, NPCID.Pumpking, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesSword, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBones, NPCID.HellArmoredBonesSword, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor, ModContent.NPCType<SkullDigger>(), ModContent.NPCType<JollyMadman>(), ModContent.NPCType<SkeletonAssassin>(), ModContent.NPCType<SkeletonDuelist>(), ModContent.NPCType<SkeletonNoble>(), ModContent.NPCType<SkeletonWanderer>(), ModContent.NPCType<WraithSlayer_Samurai>(), ModContent.NPCType<SpacePaladin>(), ModContent.NPCType<Calavia>() };
        #endregion

        #region Hallowed
        public static List<int> Hallowed = new() { NPCID.ChaosElemental, NPCID.DesertGhoulHallow, NPCID.EnchantedSword, NPCID.BigMimicHallow, NPCID.IlluminantBat, NPCID.IlluminantSlime, NPCID.LightMummy, NPCID.Pixie, NPCID.Paladin, NPCID.Unicorn, NPCID.RainbowSlime, NPCID.SandsharkHallow, NPCID.HallowBoss, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, ModContent.NPCType<Erhan>(), ModContent.NPCType<ErhanSpirit>() };
        #endregion

        #region Dark
        public static List<int> Dark = new() { NPCID.DarkCaster, NPCID.DungeonSlime, NPCID.EaterofSouls, NPCID.DevourerBody, NPCID.DevourerHead, NPCID.DevourerTail, NPCID.Clinger, NPCID.BigMimicCorruption, NPCID.CorruptSlime, NPCID.Corruptor, NPCID.CursedHammer, NPCID.DarkMummy, NPCID.DesertDjinn, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.PossessedArmor, NPCID.DesertGhoulCorruption, NPCID.Slimer, NPCID.Wraith, NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail, NPCID.CorruptBunny, NPCID.CorruptGoldfish, NPCID.CorruptPenguin, NPCID.SandsharkCorrupt, NPCID.GoblinSummoner, NPCID.ShadowFlameApparition, NPCID.Reaper, NPCID.ThePossessed, NPCID.Vampire, NPCID.Hellhound, NPCID.HeadlessHorseman, NPCID.Splinterling, NPCID.Krampus, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.MourningWood, NPCID.Pumpking };
        #endregion

        #region Blood
        public static List<int> Blood = new() { NPCID.BloodCrawler, NPCID.BloodCrawlerWall, NPCID.Crimera, NPCID.EyeballFlyingFish, NPCID.CataractEye, NPCID.DemonEye, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.WanderingEye, NPCID.FaceMonster, NPCID.BloodMummy, NPCID.BloodJelly, NPCID.BloodFeeder, NPCID.Crimslime, NPCID.CrimsonAxe, NPCID.BigMimicCrimson, NPCID.FloatyGross, NPCID.Herpling, NPCID.IchorSticker, NPCID.DesertGhoulCrimson, NPCID.BloodEelBody, NPCID.BloodEelHead, NPCID.BloodEelTail, NPCID.BloodSquid, NPCID.BloodZombie, NPCID.BloodNautilus, NPCID.Drippler, NPCID.GoblinShark, NPCID.CrimsonBunny, NPCID.CrimsonGoldfish, NPCID.CrimsonPenguin, NPCID.SandsharkCrimson, NPCID.Vampire, NPCID.BrainofCthulhu, NPCID.EyeofCthulhu, NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.Creeper, NPCID.LeechBody, NPCID.LeechHead, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII, NPCID.ServantofCthulhu };
        #endregion

        public static List<int> IsSlime = new() { NPCID.BlueSlime, NPCID.IceSlime, NPCID.SandSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, NPCID.MotherSlime, NPCID.LavaSlime, NPCID.DungeonSlime, NPCID.GoldenSlime, NPCID.KingSlime, NPCID.SlimeSpiked, NPCID.UmbrellaSlime, NPCID.SlimeMasked, NPCID.SlimeRibbonGreen, NPCID.SlimeRibbonRed, NPCID.SlimeRibbonWhite, NPCID.SlimeRibbonYellow, NPCID.ToxicSludge, NPCID.CorruptSlime, NPCID.Slimer, NPCID.Crimslime, NPCID.Gastropod, NPCID.IlluminantSlime, NPCID.RainbowSlime, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, NPCID.HoppinJack, ModContent.NPCType<Blobble>(), ModContent.NPCType<SeedGrowth>(), ModContent.NPCType<OozeBlob>(), ModContent.NPCType<BobTheBlob>(), ModContent.NPCType<RadioactiveSlime>(), ModContent.NPCType<NuclearSlime>(), ModContent.NPCType<IrradiatedBehemoth>() };

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

        public static List<int> NoElement = new() { ProjectileID.CorruptSpray, ProjectileID.CrimsonSpray, ProjectileID.HallowSpray, ProjectileID.MushroomSpray, ProjectileID.PureSpray, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ModContent.ProjectileType<BleachedSolution_Proj>() };
        #endregion
    }
    public static class ItemLists
    {
        #region Item Lists

        public static List<int> BluntSwing = new()
        { ItemID.BreathingReed, ItemID.ZombieArm, ItemID.PurpleClubberfish, ItemID.TaxCollectorsStickOfDoom, ItemID.SlapHand, ItemID.Keybrand, ItemID.HamBat, ItemID.BatBat, ItemID.StaffofRegrowth };

        public static List<int> NoElement = new()
        { ItemID.BlueSolution, ItemID.DarkBlueSolution, ItemID.GreenSolution, ItemID.PurpleSolution, ItemID.RedSolution, ItemID.RocketI, ItemID.RocketII, ItemID.RocketIII, ItemID.RocketIV, ModContent.ItemType<BleachedSolution>() };

        public static List<int> AlignmentInterest = new()
        { ModContent.ItemType<HeartOfThorns>(), ModContent.ItemType<DemonScroll>(), ModContent.ItemType<WeddingRing>(), ModContent.ItemType<SorrowfulEssence>(), ModContent.ItemType<AbandonedTeddy>(), ModContent.ItemType<CyberTech>(), ModContent.ItemType<SlayerShipEngine>(), ModContent.ItemType<MemoryChip>(), ModContent.ItemType<AnglonicMysticBlossom>(), ModContent.ItemType<KingsOakStaff>(), ModContent.ItemType<NebSummon>() };

        #endregion
    }
}
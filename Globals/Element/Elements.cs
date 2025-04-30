using Redemption.BaseExtension;
using Redemption.Buffs;
using Redemption.Textures.Elements;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public static class ElementID
    {
        #region Element Bools
        // Arcane
        public static bool[] ProjArcane = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.EnchantedBoomerang, ProjectileID.Starfury, ProjectileID.MagicMissile, ProjectileID.EighthNote, ProjectileID.QuarterNote, ProjectileID.TiedEighthNote, ProjectileID.RainbowRodBullet, ProjectileID.MagicDagger, ProjectileID.CrystalStorm, ProjectileID.SwordBeam, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt, ProjectileID.RuneBlast, ProjectileID.TerraBeam, ProjectileID.LightBeam, ProjectileID.NightBeam, ProjectileID.EnchantedBeam, ProjectileID.FrostBeam, ProjectileID.EyeBeam, ProjectileID.Skull, ProjectileID.DeathSickle, ProjectileID.LostSoulFriendly, ProjectileID.LostSoulHostile, ProjectileID.Shadowflames, ProjectileID.VampireKnife, ProjectileID.SpectreWrath, ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardShaft, ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.MedusaHeadRay, ProjectileID.MedusaHead, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaLaser, ProjectileID.ClothiersCurse, ProjectileID.TerrarianBeam, ProjectileID.Terrarian, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.StardustGuardianExplosion, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.PhantasmArrow, ProjectileID.LastPrismLaser, ProjectileID.LastPrism, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurret, ProjectileID.LunarFlare, ProjectileID.SkyFracture, ProjectileID.DD2DarkMageBolt, ProjectileID.BookOfSkullsSkull, ProjectileID.SparkleGuitar, ProjectileID.TitaniumStormShard, ProjectileID.StardustPunch, ProjectileID.NebulaDrill, ProjectileID.StardustDrill, ProjectileID.JestersArrow, ProjectileID.MonkStaffT2Ghast, ProjectileID.AbigailMinion, ProjectileID.AbigailCounter, ProjectileID.Trimarang, ProjectileID.WandOfFrostingFrost, ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.BallofFire, ProjectileID.WaterBolt, ProjectileID.Flamelash, ProjectileID.DemonSickle, ProjectileID.DemonScythe, ProjectileID.IceBlock, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.FrostBlastHostile, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.CrystalLeafShot, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.BallofFrost, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.GoldenShowerFriendly, ProjectileID.ShadowBeamHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.PaladinsHammerHostile, ProjectileID.PaladinsHammerFriendly, ProjectileID.FlamingScythe, ProjectileID.Blizzard, ProjectileID.FrostBoltStaff, ProjectileID.ImpFireball, ProjectileID.Typhoon, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.CultistBossIceMist, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.CultistBossFireBall, ProjectileID.CultistBossFireBallClone, ProjectileID.ShadowFlame, ProjectileID.StarWrath, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.SpiritFlame, ProjectileID.RainbowCrystalExplosion, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.DD2ApprenticeStorm, ProjectileID.BookStaffShot, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.BatOfLight, ProjectileID.SharpTears, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FairyQueenRangedItemShot, ProjectileID.PrincessWeapon, ProjectileID.WandOfSparkingSpark, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly);

        public static bool[] ItemArcane = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.EnchantedSword, ItemID.SpectrePickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe, ItemID.SpectreHamaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeStardust, ItemID.MonkStaffT2, ItemID.OpticStaff, ItemID.Phantasm);

        public static bool[] NPCArcane = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.CursedSkull, NPCID.GiantCursedSkull, NPCID.GraniteFlyer, NPCID.Tim, NPCID.ChaosBallTim, NPCID.ChaosBall, NPCID.WaterSphere, NPCID.ChaosElemental, NPCID.CursedHammer, NPCID.CrimsonAxe, NPCID.EnchantedSword, NPCID.DungeonSpirit, NPCID.ShadowFlameApparition, NPCID.PirateGhost, NPCID.Poltergeist, NPCID.NebulaBeast, NPCID.NebulaBrain, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier, NPCID.StardustCellBig, NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustJellyfishSmall, NPCID.StardustSoldier, NPCID.StardustSpiderBig, NPCID.StardustSpiderSmall, NPCID.StardustWormBody, NPCID.StardustWormHead, NPCID.StardustWormTail, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonHead, NPCID.CultistDragonTail);
        // Fire
        public static bool[] ProjFire = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.FireArrow, ProjectileID.BallofFire, ProjectileID.Flamarang, ProjectileID.Flamelash, ProjectileID.Sunfury, ProjectileID.HellfireArrow, ProjectileID.FlamingArrow, ProjectileID.Flames, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.RuneBlast, ProjectileID.FrostburnArrow, ProjectileID.FlamethrowerTrap, ProjectileID.FlamesTrap, ProjectileID.Fireball, ProjectileID.HeatRay, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.FlamingWood, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.ImpFireball, ProjectileID.MolotovCocktail, ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.CultistBossFireBall, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.Hellwing, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.Spark, ProjectileID.HelFire, ProjectileID.ClothiersCurse, ProjectileID.DesertDjinnCurse, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.GeyserTrap, ProjectileID.SpiritFlame, ProjectileID.DD2FlameBurstTowerT1Shot, ProjectileID.DD2FlameBurstTowerT2Shot, ProjectileID.DD2FlameBurstTowerT3Shot, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.DD2ExplosiveTrapT1, ProjectileID.DD2ExplosiveTrapT2, ProjectileID.DD2ExplosiveTrapT3, ProjectileID.MonkStaffT2, ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2PhoenixBowShot, ProjectileID.DD2BetsyArrow, ProjectileID.ApprenticeStaffT3Shot, ProjectileID.FireWhipProj, ProjectileID.FireWhip, ProjectileID.FlamingMace, ProjectileID.TorchGod, ProjectileID.WandOfSparkingSpark, ProjectileID.SolarFlareDrill, ProjectileID.Flare, ProjectileID.BlueFlare, ProjectileID.FlyingImp, ProjectileID.Cascade, ProjectileID.DD2FlameBurstTowerT1, ProjectileID.DD2FlameBurstTowerT2, ProjectileID.DD2FlameBurstTowerT3, ProjectileID.DD2FlameBurstTowerT3, ProjectileID.Volcano, ProjectileID.TheHorsemansBlade, ProjectileID.HorsemanPumpkin, ProjectileID.CursedFlare, ProjectileID.RainbowFlare, ProjectileID.ShimmerFlare);

        public static bool[] ItemFire = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.FieryGreatsword, ItemID.TheHorsemansBlade, ItemID.DD2SquireBetsySword, ItemID.MoltenPickaxe, ItemID.SolarFlarePickaxe, ItemID.MeteorHamaxe, ItemID.MoltenHamaxe, ItemID.LunarHamaxeSolar, ItemID.DD2PhoenixBow, ItemID.BluePhaseblade, ItemID.BluePhasesaber, ItemID.GreenPhaseblade, ItemID.GreenPhasesaber, ItemID.OrangePhaseblade, ItemID.OrangePhasesaber, ItemID.PurplePhaseblade, ItemID.PurplePhasesaber, ItemID.RedPhaseblade, ItemID.RedPhasesaber, ItemID.WhitePhaseblade, ItemID.WhitePhasesaber, ItemID.YellowPhaseblade, ItemID.YellowPhasesaber, ItemID.TheHorsemansBlade);

        public static bool[] NPCFire = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.BlazingWheel, NPCID.FireImp, NPCID.Demon, NPCID.VoodooDemon, NPCID.Hellbat, NPCID.LavaSlime, NPCID.MeteorHead, NPCID.BurningSphere, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.HoppinJack, NPCID.Lavabat, NPCID.RedDevil, NPCID.RuneWizard, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarFlare, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.DD2Betsy);
        // Water
        public static bool[] ProjWater = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.BlueMoon, ProjectileID.HolyWater, ProjectileID.UnholyWater, ProjectileID.IcewaterSpit, ProjectileID.RainFriendly, ProjectileID.BloodRain, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.WaterGun, ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.FlaironBubble, ProjectileID.SlimeGun, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.Bubble, ProjectileID.Xenopopper, ProjectileID.ToxicBubble, ProjectileID.Kraken, ProjectileID.BloodWater, ProjectileID.Ale, ProjectileID.DD2OgreSpit, ProjectileID.QueenSlimeGelAttack, ProjectileID.GelBalloon, ProjectileID.VolatileGelatinBall, ProjectileID.Flairon, ProjectileID.MaceWhip, ProjectileID.Trident, ProjectileID.PainterPaintball, ProjectileID.MechanicalPiranha, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.BloodCloudMoving, ProjectileID.BloodCloudRaining, ProjectileID.Swordfish, ProjectileID.Muramasa, ProjectileID.FrostDaggerfish);

        public static bool[] ItemWater = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.Muramasa, ItemID.PurpleClubberfish, ItemID.BloodRainBow, ItemID.VolatileGelatin, ItemID.Rockfish);

        public static bool[] NPCWater = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.SlimedZombie, NPCID.SlimeMasked, NPCID.Slimer, NPCID.SlimeRibbonGreen, NPCID.SlimeRibbonRed, NPCID.SlimeRibbonWhite, NPCID.SlimeRibbonYellow, NPCID.SlimeSpiked, NPCID.ArmedZombieSlimed, NPCID.BlueSlime, NPCID.CorruptSlime, NPCID.DungeonSlime, NPCID.GoldenSlime, NPCID.IceSlime, NPCID.IlluminantSlime, NPCID.KingSlime, NPCID.MotherSlime, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, NPCID.RainbowSlime, NPCID.SandSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, NPCID.UmbrellaSlime, NPCID.Crimslime, NPCID.SeaSnail, NPCID.Shark, NPCID.BloodJelly, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.Squid, NPCID.WaterSphere, NPCID.Piranha, NPCID.AnglerFish, NPCID.Arapaima, NPCID.BloodFeeder, NPCID.FungoFish, NPCID.FloatyGross, NPCID.IcyMerman, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.BloodSquid, NPCID.GoblinShark, NPCID.Drippler, NPCID.BloodZombie, NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.ZombieMerman, NPCID.FlyingFish, NPCID.AngryNimbus, NPCID.CreatureFromTheDeep, NPCID.SwampThing, NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2);
        // Ice
        public static bool[] ProjIce = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.IceBlock, ProjectileID.IceBoomerang, ProjectileID.IceBolt, ProjectileID.FrostBoltSword, ProjectileID.FrostArrow, ProjectileID.FrostBlastHostile, ProjectileID.SnowBallFriendly, ProjectileID.FrostburnArrow, ProjectileID.IceSpike, ProjectileID.IcewaterSpit, ProjectileID.BallofFrost, ProjectileID.FrostBeam, ProjectileID.IceSickle, ProjectileID.FrostBlastFriendly, ProjectileID.Blizzard, ProjectileID.NorthPoleWeapon, ProjectileID.NorthPoleSpear, ProjectileID.NorthPoleSnowflake, ProjectileID.FrostWave, ProjectileID.FrostShard, ProjectileID.FrostBoltStaff, ProjectileID.CultistBossIceMist, ProjectileID.FrostDaggerfish, ProjectileID.Amarok, ProjectileID.CoolWhip, ProjectileID.CoolWhipProj, ProjectileID.DeerclopsIceSpike, ProjectileID.DeerclopsRangedProjectile, ProjectileID.FlinxMinion, ProjectileID.FrostHydra, ProjectileID.WandOfFrostingFrost);

        public static bool[] ItemIce = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.IceBlade, ItemID.IceSickle, ItemID.Frostbrand);

        public static bool[] NPCIce = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.IceBat, NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.IceElemental, NPCID.IceMimic, NPCID.IceTortoise, NPCID.IcyMerman, NPCID.IceGolem, NPCID.SnowBalla, NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.Flocko, NPCID.SnowFlinx, NPCID.Yeti, NPCID.Deerclops, NPCID.IceQueen);
        // Earth
        public static bool[] ProjEarth = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.Boulder, ProjectileID.BoulderStaffOfEarth, ProjectileID.GolemFist, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2OgreStomp, ProjectileID.DD2OgreSmash, ProjectileID.MonkStaffT1Explosion, ProjectileID.RollingCactus, ProjectileID.RockGolemRock, ProjectileID.BoneDagger, ProjectileID.BoneJavelin, ProjectileID.SandBallGun, ProjectileID.PearlSandBallGun, ProjectileID.CrimsandBallGun, ProjectileID.EbonsandBallGun, ProjectileID.SharpTears, ProjectileID.MonkStaffT1, ProjectileID.MoonBoulder, ProjectileID.BouncyBoulder, ProjectileID.MiniBoulder);

        public static bool[] ItemEarth = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.Seedler, ItemID.FossilPickaxe, ItemID.Picksaw, ItemID.AntlionClaw, ItemID.AcornAxe, ItemID.Rockfish);

        public static bool[] NPCEarth = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.GraniteFlyer, NPCID.GraniteGolem, NPCID.SandSlime, NPCID.DesertBeast, NPCID.GiantTortoise, NPCID.IceTortoise, NPCID.RockGolem, NPCID.DesertScorpionWalk, NPCID.DesertScorpionWall, NPCID.Tumbleweed, NPCID.SandElemental, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.Golem);
        // Wind
        public static bool[] ProjWind = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2ApprenticeStorm, ProjectileID.BookStaffShot, ProjectileID.WeatherPainShot, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.LightDisc, ProjectileID.FlyingKnife);

        public static bool[] ItemWind = ItemID.Sets.Factory.CreateBoolSet();

        public static bool[] NPCWind = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.Harpy, NPCID.Dandelion, NPCID.AngryNimbus, NPCID.Tumbleweed);
        // Thunder
        public static bool[] ProjThunder = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.RuneBlast, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.UFOLaser, ProjectileID.ScutlixLaser, ProjectileID.ScutlixLaserFriendly, ProjectileID.MartianTurretBolt, ProjectileID.BrainScramblerBolt, ProjectileID.GigaZapperSpear, ProjectileID.RayGunnerLaser, ProjectileID.LaserMachinegunLaser, ProjectileID.Electrosphere, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerDeathray, ProjectileID.SaucerLaser, ProjectileID.InfluxWaver, ProjectileID.ChargedBlasterLaser, ProjectileID.ChargedBlasterOrb, ProjectileID.PhantasmalBolt, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.DeadlySphere, ProjectileID.VortexVortexLightning, ProjectileID.VortexLightning, ProjectileID.MartianWalkerLaser, ProjectileID.VortexBeaterRocket, ProjectileID.DD2LightningBugZap, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.MonkStaffT3_AltShot, ProjectileID.ThunderSpear, ProjectileID.ThunderStaffShot, ProjectileID.ThunderSpearShot, ProjectileID.ZapinatorLaser, ProjectileID.VortexDrill, ProjectileID.InfluxWaver, ProjectileID.LaserMachinegun, ProjectileID.ChargedBlasterCannon, ProjectileID.UFOMinion, ProjectileID.MoonlordTurret, ProjectileID.MoonlordTurretLaser, ProjectileID.PulseBolt, ProjectileID.MiniRetinaLaser, ProjectileID.VortexLaser, ProjectileID.MinecartMechLaser, ProjectileID.GreenLaser, ProjectileID.DeathLaser, ProjectileID.EyeLaser, ProjectileID.PinkLaser, ProjectileID.PurpleLaser);

        public static bool[] ItemThunder = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.InfluxWaver, ItemID.VortexPickaxe, ItemID.LunarHamaxeVortex, ItemID.BrainScrambler, ItemID.PulseBow, ItemID.VortexBeater);

        public static bool[] NPCThunder = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.BloodJelly, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.DD2LightningBugT3, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.VortexHornet, NPCID.VortexHornetQueen, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret);
        // Holy
        public static bool[] ProjHoly = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.TheDaoofPow, ProjectileID.HolyWater, ProjectileID.HolyArrow, ProjectileID.HallowStar, ProjectileID.LightBeam, ProjectileID.Hamdrax, ProjectileID.PaladinsHammerHostile, ProjectileID.PaladinsHammerFriendly, ProjectileID.SkyFracture, ProjectileID.BatOfLight, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.HallowJoustingLance, ProjectileID.RainbowWhip, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.LightDisc, ProjectileID.SwordWhip, ProjectileID.Gungnir, ProjectileID.PearlSandBallGun, ProjectileID.Chik, ProjectileID.VolatileGelatinBall, ProjectileID.Excalibur, ProjectileID.TrueExcalibur);

        public static bool[] ItemHoly = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.PickaxeAxe, ItemID.Pwnhammer, ItemID.Keybrand, ItemID.VolatileGelatin);

        public static bool[] NPCHoly = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.ChaosElemental, NPCID.DesertGhoulHallow, NPCID.EnchantedSword, NPCID.BigMimicHallow, NPCID.IlluminantBat, NPCID.IlluminantSlime, NPCID.LightMummy, NPCID.Pixie, NPCID.Paladin, NPCID.Unicorn, NPCID.RainbowSlime, NPCID.SandsharkHallow, NPCID.HallowBoss, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple);
        // Shadow
        public static bool[] ProjShadow = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.UnholyArrow, ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.BallOHurt, ProjectileID.DemonSickle, ProjectileID.DemonScythe, ProjectileID.DarkLance, ProjectileID.TheDaoofPow, ProjectileID.UnholyWater, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.NightBeam, ProjectileID.DeathSickle, ProjectileID.ShadowBeamHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.Shadowflames, ProjectileID.EatersBite, ProjectileID.TinyEater, ProjectileID.CultistBossFireBallClone, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.CorruptYoyo, ProjectileID.ClothiersCurse, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.SpiritFlame, ProjectileID.BlackBolt, ProjectileID.DD2DrakinShot, ProjectileID.DD2DarkMageBolt, ProjectileID.ShadowJoustingLance, ProjectileID.ScytheWhipProj, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ProjectileID.EbonsandBallGun, ProjectileID.CrimsandBallGun, ProjectileID.BookOfSkullsSkull, ProjectileID.Bat, ProjectileID.Raven, ProjectileID.ScytheWhip, ProjectileID.HoundiusShootius, ProjectileID.HoundiusShootiusFireball, ProjectileID.NightsEdge, ProjectileID.TrueNightsEdge, ProjectileID.LightsBane, ProjectileID.CursedFlare);

        public static bool[] ItemShadow = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.LightsBane, ItemID.PurpleClubberfish, ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.DeathSickle, ItemID.NightmarePickaxe, ItemID.WarAxeoftheNight, ItemID.TheBreaker, ItemID.OnyxBlaster, ItemID.BoneHelm);

        public static bool[] NPCShadow = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.DarkCaster, NPCID.DungeonSlime, NPCID.EaterofSouls, NPCID.DevourerBody, NPCID.DevourerHead, NPCID.DevourerTail, NPCID.Clinger, NPCID.BigMimicCorruption, NPCID.CorruptSlime, NPCID.Corruptor, NPCID.CursedHammer, NPCID.DarkMummy, NPCID.DesertDjinn, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.PossessedArmor, NPCID.DesertGhoulCorruption, NPCID.Slimer, NPCID.Wraith, NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail, NPCID.CorruptBunny, NPCID.CorruptGoldfish, NPCID.CorruptPenguin, NPCID.SandsharkCorrupt, NPCID.GoblinSummoner, NPCID.ShadowFlameApparition, NPCID.Reaper, NPCID.ThePossessed, NPCID.Vampire, NPCID.Hellhound, NPCID.HeadlessHorseman, NPCID.Splinterling, NPCID.Krampus, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.MourningWood, NPCID.Pumpking, NPCID.Ghost, NPCID.MotherSlime, NPCID.Tim, NPCID.ChaosBallTim, NPCID.ChaosBall, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.ShadowFlameApparition, NPCID.GoblinSummoner, NPCID.Poltergeist);
        // Nature
        public static bool[] ProjNature = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.ThornChakram, ProjectileID.Seed, ProjectileID.Mushroom, ProjectileID.TerraBeam, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.JungleSpike, ProjectileID.Leaf, ProjectileID.FlowerPetal, ProjectileID.CrystalLeafShot, ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.FlowerPow, ProjectileID.FlowerPowPetal, ProjectileID.SeedPlantera, ProjectileID.PoisonSeedPlantera, ProjectileID.ThornBall, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.PineNeedleFriendly, ProjectileID.PineNeedleHostile, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn, ProjectileID.JungleYoyo, ProjectileID.SporeTrap, ProjectileID.SporeTrap2, ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.TruffleSpore, ProjectileID.Terrarian, ProjectileID.TerrarianBeam, ProjectileID.Terragrim, ProjectileID.DandelionSeed, ProjectileID.Shroomerang, ProjectileID.ThornWhip, ProjectileID.BabyBird, ProjectileID.MushroomSpear, ProjectileID.ChlorophyteArrow, ProjectileID.ChlorophyteBullet, ProjectileID.ChlorophyteChainsaw, ProjectileID.ChlorophyteJackhammer, ProjectileID.ChlorophyteDrill, ProjectileID.ChlorophytePartisan, ProjectileID.Bee, ProjectileID.BeeArrow, ProjectileID.GiantBee, ProjectileID.MechanicalPiranha, ProjectileID.Wasp, ProjectileID.Yelets, ProjectileID.BladeOfGrass, ProjectileID.TerraBlade2, ProjectileID.TerraBlade2Shot, ProjectileID.HiveFive, ProjectileID.Beenade);

        public static bool[] ItemNature = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.CactusSword, ItemID.BladeofGrass, ItemID.Seedler, ItemID.ChlorophyteSaber, ItemID.ChristmasTreeSword, ItemID.ChlorophyteClaymore, ItemID.TerraBlade, ItemID.CactusPickaxe, ItemID.ChlorophytePickaxe, ItemID.ChlorophyteGreataxe, ItemID.Hammush, ItemID.ChlorophyteWarhammer, ItemID.SporeSac, ItemID.AcornAxe, ItemID.BeeKeeper, ItemID.HoneyComb, ItemID.BeeCloak, ItemID.HoneyBalloon, ItemID.StingerNecklace, ItemID.SweetheartNecklace);

        public static bool[] NPCNature = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.AnomuraFungus, NPCID.GiantFungiBulb, NPCID.FungiBulb, NPCID.SpikedJungleSlime, NPCID.JungleBat, NPCID.ManEater, NPCID.JungleBat, NPCID.JungleBat, NPCID.JungleBat, NPCID.JungleBat, NPCID.MushiLadybug, NPCID.Snatcher, NPCID.SporeBat, NPCID.SporeSkeleton, NPCID.ZombieMushroom, NPCID.ZombieMushroomHat, NPCID.AngryTrapper, NPCID.JungleCreeper, NPCID.JungleCreeperWall, NPCID.Lihzahrd, NPCID.LihzahrdCrawler, NPCID.Moth, NPCID.Pixie, NPCID.Dandelion, NPCID.Plantera, NPCID.PlanterasHook, NPCID.PlanterasTentacle, NPCID.Splinterling, NPCID.MourningWood, NPCID.Everscream);
        // Poison
        public static bool[] ProjPoison = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.ThornChakram, ProjectileID.PoisonedKnife, ProjectileID.Stinger, ProjectileID.PoisonDart, ProjectileID.JungleSpike, ProjectileID.PoisonDartTrap, ProjectileID.PygmySpear, ProjectileID.PoisonFang, ProjectileID.PoisonDartBlowgun, ProjectileID.PoisonSeedPlantera, ProjectileID.VenomArrow, ProjectileID.VenomBullet, ProjectileID.VenomFang, ProjectileID.HornetStinger, ProjectileID.Hornet, ProjectileID.VenomSpider, ProjectileID.ToxicFlask, ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.ToxicBubble, ProjectileID.SalamanderSpit, ProjectileID.VortexAcid, ProjectileID.DD2OgreSpit, ProjectileID.QueenBeeStinger, ProjectileID.RollingCactusSpike, ProjectileID.SpiderHiver, ProjectileID.SpiderEgg, ProjectileID.BabySpider, ProjectileID.VenomDartTrap, ProjectileID.HiveFive);

        public static bool[] ItemPoison = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.BeeKeeper, ItemID.Flymeal, ItemID.PygmyStaff);

        public static bool[] NPCPoison = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.Bee, NPCID.BeeSmall, NPCID.Hornet, NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetLeafy, NPCID.HornetSpikey, NPCID.MossHornet, NPCID.ToxicSludge, NPCID.SwampThing, NPCID.QueenBee);
        // Blood
        public static bool[] ProjBlood = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.TheRottedFork, ProjectileID.TheMeatball, ProjectileID.BloodRain, ProjectileID.IchorArrow, ProjectileID.IchorBullet, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.VampireKnife, ProjectileID.SoulDrain, ProjectileID.IchorDart, ProjectileID.IchorSplash, ProjectileID.CrimsonYoyo, ProjectileID.BloodWater, ProjectileID.BatOfLight, ProjectileID.SharpTears, ProjectileID.DripplerFlail, ProjectileID.VampireFrog, ProjectileID.BloodShot, ProjectileID.BloodNautilusTears, ProjectileID.BloodNautilusShot, ProjectileID.BloodArrow, ProjectileID.DripplerFlailExtraBall, ProjectileID.BloodCloudRaining, ProjectileID.ButchersChainsaw, ProjectileID.BloodCloudMoving, ProjectileID.BloodyMachete, ProjectileID.TheEyeOfCthulhu, ProjectileID.BloodButcherer);

        public static bool[] ItemBlood = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.BloodButcherer, ItemID.Bladetongue, ItemID.DeathbringerPickaxe, ItemID.BloodLustCluster, ItemID.BloodHamaxe, ItemID.FleshGrinder, ItemID.PsychoKnife, ItemID.ZombieArm, ItemID.FetidBaghnakhs, ItemID.BladedGlove, ItemID.BloodRainBow);

        public static bool[] NPCBlood = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.BloodCrawler, NPCID.BloodCrawlerWall, NPCID.Crimera, NPCID.EyeballFlyingFish, NPCID.CataractEye, NPCID.DemonEye, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.WanderingEye, NPCID.FaceMonster, NPCID.BloodMummy, NPCID.BloodJelly, NPCID.BloodFeeder, NPCID.Crimslime, NPCID.CrimsonAxe, NPCID.BigMimicCrimson, NPCID.FloatyGross, NPCID.Herpling, NPCID.IchorSticker, NPCID.DesertGhoulCrimson, NPCID.BloodEelBody, NPCID.BloodEelHead, NPCID.BloodEelTail, NPCID.BloodSquid, NPCID.BloodZombie, NPCID.BloodNautilus, NPCID.Drippler, NPCID.GoblinShark, NPCID.CrimsonBunny, NPCID.CrimsonGoldfish, NPCID.CrimsonPenguin, NPCID.SandsharkCrimson, NPCID.Vampire, NPCID.BrainofCthulhu, NPCID.EyeofCthulhu, NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.Creeper, NPCID.LeechBody, NPCID.LeechHead, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII, NPCID.ServantofCthulhu, NPCID.Butcher);
        // Psychic
        public static bool[] ProjPsychic = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.BrainScramblerBolt, ProjectileID.MedusaHeadRay, ProjectileID.BookStaffShot, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ProjectileID.MedusaHead, ProjectileID.DeadlySphere, ProjectileID.AbigailMinion, ProjectileID.AbigailCounter, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot);

        public static bool[] ItemPsychic = ItemID.Sets.Factory.CreateBoolSet(ItemID.BoneHelm, ItemID.BrainScrambler);

        public static bool[] NPCPsychic = NPCID.Sets.Factory.CreateBoolSet(NPCID.NebulaHeadcrab);
        // Celestial
        public static bool[] ProjCelestial = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.Starfury, ProjectileID.FallingStar, ProjectileID.RainbowRodBullet, ProjectileID.HallowStar, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.PhantasmalEye, ProjectileID.PhantasmalSphere, ProjectileID.PhantasmalDeathray, ProjectileID.Meowmere, ProjectileID.StarWrath, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaBolt, ProjectileID.NebulaEye, ProjectileID.NebulaSphere, ProjectileID.NebulaLaser, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.RainbowCrystalExplosion, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.SparkleGuitar, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FairyQueenRangedItemShot, ProjectileID.FinalFractal, ProjectileID.EmpressBlade, ProjectileID.PrincessWeapon, ProjectileID.StarCannonStar, ProjectileID.SolarFlareDrill, ProjectileID.NebulaDrill, ProjectileID.VortexDrill, ProjectileID.StardustDrill, ProjectileID.MoonlordArrow, ProjectileID.MoonlordBullet, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.LastPrism, ProjectileID.RainbowCrystal, ProjectileID.MoonlordTurret, ProjectileID.ShimmerArrow, ProjectileID.ShimmerFlare, ProjectileID.MoonBoulder);

        public static bool[] ItemCelestial = ItemID.Sets.Factory.CreateBoolSet(
            ItemID.Starfury, ItemID.PiercingStarlight, ItemID.StarWrath, ItemID.Meowmere, ItemID.SolarFlarePickaxe, ItemID.NebulaPickaxe, ItemID.VortexPickaxe, ItemID.StardustPickaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeSolar, ItemID.LunarHamaxeStardust, ItemID.LunarHamaxeVortex, ItemID.FairyQueenRangedItem, ItemID.HolyArrow, ItemID.StarCloak, ItemID.BeeCloak, ItemID.ManaCloak, ItemID.StarVeil);

        public static bool[] NPCCelestial = NPCID.Sets.Factory.CreateBoolSet(
            NPCID.MeteorHead, NPCID.NebulaBeast, NPCID.NebulaBrain, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarFlare, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.StardustCellBig, NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustJellyfishSmall, NPCID.StardustSoldier, NPCID.StardustSpiderBig, NPCID.StardustSpiderSmall, NPCID.StardustWormBody, NPCID.StardustWormHead, NPCID.StardustWormTail, NPCID.VortexHornet, NPCID.VortexHornetQueen, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.HallowBoss, NPCID.MoonLordCore, NPCID.MoonLordFreeEye, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.ShimmerSlime);
        // Explosive
        public static bool[] ProjExplosive = ProjectileID.Sets.Factory.CreateBoolSet(
            ProjectileID.Bomb, ProjectileID.BombFish, ProjectileID.Grenade, ProjectileID.Dynamite, ProjectileID.StickyBomb, ProjectileID.StickyDynamite, ProjectileID.StickyGrenade, ProjectileID.HellfireArrow, ProjectileID.HappyBomb, ProjectileID.BombSkeletronPrime, ProjectileID.Explosives, ProjectileID.GrenadeI, ProjectileID.GrenadeII, ProjectileID.GrenadeIII, ProjectileID.GrenadeIV, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ProjectileID.ProximityMineI, ProjectileID.ProximityMineII, ProjectileID.ProximityMineIII, ProjectileID.ProximityMineIV, ProjectileID.Landmine, ProjectileID.Beenade, ProjectileID.ExplosiveBunny, ProjectileID.ExplosiveBullet, ProjectileID.RocketSkeleton, ProjectileID.JackOLantern, ProjectileID.OrnamentFriendly, ProjectileID.OrnamentStar, ProjectileID.RocketSnowmanI, ProjectileID.RocketSnowmanII, ProjectileID.RocketSnowmanIII, ProjectileID.RocketSnowmanIV, ProjectileID.Missile, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerMissile, ProjectileID.SeedlerNut, ProjectileID.BouncyBomb, ProjectileID.BouncyDynamite, ProjectileID.BouncyGrenade, ProjectileID.PartyGirlGrenade, ProjectileID.SolarWhipSwordExplosion, ProjectileID.VortexBeaterRocket, ProjectileID.LunarFlare, ProjectileID.DD2GoblinBomb, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.ScarabBomb, ProjectileID.ClusterRocketI, ProjectileID.ClusterRocketII, ProjectileID.ClusterGrenadeI, ProjectileID.ClusterGrenadeII, ProjectileID.ClusterMineI, ProjectileID.ClusterMineII, ProjectileID.MiniNukeRocketI, ProjectileID.MiniNukeRocketII, ProjectileID.MiniNukeGrenadeI, ProjectileID.MiniNukeGrenadeII, ProjectileID.MiniNukeMineI, ProjectileID.MiniNukeMineII, ProjectileID.ClusterSnowmanRocketI, ProjectileID.ClusterSnowmanRocketII, ProjectileID.MiniNukeSnowmanRocketI, ProjectileID.MiniNukeSnowmanRocketII, ProjectileID.SantankMountRocket, ProjectileID.DaybreakExplosion, ProjectileID.NailFriendly, ProjectileID.Nail, ProjectileID.Celeb2Rocket, ProjectileID.Celeb2RocketExplosive, ProjectileID.Celeb2RocketExplosiveLarge, ProjectileID.Celeb2RocketLarge, ProjectileID.Celeb2Weapon, ProjectileID.Stynger, ProjectileID.MolotovCocktail, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.MonkStaffT1Explosion, ProjectileID.FireWhipProj, ProjectileID.FireWhip, ProjectileID.DD2ExplosiveTrapT1, ProjectileID.DD2ExplosiveTrapT2, ProjectileID.DD2ExplosiveTrapT3, ProjectileID.TNTBarrel, ProjectileID.Volcano);

        public static bool[] ItemExplosive = ItemID.Sets.Factory.CreateBoolSet(ItemID.DayBreak, ItemID.SantankMountItem, ItemID.VortexBeater);

        public static bool[] NPCExplosive = NPCID.Sets.Factory.CreateBoolSet(NPCID.ChatteringTeethBomb, NPCID.ExplosiveBunny);
        //

        public static bool[] ProjectilesInheritElements = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] ProjectilesInheritElementsFromThis = ProjectileID.Sets.Factory.CreateBoolSet();

        #endregion

        private static int HasElementPlayerEffect(Terraria.Player player, Entity entity)
        {
            if (player.RedemptionPlayerBuff().eldritchRoot)
            {
                if (entity is Projectile proj && proj.HasElement(Nature, false))
                    return Shadow;
                else if (entity is Item item && item.HasElementItem(Nature, false))
                    return Shadow;
            }
            if (player.GetModPlayer<ExplosiveEnchantPlayer>().explosiveWeaponImbue)
            {
                if (entity is Projectile proj && (proj.DamageType.CountsAsClass(DamageClass.Melee) || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments)
                    return Explosive;
                else if (entity is Item item && item.DamageType.CountsAsClass(DamageClass.Melee))
                    return Explosive;
            }
            return 0;
        }
        public static bool HasElement(this Projectile proj, int ID = 0, bool checkPlayerEffects = true)
        {
            if (checkPlayerEffects && proj.friendly && ID == HasElementPlayerEffect(Main.player[proj.owner], proj))
                return true;
            if (proj.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ID] is AddElement)
                return true;
            if (proj.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ID] is RemoveElement)
                return false;
            return ID switch
            {
                1 => ProjArcane[proj.type],
                2 => ProjFire[proj.type],
                3 => ProjWater[proj.type],
                4 => ProjIce[proj.type],
                5 => ProjEarth[proj.type],
                6 => ProjWind[proj.type],
                7 => ProjThunder[proj.type],
                8 => ProjHoly[proj.type],
                9 => ProjShadow[proj.type],
                10 => ProjNature[proj.type],
                11 => ProjPoison[proj.type],
                12 => ProjBlood[proj.type],
                13 => ProjPsychic[proj.type],
                14 => ProjCelestial[proj.type],
                15 => ProjExplosive[proj.type],
                _ => false,
            };
        }
        public static int GetFirstElement(this Projectile proj, bool ignoreExplosive = false)
        {
            bool[] array = new bool[16];
            for (int i = 0; i < 15; i++)
            {
                if (proj.friendly && i + 1 == HasElementPlayerEffect(Main.player[proj.owner], proj))
                    return i + 1;

                if (proj.GetGlobalProjectile<ElementalProjectile>().OverrideElement[i + 1] is AddElement)
                    return i + 1;
                if (proj.GetGlobalProjectile<ElementalProjectile>().OverrideElement[i + 1] is RemoveElement)
                    array[i + 1] = true;

                bool hasElement = (i + 1) switch
                {
                    1 => ProjArcane[proj.type],
                    2 => ProjFire[proj.type],
                    3 => ProjWater[proj.type],
                    4 => ProjIce[proj.type],
                    5 => ProjEarth[proj.type],
                    6 => ProjWind[proj.type],
                    7 => ProjThunder[proj.type],
                    8 => ProjHoly[proj.type],
                    9 => ProjShadow[proj.type],
                    10 => ProjNature[proj.type],
                    11 => ProjPoison[proj.type],
                    12 => ProjBlood[proj.type],
                    13 => ProjPsychic[proj.type],
                    14 => ProjCelestial[proj.type],
                    15 => !ignoreExplosive && ProjExplosive[proj.type],
                    _ => false,
                };
                if (hasElement && hasElement != array[i + 1])
                    return i + 1;
            }
            return 0;
        }
        public static bool HasElement(int type, int ID = 0)
        {
            return ID switch
            {
                1 => ProjArcane[type],
                2 => ProjFire[type],
                3 => ProjWater[type],
                4 => ProjIce[type],
                5 => ProjEarth[type],
                6 => ProjWind[type],
                7 => ProjThunder[type],
                8 => ProjHoly[type],
                9 => ProjShadow[type],
                10 => ProjNature[type],
                11 => ProjPoison[type],
                12 => ProjBlood[type],
                13 => ProjPsychic[type],
                14 => ProjCelestial[type],
                15 => ProjExplosive[type],
                _ => false,
            };
        }
        public static bool HasElementFromProj(int ID = 0, params int[] types)
        {
            var b = false;
            for (int j = 0; j < types.Length; j++)
            {
                b = ID switch
                {
                    1 => ProjArcane[types[j]],
                    2 => ProjFire[types[j]],
                    3 => ProjWater[types[j]],
                    4 => ProjIce[types[j]],
                    5 => ProjEarth[types[j]],
                    6 => ProjWind[types[j]],
                    7 => ProjThunder[types[j]],
                    8 => ProjHoly[types[j]],
                    9 => ProjShadow[types[j]],
                    10 => ProjNature[types[j]],
                    11 => ProjPoison[types[j]],
                    12 => ProjBlood[types[j]],
                    13 => ProjPsychic[types[j]],
                    14 => ProjCelestial[types[j]],
                    15 => ProjExplosive[types[j]],
                    _ => false,
                };
                if (b)
                    break;
            }
            return b;
        }
        public static bool HasElement(this Item item, int ID = 0)
        {
            if (ID == HasElementPlayerEffect(Main.LocalPlayer, item))
                return true;

            if (item.TryGetGlobalItem(out ElementalItem elemItem))
            {
                if (elemItem.OverrideElement[ID] is AddElement)
                    return true;
                if (elemItem.OverrideElement[ID] is RemoveElement)
                    return false;
            }
            return ID switch
            {
                1 => ItemArcane[item.type],
                2 => ItemFire[item.type],
                3 => ItemWater[item.type],
                4 => ItemIce[item.type],
                5 => ItemEarth[item.type],
                6 => ItemWind[item.type],
                7 => ItemThunder[item.type],
                8 => ItemHoly[item.type],
                9 => ItemShadow[item.type],
                10 => ItemNature[item.type],
                11 => ItemPoison[item.type],
                12 => ItemBlood[item.type],
                13 => ItemPsychic[item.type],
                14 => ItemCelestial[item.type],
                15 => ItemExplosive[item.type],
                _ => false,
            };
        }
        public static int GetFirstElement(this Item item, bool ignoreExplosive = false)
        {
            bool[] array = new bool[16];
            for (int i = 0; i < 15; i++)
            {
                if (i + 1 == HasElementPlayerEffect(Main.LocalPlayer, item))
                    return i + 1;

                if (item.TryGetGlobalItem(out ElementalItem elemItem))
                {
                    if (elemItem.OverrideElement[i + 1] is AddElement)
                        return i + 1;
                    if (elemItem.OverrideElement[i + 1] is RemoveElement)
                        array[i + 1] = true;
                }
                bool hasElement = (i + 1) switch
                {
                    1 => ItemArcane[item.type],
                    2 => ItemFire[item.type],
                    3 => ItemWater[item.type],
                    4 => ItemIce[item.type],
                    5 => ItemEarth[item.type],
                    6 => ItemWind[item.type],
                    7 => ItemThunder[item.type],
                    8 => ItemHoly[item.type],
                    9 => ItemShadow[item.type],
                    10 => ItemNature[item.type],
                    11 => ItemPoison[item.type],
                    12 => ItemBlood[item.type],
                    13 => ItemPsychic[item.type],
                    14 => ItemCelestial[item.type],
                    15 => !ignoreExplosive && ItemExplosive[item.type],
                    _ => false,
                };
                if (hasElement && hasElement != array[i + 1])
                    return i + 1;
            }
            return 0;
        }
        public static bool HasElementItem(this Item item, int ID = 0, bool checkAccessories = true)
        {
            bool itemTrue = ID switch
            {
                1 => ItemArcane[item.type],
                2 => ItemFire[item.type],
                3 => ItemWater[item.type],
                4 => ItemIce[item.type],
                5 => ItemEarth[item.type],
                6 => ItemWind[item.type],
                7 => ItemThunder[item.type],
                8 => ItemHoly[item.type],
                9 => ItemShadow[item.type],
                10 => ItemNature[item.type],
                11 => ItemPoison[item.type],
                12 => ItemBlood[item.type],
                13 => ItemPsychic[item.type],
                14 => ItemCelestial[item.type],
                15 => ItemExplosive[item.type],
                _ => false,
            };

            if (checkAccessories && ID == HasElementPlayerEffect(Main.LocalPlayer, item))
                itemTrue = true;

            if (item.TryGetGlobalItem(out ElementalItem elemItem))
            {
                if (elemItem.OverrideElement[ID] is AddElement)
                    itemTrue = true;
                if (elemItem.OverrideElement[ID] is RemoveElement)
                    itemTrue = false;
                if (itemTrue)
                    return true;
                return HasElementFromProj(ID, item.shoot);
            }
            return false;
        }
        public static bool HasElement(this Terraria.NPC npc, int ID = 0)
        {
            if (npc.GetGlobalNPC<ElementalNPC>().OverrideElement[ID] is AddElement)
                return true;
            if (npc.GetGlobalNPC<ElementalNPC>().OverrideElement[ID] is RemoveElement)
                return false;
            return ID switch
            {
                1 => NPCArcane[npc.type],
                2 => NPCFire[npc.type],
                3 => NPCWater[npc.type],
                4 => NPCIce[npc.type],
                5 => NPCEarth[npc.type],
                6 => NPCWind[npc.type],
                7 => NPCThunder[npc.type],
                8 => NPCHoly[npc.type],
                9 => NPCShadow[npc.type],
                10 => NPCNature[npc.type],
                11 => NPCPoison[npc.type],
                12 => NPCBlood[npc.type],
                13 => NPCPsychic[npc.type],
                14 => NPCCelestial[npc.type],
                15 => NPCExplosive[npc.type],
                _ => false,
            };
        }
        public static int GetFirstElement(this Terraria.NPC npc, bool ignoreExplosive = false)
        {
            bool[] array = new bool[16];
            for (int i = 0; i < 15; i++)
            {
                if (npc.TryGetGlobalNPC(out ElementalNPC elemNPC))
                {
                    if (elemNPC.OverrideElement[i + 1] is AddElement)
                        return i + 1;
                    if (elemNPC.OverrideElement[i + 1] is RemoveElement)
                        array[i + 1] = true;
                }
                bool hasElement = (i + 1) switch
                {
                    1 => ItemArcane[npc.type],
                    2 => ItemFire[npc.type],
                    3 => ItemWater[npc.type],
                    4 => ItemIce[npc.type],
                    5 => ItemEarth[npc.type],
                    6 => ItemWind[npc.type],
                    7 => ItemThunder[npc.type],
                    8 => ItemHoly[npc.type],
                    9 => ItemShadow[npc.type],
                    10 => ItemNature[npc.type],
                    11 => ItemPoison[npc.type],
                    12 => ItemBlood[npc.type],
                    13 => ItemPsychic[npc.type],
                    14 => ItemCelestial[npc.type],
                    15 => !ignoreExplosive && ItemExplosive[npc.type],
                    _ => false,
                };
                if (hasElement && hasElement != array[i + 1])
                    return i + 1;
            }
            return 0;
        }

        public static string ElementIconFromID(int ID)
        {
            return ID switch
            {
                Fire => "[i:Redemption/Fire]",
                Water => "[i:Redemption/Water]",
                Ice => "[i:Redemption/Ice]",
                Earth => "[i:Redemption/Earth]",
                Wind => "[i:Redemption/Wind]",
                Thunder => "[i:Redemption/Thunder]",
                Holy => "[i:Redemption/Holy]",
                Shadow => "[i:Redemption/Shadow]",
                Nature => "[i:Redemption/Nature]",
                Poison => "[i:Redemption/Poison]",
                Blood => "[i:Redemption/Blood]",
                Psychic => "[i:Redemption/Psychic]",
                Celestial => "[i:Redemption/Cosmic]",
                Explosive => "[i:Redemption/Explosive]",
                _ => "[i:Redemption/Arcane]",
            };
        }
        public static string ElementColorCodeFromID(int ID)
        {
            return ID switch
            {
                Fire => "[c/F57E19:",
                Water => "[c/329AEF:",
                Ice => "[c/B7FFFF:",
                Earth => "[c/A97D5D:",
                Wind => "[c/B4B4B4:",
                Thunder => "[c/FFFFE0:",
                Holy => "[c/FFED7C:",
                Shadow => "[c/7B68EE:",
                Nature => "[c/87CC00:",
                Poison => "[c/4D7F69:",
                Blood => "[c/CC4141:",
                Psychic => "[c/C888F5:",
                Celestial => "[c/FEBEF3:",
                Explosive => "[c/FFC896:",
                _ => "[c/D0CEFF:",
            };
        }
        public static int BonusItemFromID(int ID)
        {
            return ID switch
            {
                1 => ItemType<Axe>(),
                2 => ItemType<Spear>(),
                3 => ItemType<Hammer>(),
                4 => ItemType<Clash>(),
                5 => ItemType<Explosive>(),
                6 => ItemType<Arcane>(),
                7 => ItemType<Fire>(),
                8 => ItemType<Water>(),
                9 => ItemType<Ice>(),
                10 => ItemType<Earth>(),
                11 => ItemType<Wind>(),
                12 => ItemType<Thunder>(),
                13 => ItemType<Holy>(),
                14 => ItemType<Shadow>(),
                15 => ItemType<Nature>(),
                16 => ItemType<Poison>(),
                17 => ItemType<Blood>(),
                18 => ItemType<Psychic>(),
                19 => ItemType<Cosmic>(),
                _ => ItemType<Slash>(),
            };
        }
        public static string BonusPlainNameFromID(int ID)
        {
            return ID switch
            {
                1 => AxeS_Plain,
                2 => SpearS_Plain,
                3 => HammerS_Plain,
                4 => ClashS_Plain,
                5 => ExplosiveS_Plain,
                6 => ArcaneS_Plain,
                7 => FireS_Plain,
                8 => WaterS_Plain,
                9 => IceS_Plain,
                10 => EarthS_Plain,
                11 => WindS_Plain,
                12 => ThunderS_Plain,
                13 => HolyS_Plain,
                14 => ShadowS_Plain,
                15 => NatureS_Plain,
                16 => PoisonS_Plain,
                17 => BloodS_Plain,
                18 => PsychicS_Plain,
                19 => CelestialS_Plain,
                _ => SlashS_Plain,
            };
        }
        public static string BonusNameFromID(int ID)
        {
            return ID switch
            {
                1 => AxeS,
                2 => SpearS,
                3 => HammerS,
                4 => ClashS,
                5 => ExplosiveS,
                6 => ArcaneS,
                7 => FireS,
                8 => WaterS,
                9 => IceS,
                10 => EarthS,
                11 => WindS,
                12 => ThunderS,
                13 => HolyS,
                14 => ShadowS,
                15 => NatureS,
                16 => PoisonS,
                17 => BloodS,
                18 => PsychicS,
                19 => CelestialS,
                _ => SlashS,
            };
        }
        public static string BonusDescFromID(int ID)
        {
            return ID switch
            {
                1 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.AxeBonus"),
                2 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SpearBonus"),
                3 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.HammerBonus"),
                4 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ClashBonus"),
                5 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ExplodeBonus"),
                6 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ArcaneBonus"),
                7 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.FireBonus"),
                8 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.WaterBonus"),
                9 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.IceBonus"),
                10 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.EarthBonus"),
                11 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.WindBonus"),
                12 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ThunderBonus"),
                13 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.HolyBonus"),
                14 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.ShadowBonus"),
                15 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.NatureBonus"),
                16 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.PoisonBonus"),
                17 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.BloodBonus"),
                18 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.PsychicBonus"),
                19 => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.CelestialBonus"),
                _ => Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus"),
            };
        }

        public const short Arcane = 1;
        public const short Fire = 2;
        public const short Water = 3;
        public const short Ice = 4;
        public const short Earth = 5;
        public const short Wind = 6;
        public const short Thunder = 7;
        public const short Holy = 8;
        public const short Shadow = 9;
        public const short Nature = 10;
        public const short Poison = 11;
        public const short Blood = 12;
        public const short Psychic = 13;
        public const short Celestial = 14;
        public const short Explosive = 15;
        public const sbyte AddElement = 1;
        public const sbyte RemoveElement = -1;

        public static string ArcaneS = Language.GetTextValue("Mods.Redemption.Items.Arcane.DisplayName");
        public static string FireS = Language.GetTextValue("Mods.Redemption.Items.Fire.DisplayName");
        public static string WaterS = Language.GetTextValue("Mods.Redemption.Items.Water.DisplayName");
        public static string IceS = Language.GetTextValue("Mods.Redemption.Items.Ice.DisplayName");
        public static string EarthS = Language.GetTextValue("Mods.Redemption.Items.Earth.DisplayName");
        public static string WindS = Language.GetTextValue("Mods.Redemption.Items.Wind.DisplayName");
        public static string ThunderS = Language.GetTextValue("Mods.Redemption.Items.Thunder.DisplayName");
        public static string HolyS = Language.GetTextValue("Mods.Redemption.Items.Holy.DisplayName");
        public static string ShadowS = Language.GetTextValue("Mods.Redemption.Items.Shadow.DisplayName");
        public static string NatureS = Language.GetTextValue("Mods.Redemption.Items.Nature.DisplayName");
        public static string PoisonS = Language.GetTextValue("Mods.Redemption.Items.Poison.DisplayName");
        public static string BloodS = Language.GetTextValue("Mods.Redemption.Items.Blood.DisplayName");
        public static string PsychicS = Language.GetTextValue("Mods.Redemption.Items.Psychic.DisplayName");
        public static string CelestialS = Language.GetTextValue("Mods.Redemption.Items.Cosmic.DisplayName");
        public static string ExplosiveS = Language.GetTextValue("Mods.Redemption.Items.Explosive.DisplayName");

        public static string SlashS = Language.GetTextValue("Mods.Redemption.Items.Slash.DisplayName");
        public static string AxeS = Language.GetTextValue("Mods.Redemption.Items.Axe.DisplayName");
        public static string HammerS = Language.GetTextValue("Mods.Redemption.Items.Hammer.DisplayName");
        public static string SpearS = Language.GetTextValue("Mods.Redemption.Items.Spear.DisplayName");
        public static string ClashS = Language.GetTextValue("Mods.Redemption.Items.Clash.DisplayName");

        public static string ArcaneS_Plain = Language.GetTextValue("Mods.Redemption.Items.Arcane.PlainName");
        public static string FireS_Plain = Language.GetTextValue("Mods.Redemption.Items.Fire.PlainName");
        public static string WaterS_Plain = Language.GetTextValue("Mods.Redemption.Items.Water.PlainName");
        public static string IceS_Plain = Language.GetTextValue("Mods.Redemption.Items.Ice.PlainName");
        public static string EarthS_Plain = Language.GetTextValue("Mods.Redemption.Items.Earth.PlainName");
        public static string WindS_Plain = Language.GetTextValue("Mods.Redemption.Items.Wind.PlainName");
        public static string ThunderS_Plain = Language.GetTextValue("Mods.Redemption.Items.Thunder.PlainName");
        public static string HolyS_Plain = Language.GetTextValue("Mods.Redemption.Items.Holy.PlainName");
        public static string ShadowS_Plain = Language.GetTextValue("Mods.Redemption.Items.Shadow.PlainName");
        public static string NatureS_Plain = Language.GetTextValue("Mods.Redemption.Items.Nature.PlainName");
        public static string PoisonS_Plain = Language.GetTextValue("Mods.Redemption.Items.Poison.PlainName");
        public static string BloodS_Plain = Language.GetTextValue("Mods.Redemption.Items.Blood.PlainName");
        public static string PsychicS_Plain = Language.GetTextValue("Mods.Redemption.Items.Psychic.PlainName");
        public static string CelestialS_Plain = Language.GetTextValue("Mods.Redemption.Items.Cosmic.PlainName");
        public static string ExplosiveS_Plain = Language.GetTextValue("Mods.Redemption.Items.Explosive.PlainName");

        public static string SlashS_Plain = Language.GetTextValue("Mods.Redemption.Items.Slash.PlainName");
        public static string AxeS_Plain = Language.GetTextValue("Mods.Redemption.Items.Axe.PlainName");
        public static string HammerS_Plain = Language.GetTextValue("Mods.Redemption.Items.Hammer.PlainName");
        public static string SpearS_Plain = Language.GetTextValue("Mods.Redemption.Items.Spear.PlainName");
        public static string ClashS_Plain = Language.GetTextValue("Mods.Redemption.Items.Clash.PlainName");
    }
}
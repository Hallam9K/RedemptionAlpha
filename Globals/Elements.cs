using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.Globals
{
    public static class ElementID
    {
        #region Element Bools
        public static bool[] ProjArcane = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.EnchantedBoomerang, ProjectileID.Starfury, ProjectileID.MagicMissile, ProjectileID.EighthNote, ProjectileID.QuarterNote, ProjectileID.TiedEighthNote, ProjectileID.RainbowRodBullet, ProjectileID.EyeLaser, ProjectileID.PinkLaser, ProjectileID.PurpleLaser, ProjectileID.MagicDagger, ProjectileID.CrystalStorm, ProjectileID.DeathLaser, ProjectileID.SwordBeam, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt, ProjectileID.RuneBlast, ProjectileID.TerraBeam, ProjectileID.LightBeam, ProjectileID.NightBeam, ProjectileID.EnchantedBeam, ProjectileID.FrostBeam, ProjectileID.EyeBeam, ProjectileID.Skull, ProjectileID.DeathSickle, ProjectileID.LostSoulFriendly, ProjectileID.LostSoulHostile, ProjectileID.Shadowflames, ProjectileID.VampireKnife, ProjectileID.SpectreWrath, ProjectileID.PulseBolt, ProjectileID.MiniRetinaLaser, ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardShaft, ProjectileID.CrystalPulse, ProjectileID.CrystalPulse2, ProjectileID.MedusaHeadRay, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaLaser, ProjectileID.VortexLaser, ProjectileID.ClothiersCurse, ProjectileID.MinecartMechLaser, ProjectileID.TerrarianBeam, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.StardustGuardianExplosion, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.PhantasmArrow, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.LunarFlare, ProjectileID.SkyFracture, ProjectileID.DD2DarkMageBolt, ProjectileID.BookOfSkullsSkull, ProjectileID.SparkleGuitar, ProjectileID.TitaniumStormShard, ProjectileID.StardustPunch, ProjectileID.NebulaDrill, ProjectileID.StardustDrill, ProjectileID.JestersArrow, ProjectileID.MonkStaffT2Ghast, ProjectileID.AbigailMinion, ProjectileID.AbigailCounter, ProjectileID.Trimarang);
        public static bool[] ItemArcane = ItemID.Sets.Factory.CreateBoolSet(ItemID.EnchantedSword, ItemID.SpectrePickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe, ItemID.SpectreHamaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeStardust);
        public static bool[] NPCArcane = NPCID.Sets.Factory.CreateBoolSet(NPCID.CursedSkull, NPCID.GiantCursedSkull, NPCID.GraniteFlyer, NPCID.Tim, NPCID.ChaosBallTim, NPCID.ChaosBall, NPCID.WaterSphere, NPCID.ChaosElemental, NPCID.CursedHammer, NPCID.CrimsonAxe, NPCID.EnchantedSword, NPCID.DungeonSpirit, NPCID.ShadowFlameApparition, NPCID.PirateGhost, NPCID.Poltergeist, NPCID.NebulaBeast, NPCID.NebulaBrain, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier, NPCID.StardustCellBig, NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustJellyfishSmall, NPCID.StardustSoldier, NPCID.StardustSpiderBig, NPCID.StardustSpiderSmall, NPCID.StardustWormBody, NPCID.StardustWormHead, NPCID.StardustWormTail, NPCID.CultistDragonBody1, NPCID.CultistDragonBody2, NPCID.CultistDragonBody3, NPCID.CultistDragonBody4, NPCID.CultistDragonHead, NPCID.CultistDragonTail);

        public static bool[] ProjFire = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.FireArrow, ProjectileID.BallofFire, ProjectileID.Flamarang, ProjectileID.Flamelash, ProjectileID.Sunfury, ProjectileID.HellfireArrow, ProjectileID.FlamingArrow, ProjectileID.Flames, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.RuneBlast, ProjectileID.FrostburnArrow, ProjectileID.FlamethrowerTrap, ProjectileID.FlamesTrap, ProjectileID.Fireball, ProjectileID.HeatRay, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.FlamingWood, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.ImpFireball, ProjectileID.MolotovCocktail, ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.CultistBossFireBall, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.Hellwing, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.Spark, ProjectileID.HelFire, ProjectileID.ClothiersCurse, ProjectileID.DesertDjinnCurse, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.GeyserTrap, ProjectileID.SpiritFlame, ProjectileID.DD2FlameBurstTowerT1Shot, ProjectileID.DD2FlameBurstTowerT2Shot, ProjectileID.DD2FlameBurstTowerT3Shot, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.MonkStaffT2, ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2PhoenixBowShot, ProjectileID.DD2BetsyArrow, ProjectileID.ApprenticeStaffT3Shot, ProjectileID.FireWhipProj, ProjectileID.FireWhip, ProjectileID.FlamingMace, ProjectileID.TorchGod, ProjectileID.WandOfSparkingSpark, ProjectileID.SolarFlareDrill, ProjectileID.Flare, ProjectileID.BlueFlare, ProjectileID.FlyingImp, ProjectileID.Cascade, ProjectileID.DD2FlameBurstTowerT1, ProjectileID.DD2FlameBurstTowerT2, ProjectileID.DD2FlameBurstTowerT3, ProjectileID.DD2FlameBurstTowerT3, ProjectileID.Volcano, ProjectileID.TheHorsemansBlade, ProjectileID.HorsemanPumpkin, ProjectileID.CursedFlare, ProjectileID.RainbowFlare, ProjectileID.ShimmerFlare);
        public static bool[] ItemFire = ItemID.Sets.Factory.CreateBoolSet(ItemID.FieryGreatsword, ItemID.TheHorsemansBlade, ItemID.DD2SquireBetsySword, ItemID.MoltenPickaxe, ItemID.SolarFlarePickaxe, ItemID.MeteorHamaxe, ItemID.MoltenHamaxe, ItemID.LunarHamaxeSolar, ItemID.DD2PhoenixBow, ItemID.BluePhaseblade, ItemID.BluePhasesaber, ItemID.GreenPhaseblade, ItemID.GreenPhasesaber, ItemID.OrangePhaseblade, ItemID.OrangePhasesaber, ItemID.PurplePhaseblade, ItemID.PurplePhasesaber, ItemID.RedPhaseblade, ItemID.RedPhasesaber, ItemID.WhitePhaseblade, ItemID.WhitePhasesaber, ItemID.YellowPhaseblade, ItemID.YellowPhasesaber);
        public static bool[] NPCFire = NPCID.Sets.Factory.CreateBoolSet(NPCID.BlazingWheel, NPCID.FireImp, NPCID.Demon, NPCID.VoodooDemon, NPCID.Hellbat, NPCID.LavaSlime, NPCID.MeteorHead, NPCID.BurningSphere, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.HoppinJack, NPCID.Lavabat, NPCID.RedDevil, NPCID.RuneWizard, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarFlare, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.DD2Betsy);

        public static bool[] ProjWater = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.WaterStream, ProjectileID.WaterBolt, ProjectileID.BlueMoon, ProjectileID.HolyWater, ProjectileID.UnholyWater, ProjectileID.IcewaterSpit, ProjectileID.RainFriendly, ProjectileID.BloodRain, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.WaterGun, ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.FlaironBubble, ProjectileID.SlimeGun, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.Bubble, ProjectileID.Xenopopper, ProjectileID.ToxicBubble, ProjectileID.Kraken, ProjectileID.BloodWater, ProjectileID.Ale, ProjectileID.DD2OgreSpit, ProjectileID.QueenSlimeGelAttack, ProjectileID.GelBalloon, ProjectileID.VolatileGelatinBall, ProjectileID.Flairon, ProjectileID.MaceWhip, ProjectileID.Trident, ProjectileID.PainterPaintball, ProjectileID.MechanicalPiranha, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.BloodCloudMoving, ProjectileID.BloodCloudRaining, ProjectileID.Swordfish, ProjectileID.Muramasa);
        public static bool[] ItemWater = ItemID.Sets.Factory.CreateBoolSet(ItemID.Muramasa, ItemID.PurpleClubberfish, ItemID.BloodRainBow);
        public static bool[] NPCWater = NPCID.Sets.Factory.CreateBoolSet(NPCID.SlimedZombie, NPCID.SlimeMasked, NPCID.Slimer, NPCID.SlimeRibbonGreen, NPCID.SlimeRibbonRed, NPCID.SlimeRibbonWhite, NPCID.SlimeRibbonYellow, NPCID.SlimeSpiked, NPCID.ArmedZombieSlimed, NPCID.BlueSlime, NPCID.CorruptSlime, NPCID.DungeonSlime, NPCID.GoldenSlime, NPCID.IceSlime, NPCID.IlluminantSlime, NPCID.KingSlime, NPCID.MotherSlime, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple, NPCID.RainbowSlime, NPCID.SandSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime, NPCID.UmbrellaSlime, NPCID.Crimslime, NPCID.SeaSnail, NPCID.Shark, NPCID.BloodJelly, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.Squid, NPCID.WaterSphere, NPCID.Piranha, NPCID.AnglerFish, NPCID.Arapaima, NPCID.BloodFeeder, NPCID.FungoFish, NPCID.FloatyGross, NPCID.IcyMerman, NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow, NPCID.BloodSquid, NPCID.GoblinShark, NPCID.Drippler, NPCID.BloodZombie, NPCID.CorruptGoldfish, NPCID.CrimsonGoldfish, NPCID.ZombieMerman, NPCID.FlyingFish, NPCID.AngryNimbus, NPCID.CreatureFromTheDeep, NPCID.SwampThing, NPCID.DukeFishron, NPCID.Sharkron, NPCID.Sharkron2);

        public static bool[] ProjIce = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.IceBlock, ProjectileID.IceBoomerang, ProjectileID.IceBolt, ProjectileID.FrostBoltSword, ProjectileID.FrostArrow, ProjectileID.FrostBlastHostile, ProjectileID.SnowBallFriendly, ProjectileID.FrostburnArrow, ProjectileID.IceSpike, ProjectileID.IcewaterSpit, ProjectileID.BallofFrost, ProjectileID.FrostBeam, ProjectileID.IceSickle, ProjectileID.FrostBlastFriendly, ProjectileID.Blizzard, ProjectileID.NorthPoleWeapon, ProjectileID.NorthPoleSpear, ProjectileID.NorthPoleSnowflake, ProjectileID.FrostWave, ProjectileID.FrostShard, ProjectileID.FrostBoltStaff, ProjectileID.CultistBossIceMist, ProjectileID.FrostDaggerfish, ProjectileID.Amarok, ProjectileID.CoolWhip, ProjectileID.CoolWhipProj, ProjectileID.DeerclopsIceSpike, ProjectileID.DeerclopsRangedProjectile, ProjectileID.FlinxMinion, ProjectileID.FrostHydra, ProjectileID.WandOfFrostingFrost);
        public static bool[] ItemIce = ItemID.Sets.Factory.CreateBoolSet(ItemID.IceBlade, ItemID.IceSickle, ItemID.Frostbrand);
        public static bool[] NPCIce = NPCID.Sets.Factory.CreateBoolSet(NPCID.ZombieEskimo, NPCID.ArmedZombieEskimo, NPCID.IceBat, NPCID.IceSlime, NPCID.SpikedIceSlime, NPCID.IceElemental, NPCID.IceMimic, NPCID.IceTortoise, NPCID.IcyMerman, NPCID.IceGolem, NPCID.SnowBalla, NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.Flocko, NPCID.SnowFlinx, NPCID.Yeti, NPCID.Deerclops, NPCID.IceQueen);

        public static bool[] ProjEarth = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.Boulder, ProjectileID.BoulderStaffOfEarth, ProjectileID.GolemFist, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2OgreStomp, ProjectileID.DD2OgreSmash, ProjectileID.MonkStaffT1Explosion, ProjectileID.RollingCactus, ProjectileID.RockGolemRock, ProjectileID.BoneDagger, ProjectileID.BoneJavelin, ProjectileID.SandBallGun, ProjectileID.PearlSandBallGun, ProjectileID.CrimsandBallGun, ProjectileID.EbonsandBallGun, ProjectileID.SharpTears, ProjectileID.MonkStaffT1, ProjectileID.MoonBoulder, ProjectileID.BouncyBoulder, ProjectileID.MiniBoulder);
        public static bool[] ItemEarth = ItemID.Sets.Factory.CreateBoolSet(ItemID.Seedler, ItemID.FossilPickaxe, ItemID.Picksaw, ItemID.AntlionClaw, ItemID.AcornAxe);
        public static bool[] NPCEarth = NPCID.Sets.Factory.CreateBoolSet(NPCID.GraniteFlyer, NPCID.GraniteGolem, NPCID.SandSlime, NPCID.DesertBeast, NPCID.GiantTortoise, NPCID.IceTortoise, NPCID.RockGolem, NPCID.DesertScorpionWalk, NPCID.DesertScorpionWall, NPCID.Tumbleweed, NPCID.SandElemental, NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow, NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.Golem);

        public static bool[] ProjWind = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.Sharknado, ProjectileID.SharknadoBolt, ProjectileID.Cthulunado, ProjectileID.Tempest, ProjectileID.Typhoon, ProjectileID.SandnadoFriendly, ProjectileID.SandnadoHostile, ProjectileID.DD2SquireSonicBoom, ProjectileID.DD2ApprenticeStorm, ProjectileID.BookStaffShot, ProjectileID.WeatherPainShot, ProjectileID.RainNimbus, ProjectileID.RainCloudMoving, ProjectileID.RainCloudRaining, ProjectileID.LightDisc, ProjectileID.FlyingKnife);
        public static bool[] ItemWind = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] NPCWind = NPCID.Sets.Factory.CreateBoolSet(NPCID.Harpy, NPCID.Dandelion, NPCID.AngryNimbus, NPCID.Tumbleweed);

        public static bool[] ProjThunder = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.RuneBlast, ProjectileID.MagnetSphereBall, ProjectileID.MagnetSphereBolt, ProjectileID.UFOLaser, ProjectileID.ScutlixLaser, ProjectileID.ScutlixLaserFriendly, ProjectileID.MartianTurretBolt, ProjectileID.BrainScramblerBolt, ProjectileID.GigaZapperSpear, ProjectileID.RayGunnerLaser, ProjectileID.LaserMachinegunLaser, ProjectileID.Electrosphere, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerDeathray, ProjectileID.SaucerLaser, ProjectileID.InfluxWaver, ProjectileID.ChargedBlasterLaser, ProjectileID.ChargedBlasterOrb, ProjectileID.PhantasmalBolt, ProjectileID.CultistBossLightningOrb, ProjectileID.CultistBossLightningOrbArc, ProjectileID.DeadlySphere, ProjectileID.VortexVortexLightning, ProjectileID.VortexLightning, ProjectileID.MartianWalkerLaser, ProjectileID.VortexBeaterRocket, ProjectileID.DD2LightningBugZap, ProjectileID.DD2LightningAuraT1, ProjectileID.DD2LightningAuraT2, ProjectileID.DD2LightningAuraT3, ProjectileID.MonkStaffT3, ProjectileID.MonkStaffT3_Alt, ProjectileID.MonkStaffT3_AltShot, ProjectileID.ThunderSpear, ProjectileID.ThunderStaffShot, ProjectileID.ThunderSpearShot, ProjectileID.ZapinatorLaser, ProjectileID.VortexDrill, ProjectileID.InfluxWaver, ProjectileID.LaserMachinegun, ProjectileID.ChargedBlasterCannon, ProjectileID.UFOMinion, ProjectileID.MoonlordTurret, ProjectileID.MoonlordTurretLaser);
        public static bool[] ItemThunder = ItemID.Sets.Factory.CreateBoolSet(ItemID.InfluxWaver, ItemID.VortexPickaxe, ItemID.LunarHamaxeVortex);
        public static bool[] NPCThunder = NPCID.Sets.Factory.CreateBoolSet(NPCID.BloodJelly, NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish, NPCID.DD2LightningBugT3, NPCID.DeadlySphere, NPCID.MartianDrone, NPCID.MartianWalker, NPCID.MartianTurret, NPCID.VortexHornet, NPCID.VortexHornetQueen, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.MartianSaucer, NPCID.MartianSaucerCannon, NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret);

        public static bool[] ProjHoly = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.TheDaoofPow, ProjectileID.HolyWater, ProjectileID.HolyArrow, ProjectileID.HallowStar, ProjectileID.LightBeam, ProjectileID.Hamdrax, ProjectileID.PaladinsHammerHostile, ProjectileID.PaladinsHammerFriendly, ProjectileID.SkyFracture, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.BatOfLight, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.HallowJoustingLance, ProjectileID.RainbowWhip, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.LightDisc, ProjectileID.SwordWhip, ProjectileID.Gungnir, ProjectileID.PearlSandBallGun, ProjectileID.Chik, ProjectileID.VolatileGelatinBall, ProjectileID.Excalibur, ProjectileID.TrueExcalibur);
        public static bool[] ItemHoly = ItemID.Sets.Factory.CreateBoolSet(ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.PickaxeAxe, ItemID.Pwnhammer, ItemID.Keybrand);
        public static bool[] NPCHoly = NPCID.Sets.Factory.CreateBoolSet(NPCID.ChaosElemental, NPCID.DesertGhoulHallow, NPCID.EnchantedSword, NPCID.BigMimicHallow, NPCID.IlluminantBat, NPCID.IlluminantSlime, NPCID.LightMummy, NPCID.Pixie, NPCID.Paladin, NPCID.Unicorn, NPCID.RainbowSlime, NPCID.SandsharkHallow, NPCID.HallowBoss, NPCID.QueenSlimeBoss, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple);

        public static bool[] ProjShadow = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.UnholyArrow, ProjectileID.VilethornBase, ProjectileID.VilethornTip, ProjectileID.BallOHurt, ProjectileID.DemonSickle, ProjectileID.DemonScythe, ProjectileID.DarkLance, ProjectileID.TheDaoofPow, ProjectileID.UnholyWater, ProjectileID.CursedFlameFriendly, ProjectileID.CursedFlameHostile, ProjectileID.EyeFire, ProjectileID.CursedArrow, ProjectileID.CursedBullet, ProjectileID.UnholyTridentFriendly, ProjectileID.UnholyTridentHostile, ProjectileID.NightBeam, ProjectileID.DeathSickle, ProjectileID.ShadowBeamHostile, ProjectileID.ShadowBeamFriendly, ProjectileID.Shadowflames, ProjectileID.EatersBite, ProjectileID.TinyEater, ProjectileID.CultistBossFireBallClone, ProjectileID.CursedDart, ProjectileID.CursedDartFlame, ProjectileID.ClingerStaff, ProjectileID.ShadowFlameArrow, ProjectileID.ShadowFlame, ProjectileID.ShadowFlameKnife, ProjectileID.CorruptYoyo, ProjectileID.ClothiersCurse, ProjectileID.AncientDoomProjectile, ProjectileID.DesertDjinnCurse, ProjectileID.SpiritFlame, ProjectileID.BlackBolt, ProjectileID.DD2DrakinShot, ProjectileID.DD2DarkMageBolt, ProjectileID.ShadowJoustingLance, ProjectileID.ScytheWhipProj, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ProjectileID.EbonsandBallGun, ProjectileID.CrimsandBallGun, ProjectileID.BookOfSkullsSkull, ProjectileID.Bat, ProjectileID.Raven, ProjectileID.ScytheWhip, ProjectileID.HoundiusShootius, ProjectileID.HoundiusShootiusFireball, ProjectileID.NightsEdge, ProjectileID.TrueNightsEdge, ProjectileID.LightsBane, ProjectileID.CursedFlare);
        public static bool[] ItemShadow = ItemID.Sets.Factory.CreateBoolSet(ItemID.LightsBane, ItemID.PurpleClubberfish, ItemID.NightsEdge, ItemID.TrueNightsEdge, ItemID.DeathSickle, ItemID.NightmarePickaxe, ItemID.WarAxeoftheNight, ItemID.TheBreaker, ItemID.OnyxBlaster, ItemID.BoneHelm);
        public static bool[] NPCShadow = NPCID.Sets.Factory.CreateBoolSet(NPCID.DarkCaster, NPCID.DungeonSlime, NPCID.EaterofSouls, NPCID.DevourerBody, NPCID.DevourerHead, NPCID.DevourerTail, NPCID.Clinger, NPCID.BigMimicCorruption, NPCID.CorruptSlime, NPCID.Corruptor, NPCID.CursedHammer, NPCID.DarkMummy, NPCID.DesertDjinn, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.RaggedCaster, NPCID.RaggedCasterOpenCoat, NPCID.PossessedArmor, NPCID.DesertGhoulCorruption, NPCID.Slimer, NPCID.Wraith, NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail, NPCID.CorruptBunny, NPCID.CorruptGoldfish, NPCID.CorruptPenguin, NPCID.SandsharkCorrupt, NPCID.GoblinSummoner, NPCID.ShadowFlameApparition, NPCID.Reaper, NPCID.ThePossessed, NPCID.Vampire, NPCID.Hellhound, NPCID.HeadlessHorseman, NPCID.Splinterling, NPCID.Krampus, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail, NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3, NPCID.MourningWood, NPCID.Pumpking, NPCID.Ghost, NPCID.MotherSlime, NPCID.Tim, NPCID.ChaosBallTim, NPCID.ChaosBall, NPCID.Necromancer, NPCID.NecromancerArmored, NPCID.ShadowFlameApparition, NPCID.GoblinSummoner, NPCID.Poltergeist);

        public static bool[] ProjNature = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.ThornChakram, ProjectileID.Seed, ProjectileID.Mushroom, ProjectileID.TerraBeam, ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight, ProjectileID.JungleSpike, ProjectileID.Leaf, ProjectileID.FlowerPetal, ProjectileID.CrystalLeafShot, ProjectileID.SporeCloud, ProjectileID.ChlorophyteOrb, ProjectileID.FlowerPow, ProjectileID.FlowerPowPetal, ProjectileID.SeedPlantera, ProjectileID.PoisonSeedPlantera, ProjectileID.ThornBall, ProjectileID.JackOLantern, ProjectileID.FlamingJack, ProjectileID.PineNeedleFriendly, ProjectileID.PineNeedleHostile, ProjectileID.SeedlerNut, ProjectileID.SeedlerThorn, ProjectileID.JungleYoyo, ProjectileID.SporeTrap, ProjectileID.SporeTrap2, ProjectileID.SporeGas, ProjectileID.SporeGas2, ProjectileID.SporeGas3, ProjectileID.TruffleSpore, ProjectileID.Terrarian, ProjectileID.TerrarianBeam, ProjectileID.Terragrim, ProjectileID.DandelionSeed, ProjectileID.Shroomerang, ProjectileID.ThornWhip, ProjectileID.BabyBird, ProjectileID.MushroomSpear, ProjectileID.ChlorophyteArrow, ProjectileID.ChlorophyteBullet, ProjectileID.ChlorophyteChainsaw, ProjectileID.ChlorophyteJackhammer, ProjectileID.ChlorophyteDrill, ProjectileID.ChlorophytePartisan, ProjectileID.Bee, ProjectileID.BeeArrow, ProjectileID.GiantBee, ProjectileID.MechanicalPiranha, ProjectileID.Wasp, ProjectileID.Yelets, ProjectileID.BladeOfGrass, ProjectileID.TerraBlade2, ProjectileID.TerraBlade2Shot, ProjectileID.HiveFive);
        public static bool[] ItemNature = ItemID.Sets.Factory.CreateBoolSet(ItemID.CactusSword, ItemID.BladeofGrass, ItemID.Seedler, ItemID.ChlorophyteSaber, ItemID.ChristmasTreeSword, ItemID.ChlorophyteClaymore, ItemID.TerraBlade, ItemID.CactusPickaxe, ItemID.ChlorophytePickaxe, ItemID.ChlorophyteGreataxe, ItemID.Hammush, ItemID.ChlorophyteWarhammer, ItemID.SporeSac, ItemID.AcornAxe);
        public static bool[] NPCNature = NPCID.Sets.Factory.CreateBoolSet(NPCID.AnomuraFungus, NPCID.GiantFungiBulb, NPCID.FungiBulb, NPCID.SpikedJungleSlime, NPCID.JungleBat, NPCID.ManEater, NPCID.JungleBat, NPCID.JungleBat, NPCID.JungleBat, NPCID.JungleBat, NPCID.MushiLadybug, NPCID.Snatcher, NPCID.SporeBat, NPCID.SporeSkeleton, NPCID.ZombieMushroom, NPCID.ZombieMushroomHat, NPCID.AngryTrapper, NPCID.JungleCreeper, NPCID.JungleCreeperWall, NPCID.Lihzahrd, NPCID.LihzahrdCrawler, NPCID.Moth, NPCID.Pixie, NPCID.Dandelion, NPCID.Plantera, NPCID.PlanterasHook, NPCID.PlanterasTentacle, NPCID.Splinterling, NPCID.MourningWood, NPCID.Everscream);

        public static bool[] ProjPoison = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.ThornChakram, ProjectileID.PoisonedKnife, ProjectileID.Stinger, ProjectileID.PoisonDart, ProjectileID.JungleSpike, ProjectileID.PoisonDartTrap, ProjectileID.PygmySpear, ProjectileID.PoisonFang, ProjectileID.PoisonDartBlowgun, ProjectileID.PoisonSeedPlantera, ProjectileID.VenomArrow, ProjectileID.VenomBullet, ProjectileID.VenomFang, ProjectileID.HornetStinger, ProjectileID.Hornet, ProjectileID.VenomSpider, ProjectileID.ToxicFlask, ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3, ProjectileID.ToxicBubble, ProjectileID.SalamanderSpit, ProjectileID.VortexAcid, ProjectileID.DD2OgreSpit, ProjectileID.QueenBeeStinger, ProjectileID.RollingCactusSpike, ProjectileID.SpiderHiver, ProjectileID.SpiderEgg, ProjectileID.BabySpider, ProjectileID.VenomDartTrap, ProjectileID.HiveFive);
        public static bool[] ItemPoison = ItemID.Sets.Factory.CreateBoolSet(ItemID.BeeKeeper, ItemID.Flymeal);
        public static bool[] NPCPoison = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bee, NPCID.BeeSmall, NPCID.Hornet, NPCID.HornetFatty, NPCID.HornetHoney, NPCID.HornetLeafy, NPCID.HornetSpikey, NPCID.MossHornet, NPCID.ToxicSludge, NPCID.SwampThing, NPCID.QueenBee);

        public static bool[] ProjBlood = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.TheRottedFork, ProjectileID.TheMeatball, ProjectileID.BloodRain, ProjectileID.IchorArrow, ProjectileID.IchorBullet, ProjectileID.GoldenShowerFriendly, ProjectileID.GoldenShowerHostile, ProjectileID.VampireKnife, ProjectileID.SoulDrain, ProjectileID.IchorDart, ProjectileID.IchorSplash, ProjectileID.CrimsonYoyo, ProjectileID.BloodWater, ProjectileID.BatOfLight, ProjectileID.SharpTears, ProjectileID.DripplerFlail, ProjectileID.VampireFrog, ProjectileID.BloodShot, ProjectileID.BloodNautilusTears, ProjectileID.BloodNautilusShot, ProjectileID.BloodArrow, ProjectileID.DripplerFlailExtraBall, ProjectileID.BloodCloudRaining, ProjectileID.ButchersChainsaw, ProjectileID.BloodCloudMoving, ProjectileID.BloodyMachete, ProjectileID.TheEyeOfCthulhu, ProjectileID.BloodButcherer);
        public static bool[] ItemBlood = ItemID.Sets.Factory.CreateBoolSet(ItemID.BloodButcherer, ItemID.Bladetongue, ItemID.DeathbringerPickaxe, ItemID.BloodLustCluster, ItemID.BloodHamaxe, ItemID.FleshGrinder, ItemID.PsychoKnife, ItemID.ZombieArm, ItemID.FetidBaghnakhs, ItemID.BladedGlove, ItemID.BloodRainBow);
        public static bool[] NPCBlood = NPCID.Sets.Factory.CreateBoolSet(NPCID.BloodCrawler, NPCID.BloodCrawlerWall, NPCID.Crimera, NPCID.EyeballFlyingFish, NPCID.CataractEye, NPCID.DemonEye, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye, NPCID.WanderingEye, NPCID.FaceMonster, NPCID.BloodMummy, NPCID.BloodJelly, NPCID.BloodFeeder, NPCID.Crimslime, NPCID.CrimsonAxe, NPCID.BigMimicCrimson, NPCID.FloatyGross, NPCID.Herpling, NPCID.IchorSticker, NPCID.DesertGhoulCrimson, NPCID.BloodEelBody, NPCID.BloodEelHead, NPCID.BloodEelTail, NPCID.BloodSquid, NPCID.BloodZombie, NPCID.BloodNautilus, NPCID.Drippler, NPCID.GoblinShark, NPCID.CrimsonBunny, NPCID.CrimsonGoldfish, NPCID.CrimsonPenguin, NPCID.SandsharkCrimson, NPCID.Vampire, NPCID.BrainofCthulhu, NPCID.EyeofCthulhu, NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.Creeper, NPCID.LeechBody, NPCID.LeechHead, NPCID.LeechTail, NPCID.TheHungry, NPCID.TheHungryII, NPCID.ServantofCthulhu, NPCID.Butcher);

        public static bool[] ProjPsychic = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.BrainScramblerBolt, ProjectileID.MedusaHeadRay, ProjectileID.BookStaffShot, ProjectileID.InsanityShadowHostile, ProjectileID.InsanityShadowFriendly, ProjectileID.MedusaHead, ProjectileID.DeadlySphere, ProjectileID.AbigailMinion, ProjectileID.AbigailCounter);
        public static bool[] ItemPsychic = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] NPCPsychic = NPCID.Sets.Factory.CreateBoolSet();

        public static bool[] ProjCelestial = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.Starfury, ProjectileID.FallingStar, ProjectileID.RainbowRodBullet, ProjectileID.HallowStar, ProjectileID.RainbowBack, ProjectileID.RainbowFront, ProjectileID.PhantasmalEye, ProjectileID.PhantasmalSphere, ProjectileID.PhantasmalDeathray, ProjectileID.Meowmere, ProjectileID.StarWrath, ProjectileID.StardustSoldierLaser, ProjectileID.Twinkle, ProjectileID.NebulaBolt, ProjectileID.NebulaEye, ProjectileID.NebulaSphere, ProjectileID.NebulaLaser, ProjectileID.NebulaArcanum, ProjectileID.NebulaArcanumExplosionShot, ProjectileID.NebulaArcanumExplosionShotShard, ProjectileID.LastPrismLaser, ProjectileID.NebulaBlaze1, ProjectileID.NebulaBlaze2, ProjectileID.MoonlordTurretLaser, ProjectileID.RainbowCrystalExplosion, ProjectileID.ManaCloakStar, ProjectileID.BeeCloakStar, ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.SuperStar, ProjectileID.SuperStarSlash, ProjectileID.SparkleGuitar, ProjectileID.HallowBossLastingRainbow, ProjectileID.HallowBossRainbowStreak, ProjectileID.FairyQueenLance, ProjectileID.FairyQueenSunDance, ProjectileID.FairyQueenHymn, ProjectileID.PiercingStarlight, ProjectileID.FairyQueenMagicItemShot, ProjectileID.FairyQueenRangedItemShot, ProjectileID.FinalFractal, ProjectileID.EmpressBlade, ProjectileID.PrincessWeapon, ProjectileID.StarCannonStar, ProjectileID.SolarFlareDrill, ProjectileID.NebulaDrill, ProjectileID.VortexDrill, ProjectileID.StardustDrill, ProjectileID.MoonlordArrow, ProjectileID.MoonlordBullet, ProjectileID.StardustCellMinion, ProjectileID.StardustCellMinionShot, ProjectileID.StardustDragon1, ProjectileID.StardustDragon2, ProjectileID.StardustDragon3, ProjectileID.StardustDragon4, ProjectileID.SolarFlareRay, ProjectileID.SolarCounter, ProjectileID.SolarWhipSword, ProjectileID.SolarWhipSwordExplosion, ProjectileID.Daybreak, ProjectileID.DaybreakExplosion, ProjectileID.LastPrism, ProjectileID.RainbowCrystal, ProjectileID.MoonlordTurret, ProjectileID.MoonlordTurretLaser, ProjectileID.ShimmerArrow, ProjectileID.ShimmerFlare, ProjectileID.MoonBoulder);
        public static bool[] ItemCelestial = ItemID.Sets.Factory.CreateBoolSet(ItemID.Starfury, ItemID.PiercingStarlight, ItemID.StarWrath, ItemID.Meowmere, ItemID.SolarFlarePickaxe, ItemID.NebulaPickaxe, ItemID.VortexPickaxe, ItemID.StardustPickaxe, ItemID.LunarHamaxeNebula, ItemID.LunarHamaxeSolar, ItemID.LunarHamaxeStardust, ItemID.LunarHamaxeVortex, ItemID.FairyQueenRangedItem);
        public static bool[] NPCCelestial = NPCID.Sets.Factory.CreateBoolSet(NPCID.MeteorHead, NPCID.NebulaBeast, NPCID.NebulaBrain, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier, NPCID.SolarCorite, NPCID.SolarCrawltipedeTail, NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.SolarFlare, NPCID.SolarSolenian, NPCID.SolarSpearman, NPCID.SolarSroller, NPCID.StardustCellBig, NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustJellyfishSmall, NPCID.StardustSoldier, NPCID.StardustSpiderBig, NPCID.StardustSpiderSmall, NPCID.StardustWormBody, NPCID.StardustWormHead, NPCID.StardustWormTail, NPCID.VortexHornet, NPCID.VortexHornetQueen, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.HallowBoss, NPCID.MoonLordCore, NPCID.MoonLordFreeEye, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.ShimmerSlime);

        public static bool[] ProjExplosive = ProjectileID.Sets.Factory.CreateBoolSet(ProjectileID.Bomb, ProjectileID.BombFish, ProjectileID.Grenade, ProjectileID.Dynamite, ProjectileID.StickyBomb, ProjectileID.StickyDynamite, ProjectileID.StickyGrenade, ProjectileID.HellfireArrow, ProjectileID.HappyBomb, ProjectileID.BombSkeletronPrime, ProjectileID.Explosives, ProjectileID.GrenadeI, ProjectileID.GrenadeII, ProjectileID.GrenadeIII, ProjectileID.GrenadeIV, ProjectileID.RocketI, ProjectileID.RocketII, ProjectileID.RocketIII, ProjectileID.RocketIV, ProjectileID.ProximityMineI, ProjectileID.ProximityMineII, ProjectileID.ProximityMineIII, ProjectileID.ProximityMineIV, ProjectileID.Landmine, ProjectileID.Beenade, ProjectileID.ExplosiveBunny, ProjectileID.ExplosiveBullet, ProjectileID.RocketSkeleton, ProjectileID.JackOLantern, ProjectileID.OrnamentFriendly, ProjectileID.RocketSnowmanI, ProjectileID.RocketSnowmanII, ProjectileID.RocketSnowmanIII, ProjectileID.RocketSnowmanIV, ProjectileID.Missile, ProjectileID.Meteor1, ProjectileID.Meteor2, ProjectileID.Meteor3, ProjectileID.ElectrosphereMissile, ProjectileID.SaucerMissile, ProjectileID.SeedlerNut, ProjectileID.BouncyBomb, ProjectileID.BouncyDynamite, ProjectileID.BouncyGrenade, ProjectileID.PartyGirlGrenade, ProjectileID.SolarWhipSwordExplosion, ProjectileID.VortexBeaterRocket, ProjectileID.LunarFlare, ProjectileID.DD2GoblinBomb, ProjectileID.DD2ExplosiveTrapT1Explosion, ProjectileID.DD2ExplosiveTrapT2Explosion, ProjectileID.DD2ExplosiveTrapT3Explosion, ProjectileID.ScarabBomb, ProjectileID.ClusterRocketI, ProjectileID.ClusterRocketII, ProjectileID.ClusterGrenadeI, ProjectileID.ClusterGrenadeII, ProjectileID.ClusterMineI, ProjectileID.ClusterMineII, ProjectileID.MiniNukeRocketI, ProjectileID.MiniNukeRocketII, ProjectileID.MiniNukeGrenadeI, ProjectileID.MiniNukeGrenadeII, ProjectileID.MiniNukeMineI, ProjectileID.MiniNukeMineII, ProjectileID.ClusterSnowmanRocketI, ProjectileID.ClusterSnowmanRocketII, ProjectileID.MiniNukeSnowmanRocketI, ProjectileID.MiniNukeSnowmanRocketII, ProjectileID.SantankMountRocket, ProjectileID.DaybreakExplosion, ProjectileID.NailFriendly, ProjectileID.Nail, ProjectileID.Celeb2Rocket, ProjectileID.Celeb2RocketExplosive, ProjectileID.Celeb2RocketExplosiveLarge, ProjectileID.Celeb2RocketLarge, ProjectileID.Celeb2Weapon, ProjectileID.Stynger, ProjectileID.MolotovCocktail, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoHostileBlast, ProjectileID.InfernoHostileBolt, ProjectileID.MonkStaffT1Explosion, ProjectileID.FireWhipProj, ProjectileID.FireWhip, ProjectileID.DD2ExplosiveTrapT1, ProjectileID.DD2ExplosiveTrapT2, ProjectileID.DD2ExplosiveTrapT3, ProjectileID.TNTBarrel);
        public static bool[] ItemExplosive = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] NPCExplosive = NPCID.Sets.Factory.CreateBoolSet(NPCID.ChatteringTeethBomb);

        public static bool HasElement(this Projectile proj, int ID = 0)
        {
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
        public static bool HasElement(int ID = 0, params int[] types)
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
        public static bool HasElementItem(this Item item, int ID = 0)
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
            if (item.TryGetGlobalItem(out ElementalItem elemItem))
            {
                if (elemItem.OverrideElement[ID] is AddElement)
                    itemTrue = true;
                if (elemItem.OverrideElement[ID] is RemoveElement)
                    itemTrue = false;
                if (itemTrue)
                    return true;
                return HasElement(ID, item.shoot, elemItem.ItemShootExtra[0], elemItem.ItemShootExtra[1]);
            }
            return false;
        }
        public static void ExtraItemShoot(this Item item, int a = 0, int b = 0)
        {
            if (item.TryGetGlobalItem(out ElementalItem elemItem))
            {
                elemItem.ItemShootExtra[0] = a;
                elemItem.ItemShootExtra[1] = b;
            }
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
        public const short AddElement = 1;
        public const short RemoveElement = -1;

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
        public static string CelestialS = Language.GetTextValue("Mods.Redemption.Items.Celestial.DisplayName");
        public static string ExplosiveS = Language.GetTextValue("Mods.Redemption.Items.Explosive.DisplayName");
        #endregion
    }
    public class ElementalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int[] OverrideElement = new int[16];
        public override void ModifyHitNPC(Projectile projectile, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (projectile.HasElement(ElementID.Explosive))
                    modifiers.ScalingArmorPenetration += .2f;
            }
        }
        public override bool PreAI(Projectile projectile)
        {
            if (Main.player[projectile.owner].RedemptionPlayerBuff().eldritchRoot && projectile.HasElement(ElementID.Nature))
                OverrideElement[ElementID.Shadow] = 1;

            return base.PreAI(projectile);
        }
    }
    public class ElementalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int[] OverrideElement = new int[16];
        public int[] ItemShootExtra = new int[2];
        public override void ModifyWeaponCrit(Item item, Terraria.Player player, ref float crit)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (modPlayer.powerCell)
                {
                    if (item.HasElementItem(ElementID.Fire))
                        crit += 4;
                    if (item.HasElementItem(ElementID.Holy))
                        crit += 4;
                }
                if (modPlayer.gracesGuidance)
                {
                    if (item.HasElementItem(ElementID.Fire))
                        crit += 6;
                    if (item.HasElementItem(ElementID.Holy))
                        crit += 6;
                }
                if (modPlayer.sacredCross && item.HasElementItem(ElementID.Holy))
                    crit += 6;
                if (modPlayer.forestCore && player.dryadWard && item.HasElementItem(ElementID.Nature))
                    crit += 10;
                if (modPlayer.thornCirclet && item.HasElementItem(ElementID.Nature))
                    crit += 6;
            }
        }
        public override void ModifyWeaponDamage(Item item, Terraria.Player player, ref StatModifier damage)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (player.RedemptionPlayerBuff().eldritchRoot && item.HasElementItem(ElementID.Nature))
                OverrideElement[ElementID.Shadow] = 1;
            else
                OverrideElement[ElementID.Shadow] = 0;

            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (item.HasElementItem(ElementID.Arcane))
                    damage += modPlayer.ElementalDamage[ElementID.Arcane];
                if (item.HasElementItem(ElementID.Fire))
                    damage += modPlayer.ElementalDamage[ElementID.Fire];
                if (item.HasElementItem(ElementID.Water))
                    damage += modPlayer.ElementalDamage[ElementID.Water];
                if (item.HasElementItem(ElementID.Ice))
                    damage += modPlayer.ElementalDamage[ElementID.Ice];
                if (item.HasElementItem(ElementID.Earth))
                    damage += modPlayer.ElementalDamage[ElementID.Earth];
                if (item.HasElementItem(ElementID.Wind))
                    damage += modPlayer.ElementalDamage[ElementID.Wind];
                if (item.HasElementItem(ElementID.Thunder))
                    damage += modPlayer.ElementalDamage[ElementID.Thunder];
                if (item.HasElementItem(ElementID.Holy))
                    damage += modPlayer.ElementalDamage[ElementID.Holy];
                if (item.HasElementItem(ElementID.Shadow))
                    damage += modPlayer.ElementalDamage[ElementID.Shadow];
                if (item.HasElementItem(ElementID.Nature))
                    damage += modPlayer.ElementalDamage[ElementID.Nature];
                if (item.HasElementItem(ElementID.Poison))
                    damage += modPlayer.ElementalDamage[ElementID.Poison];
                if (item.HasElementItem(ElementID.Blood))
                    damage += modPlayer.ElementalDamage[ElementID.Blood];
                if (item.HasElementItem(ElementID.Psychic))
                    damage += modPlayer.ElementalDamage[ElementID.Psychic];
                if (item.HasElementItem(ElementID.Celestial))
                    damage += modPlayer.ElementalDamage[ElementID.Celestial];
                #endregion
            }
        }
    }
    public class ElementalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int[] OverrideElement = new int[16];
        public float[] OverrideMultiplier = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public float[] elementDmg = new float[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        public override void SetDefaults(Terraria.NPC npc)
        {
            SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                #region Elemental Attributes
                float multiplier = 1;
                ElementalEffects(npc, player, item, ref multiplier, ref modifiers);
                for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().elementDmg.Length; j++)
                {
                    if (npc.GetGlobalNPC<ElementalNPC>().elementDmg[j] is 1 || !item.HasElement(j))
                        continue;
                    multiplier *= npc.GetGlobalNPC<ElementalNPC>().elementDmg[j];
                }
                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;
                if (npc.boss)
                    multiplier = MathHelper.Clamp(multiplier, .75f, 1.25f);

                if (multiplier >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;

                SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
                #endregion
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(projectile.type))
            {
                #region Elemental Attributes
                float multiplier = 1;
                ElementalEffects(npc, projectile, ref multiplier, ref modifiers);
                for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().elementDmg.Length; j++)
                {
                    if (npc.GetGlobalNPC<ElementalNPC>().elementDmg[j] is 1 || !projectile.HasElement(j))
                        continue;
                    multiplier *= npc.GetGlobalNPC<ElementalNPC>().elementDmg[j];
                }
                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;
                if (npc.boss)
                    multiplier = MathHelper.Clamp(multiplier, .75f, 1.25f);

                if (multiplier >= 1.1f)
                    CombatText.NewText(npc.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(npc.getRect(), Color.IndianRed, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;

                SetElementalMultipliers(npc, ref npc.GetGlobalNPC<ElementalNPC>().elementDmg);
                #endregion
            }
        }
        public static void SetElementalMultipliers(Terraria.NPC npc, ref float[] multiplier)
        {
            for (int j = 0; j < npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier.Length; j++)
            {
                if (npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[j] == 1)
                    multiplier[j] = 1;
                else
                    multiplier[j] = npc.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[j];
            }
            if (NPCLists.Plantlike.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Wind] *= 1.25f;
                multiplier[ElementID.Nature] *= 0.75f;
                multiplier[ElementID.Poison] *= 0.5f;
            }
            if (NPCLists.Undead.Contains(npc.type) || NPCLists.Skeleton.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.25f;
                multiplier[ElementID.Shadow] *= 0.8f;
            }
            if (NPCLists.Demon.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.3f;
                multiplier[ElementID.Celestial] *= 1.3f;
                multiplier[ElementID.Fire] *= 0.5f;
                multiplier[ElementID.Water] *= 1.15f;
                multiplier[ElementID.Ice] *= 1.15f;
            }
            if (NPCLists.Spirit.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.15f;
                multiplier[ElementID.Celestial] *= 1.15f;
                multiplier[ElementID.Arcane] *= 1.15f;
            }
            if (NPCLists.IsSlime.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Ice] *= 0.75f;
                multiplier[ElementID.Water] *= 0.5f;
            }
            if (NPCLists.Cold.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.25f;
                multiplier[ElementID.Ice] *= 0.75f;
                multiplier[ElementID.Thunder] *= 1.1f;
                multiplier[ElementID.Wind] *= 1.1f;
                multiplier[ElementID.Poison] *= 0.9f;
            }
            if (NPCLists.Hot.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 0.5f;
                multiplier[ElementID.Ice] *= 1.25f;
                multiplier[ElementID.Water] *= 1.1f;
                multiplier[ElementID.Wind] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.1f;
            }
            if (NPCLists.Wet.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 0.75f;
                multiplier[ElementID.Ice] *= 1.25f;
                multiplier[ElementID.Poison] *= 1.25f;
                multiplier[ElementID.Water] *= 0.5f;
            }
            if (NPCLists.Infected.Contains(npc.type))
            {
                multiplier[ElementID.Fire] *= 1.15f;
                multiplier[ElementID.Ice] *= 0.7f;
                multiplier[ElementID.Blood] *= 1.25f;
                multiplier[ElementID.Poison] *= 0.25f;
            }
            if (NPCLists.Robotic.Contains(npc.type))
            {
                multiplier[ElementID.Blood] *= 0.75f;
                multiplier[ElementID.Poison] *= 0.75f;
                multiplier[ElementID.Thunder] *= 1.1f;
                multiplier[ElementID.Water] *= 1.35f;
            }
            if (!NPCLists.Inorganic.Contains(npc.type))
            {
                multiplier[ElementID.Blood] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.05f;
            }
            if (NPCLists.Hallowed.Contains(npc.type))
            {
                multiplier[ElementID.Celestial] *= 0.9f;
                multiplier[ElementID.Holy] *= 0.5f;
                multiplier[ElementID.Shadow] *= 1.25f;
            }
            if (NPCLists.Dark.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.15f;
                multiplier[ElementID.Nature] *= 1.25f;
                multiplier[ElementID.Shadow] *= 0.75f;
            }
            if (NPCLists.Blood.Contains(npc.type))
            {
                multiplier[ElementID.Holy] *= 1.1f;
                multiplier[ElementID.Ice] *= 1.1f;
                multiplier[ElementID.Poison] *= 1.1f;
                multiplier[ElementID.Shadow] *= 0.9f;
                multiplier[ElementID.Blood] *= 0.75f;
            }
        }
        public static void ElementalEffects(Terraria.NPC npc, Terraria.Player player, Item item, ref float multiplier, ref Terraria.NPC.HitModifiers knockback)
        {
            if (item.HasElement(ElementID.Shadow) && NPCLists.Dark.Contains(npc.type) && player.RedemptionPlayerBuff().eldritchRoot)
                multiplier *= 1.33333f;

            if (item.HasElement(ElementID.Thunder) && ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)))
                multiplier *= 1.1f;
            if (item.HasElement(ElementID.Earth) && !npc.noTileCollide && npc.collideY)
                multiplier *= 1.1f;

            if (item.HasElement(ElementID.Poison) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                multiplier *= 1.1f;
            if (item.HasElement(ElementID.Wind) && (npc.noGravity || !npc.collideY))
            {
                knockback.Knockback *= 1.25f;
                if (npc.knockBackResist > 0)
                    knockback.Knockback.Flat += 2;
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
        public static void ElementalEffects(Terraria.NPC npc, Projectile proj, ref float multiplier, ref Terraria.NPC.HitModifiers knockback)
        {
            if (proj.HasElement(ElementID.Shadow) && NPCLists.Dark.Contains(npc.type) && Main.player[proj.owner].RedemptionPlayerBuff().eldritchRoot)
                multiplier *= 1.33333f;

            if (proj.HasElement(ElementID.Thunder) && ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet) || NPCLists.Wet.Contains(npc.type)))
                multiplier *= 1.1f;
            if (proj.HasElement(ElementID.Earth) && !npc.noTileCollide && npc.collideY)
                multiplier *= 1.1f;

            if (proj.HasElement(ElementID.Poison) && (npc.poisoned || npc.venom || npc.RedemptionNPCBuff().dirtyWound))
                multiplier *= 1.1f;
            if (proj.HasElement(ElementID.Wind) && (npc.noGravity || !npc.collideY))
            {
                knockback.Knockback *= 1.25f;
                if (npc.knockBackResist > 0)
                    knockback.Knockback.Flat += 2;
            }

            multiplier = (int)Math.Round(multiplier * 100);
            multiplier /= 100;
        }
    }
}

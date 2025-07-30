using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace Redemption
{
    public static class CustomSounds
    {
        public static void UpdateLoopingSound(ref SlotId slot, SoundStyle style, float volume, float pitch = 0f, Vector2? position = null)
        {
            SoundEngine.TryGetActiveSound(slot, out var sound);

            if (volume > 0f)
            {
                if (sound == null)
                {
                    slot = SoundEngine.PlaySound(style with { Volume = volume, Pitch = pitch }, position);
                    return;
                }

                sound.Position = position;
                sound.Volume = volume;
            }
            else if (sound != null)
            {
                sound.Stop();

                slot = SlotId.Invalid;
            }
        }
        public static readonly SoundStyle MaskBreak = new("Redemption/Sounds/Custom/MaskBreak") { PitchVariance = 0.3f };
        public static readonly SoundStyle AlarmItem = new("Redemption/Sounds/Custom/AlarmItem") { Volume = .5f, PitchVariance = .1f };
        public static readonly SoundStyle Alarm2 = new("Redemption/Sounds/Custom/Alarm2");
        public static readonly SoundStyle BallCreate = new("Redemption/Sounds/Custom/BallCreate") { PitchVariance = .1f };
        public static readonly SoundStyle BallFire = new("Redemption/Sounds/Custom/BallFire") { PitchVariance = .1f };
        public static readonly SoundStyle Banjo = new("Redemption/Sounds/Custom/Banjo", 3);
        public static readonly SoundStyle Bass1 = new("Redemption/Sounds/Custom/Bass1") { PitchVariance = .1f };
        public static readonly SoundStyle BulletBounce = new("Redemption/Sounds/Custom/BulletBounce", 3) { Volume = .3f, PitchVariance = .1f };
        public static readonly SoundStyle ChainSwing = new("Redemption/Sounds/Custom/ChainSwing");
        public static readonly SoundStyle ChickenCluck = new("Redemption/Sounds/Custom/ChickenCluck", 3) { PitchVariance = .1f };
        public static readonly SoundStyle Choir = new("Redemption/Sounds/Custom/Choir");
        public static readonly SoundStyle DistortedRoar = new("Redemption/Sounds/Custom/DistortedRoar") { Volume = .5f };
        public static readonly SoundStyle Doot = new("Redemption/Sounds/Custom/Doot") { PitchVariance = .3f };
        public static readonly SoundStyle EarthBoom = new("Redemption/Sounds/Custom/EarthBoom");
        public static readonly SoundStyle ElectricNoise = new("Redemption/Sounds/Custom/ElectricNoise");
        public static readonly SoundStyle ElectricSlash = new("Redemption/Sounds/Custom/ElectricSlash") { PitchVariance = .1f };
        public static readonly SoundStyle ElectricSlash2 = new("Redemption/Sounds/Custom/ElectricSlash2") { PitchVariance = .1f };
        public static readonly SoundStyle EnergyCharge = new("Redemption/Sounds/Custom/EnergyChargeSound");
        public static readonly SoundStyle EnergyCharge2 = new("Redemption/Sounds/Custom/EnergyChargeSound2");
        public static readonly SoundStyle FlyBuzz = new("Redemption/Sounds/Custom/FlyBuzz") { PitchVariance = .1f };
        public static readonly SoundStyle Gas1 = new("Redemption/Sounds/Custom/Gas1");
        public static readonly SoundStyle GigaLaserCharge = new("Redemption/Sounds/Custom/GigaLaserCharge") { Volume = 1.5f };
        public static readonly SoundStyle GigaLaserCoolDown = new("Redemption/Sounds/Custom/GigaLaserCoolDown") { Volume = 1.5f };
        public static readonly SoundStyle GigaLaserFire = new("Redemption/Sounds/Custom/GigaLaserFire") { Volume = 1.5f };
        public static readonly SoundStyle GravityHammerSlam = new("Redemption/Sounds/Custom/GravityHammerSlam") { Volume = 0.6f };
        public static readonly SoundStyle GrenadeLauncher = new("Redemption/Sounds/Custom/GrenadeLauncher") { Volume = .5f };
        public static readonly SoundStyle GuardBreak = new("Redemption/Sounds/Custom/GuardBreak") { Volume = .7f };
        public static readonly SoundStyle Gun1KS = new("Redemption/Sounds/Custom/Gun1KS") { Volume = 1.5f };
        public static readonly SoundStyle Gun2KS = new("Redemption/Sounds/Custom/Gun2KS") { Volume = 1.5f };
        public static readonly SoundStyle Gun3KS = new("Redemption/Sounds/Custom/Gun3KS") { Volume = 1.5f };
        public static readonly SoundStyle Gun1 = new("Redemption/Sounds/Custom/Gun1");
        public static readonly SoundStyle Gun2 = new("Redemption/Sounds/Custom/Gun2");
        public static readonly SoundStyle Gun3 = new("Redemption/Sounds/Custom/Gun3");
        public static readonly SoundStyle IceMist = new("Redemption/Sounds/Custom/IceMist") { PitchVariance = .1f };
        public static readonly SoundStyle LabSafeS = new("Redemption/Sounds/Custom/LabSafeS");
        public static readonly SoundStyle Laser1 = new("Redemption/Sounds/Custom/Laser1");
        public static readonly SoundStyle MACEProjectLaunch = new("Redemption/Sounds/Custom/MACEProjectLaunch") { PitchVariance = .1f };
        public static readonly SoundStyle MissileExplosion = new("Redemption/Sounds/Custom/MissileExplosion");
        public static readonly SoundStyle MissileFire1 = new("Redemption/Sounds/Custom/MissileFire1") { Volume = .8f, PitchVariance = .1f };
        public static readonly SoundStyle Muller1 = new("Redemption/Sounds/Custom/Muller1") { Volume = .9f, PitchVariance = .1f };
        public static readonly SoundStyle Muller2 = new("Redemption/Sounds/Custom/Muller2") { Volume = .9f, PitchVariance = .1f };
        public static readonly SoundStyle Muller3 = new("Redemption/Sounds/Custom/Muller3") { Volume = .9f, PitchVariance = .1f };
        public static readonly SoundStyle Muller4 = new("Redemption/Sounds/Custom/Muller4") { Volume = .9f, PitchVariance = .1f };
        public static readonly SoundStyle Muller5 = new("Redemption/Sounds/Custom/Muller5") { Volume = .9f, PitchVariance = .1f };
        public static readonly SoundStyle NebSound1 = new("Redemption/Sounds/Custom/NebSound1") { PitchVariance = .1f };
        public static readonly SoundStyle NebSound2 = new("Redemption/Sounds/Custom/NebSound2") { PitchVariance = .1f };
        public static readonly SoundStyle NebSound3 = new("Redemption/Sounds/Custom/NebSound3") { PitchVariance = .1f };
        public static readonly SoundStyle NukeExplosion = new("Redemption/Sounds/Custom/NukeExplosion") { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle PatientZeroLaser = new("Redemption/Sounds/Custom/PatientZeroLaser");
        public static readonly SoundStyle PatientZeroLaserL = new("Redemption/Sounds/Custom/PatientZeroLaserL");
        public static readonly SoundStyle PlasmaBlast = new("Redemption/Sounds/Custom/PlasmaBlast");
        public static readonly SoundStyle PlasmaShot = new("Redemption/Sounds/Custom/PlasmaShot");
        public static readonly SoundStyle PortalWub = new("Redemption/Sounds/Custom/PortalWub") { Volume = .5f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle Quake = new("Redemption/Sounds/Custom/Quake");
        public static readonly SoundStyle Reflect = new("Redemption/Sounds/Custom/Reflect") { Volume = .5f, PitchVariance = .1f };
        public static readonly SoundStyle ShootChange = new("Redemption/Sounds/Custom/ShootChange");
        public static readonly SoundStyle ShotgunBlastKS = new("Redemption/Sounds/Custom/ShotgunBlastKS") { Volume = 1.5f, PitchVariance = .1f };
        public static readonly SoundStyle ShotgunBlast1 = new("Redemption/Sounds/Custom/ShotgunBlast1") { PitchVariance = .1f };
        public static readonly SoundStyle Shriek = new("Redemption/Sounds/Custom/Shriek") { Volume = .4f };
        public static readonly SoundStyle Slam2 = new("Redemption/Sounds/Custom/Slam2") { Volume = .5f };
        public static readonly SoundStyle Slice1 = new("Redemption/Sounds/Custom/Slice1") { PitchVariance = .1f };
        public static readonly SoundStyle Slice2 = new("Redemption/Sounds/Custom/Slice2");
        public static readonly SoundStyle Slice3 = new("Redemption/Sounds/Custom/Slice3") { PitchVariance = .1f };
        public static readonly SoundStyle Slice4 = new("Redemption/Sounds/Custom/Slice4") { PitchVariance = .1f };
        public static readonly SoundStyle Slice5 = new("Redemption/Sounds/Custom/Slice5") { PitchVariance = .1f };
        public static readonly SoundStyle Slash1 = new("Redemption/Sounds/Custom/Slash1") { PitchVariance = .1f };
        public static readonly SoundStyle Slash2 = new("Redemption/Sounds/Custom/Slash2") { PitchVariance = .1f };
        public static readonly SoundStyle Spark1 = new("Redemption/Sounds/Custom/Spark1") { PitchVariance = .1f };
        public static readonly SoundStyle SpookyNoise = new("Redemption/Sounds/Custom/SpookyNoise");
        public static readonly SoundStyle Swing1 = new("Redemption/Sounds/Custom/Swing1") { Volume = .4f, PitchVariance = .1f };
        public static readonly SoundStyle Swoosh1 = new("Redemption/Sounds/Custom/Swoosh1") { Volume = .4f, PitchVariance = .1f };
        public static readonly SoundStyle Teleport1 = new("Redemption/Sounds/Custom/Teleport1") { Volume = .5f, PitchVariance = .1f };
        public static readonly SoundStyle Teleport2 = new("Redemption/Sounds/Custom/Teleport2") { PitchVariance = .1f };
        public static readonly SoundStyle Thunderstrike = new("Redemption/Sounds/Custom/Thunderstrike") { Volume = .7f, PitchVariance = .1f };
        public static readonly SoundStyle Transformation = new("Redemption/Sounds/Custom/Transformation") { Volume = .7f, PitchVariance = .1f };
        public static readonly SoundStyle VomitAttack = new("Redemption/Sounds/Custom/VomitAttack") { PitchVariance = .1f };
        public static readonly SoundStyle Zap1 = new("Redemption/Sounds/Custom/Zap1") { Volume = .5f, PitchVariance = .1f };
        public static readonly SoundStyle Zap2 = new("Redemption/Sounds/Custom/Zap2") { PitchVariance = .1f };
        public static readonly SoundStyle Voice1 = new("Redemption/Sounds/Custom/Voice1") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice2 = new("Redemption/Sounds/Custom/Voice2") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice3 = new("Redemption/Sounds/Custom/Voice3") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice4 = new("Redemption/Sounds/Custom/Voice4") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice5 = new("Redemption/Sounds/Custom/Voice5") { Volume = 0.7f, MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice6 = new("Redemption/Sounds/Custom/Voice6") { Volume = 0.4f, MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice7 = new("Redemption/Sounds/Custom/Voice7") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle Voice8 = new("Redemption/Sounds/Custom/Voice8") { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle MegaLaser = new("Redemption/Sounds/Custom/MegaLaser");
        public static readonly SoundStyle ObliteratorYo = new("Redemption/Sounds/Custom/ObliteratorYo") { Volume = 0.7f };
        public static readonly SoundStyle OODashReady = new("Redemption/Sounds/Custom/OODashReady") { Volume = 0.8f };
        public static readonly SoundStyle NewLocation = new("Redemption/Sounds/Custom/NewLocationSound");
        public static readonly SoundStyle ShieldActivate = new("Redemption/Sounds/Custom/GigaShieldActivate");
        public static readonly SoundStyle Launch2 = new("Redemption/Sounds/Custom/Launch2") { PitchVariance = .1f };
        public static readonly SoundStyle MACERoar = new("Redemption/Sounds/Custom/MaceRoar") { PitchVariance = .1f };
        public static readonly SoundStyle GigaFlame = new("Redemption/Sounds/Custom/GigaFlame") { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle WindLong = new("Redemption/Sounds/Custom/WindLong1") { Volume = .5f, PitchVariance = .1f };
        public static readonly SoundStyle Jyrina = new("Redemption/Sounds/Custom/Jyrina1") { PitchVariance = .2f };
        public static readonly SoundStyle Synth = new("Redemption/Sounds/Custom/SynthSound");
        public static readonly SoundStyle ElectricLoop = new("Redemption/Sounds/Custom/ElectricLoop") { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle FlameRise = new("Redemption/Sounds/Custom/FlameRise");
        public static readonly SoundStyle WindUp = new("Redemption/Sounds/Custom/WindUp") { PitchVariance = .1f };
        public static readonly SoundStyle Violin = new("Redemption/Sounds/Custom/Violin");
        public static readonly SoundStyle sans = new("Redemption/Sounds/Custom/sans") { Volume = .8f, PitchVariance = .1f };
        public static readonly SoundStyle BAZINGA = new("Redemption/Sounds/Custom/BAZINGA");
        public static readonly SoundStyle WorldTree = new("Redemption/Sounds/Custom/WorldTree");
        public static readonly SoundStyle DANShot = new("Redemption/Sounds/Custom/DANShot") { PitchVariance = .1f };
        public static readonly SoundStyle Pixie1 = new("Redemption/Sounds/Custom/Pixie1") { PitchVariance = .1f };
        public static readonly SoundStyle Pixie2 = new("Redemption/Sounds/Custom/Pixie2") { PitchVariance = .1f };
        public static readonly SoundStyle Pixie3 = new("Redemption/Sounds/Custom/Pixie3") { PitchVariance = .1f };
        public static readonly SoundStyle RoosterRoar = new("Redemption/Sounds/Custom/RoosterRoar") { PitchVariance = .1f };
        public static readonly SoundStyle Roar1 = new("Redemption/Sounds/Custom/Roar1") { PitchVariance = .1f };
        public static readonly SoundStyle NoitaDeath = new("Redemption/Sounds/Custom/NoitaDeath");
        public static readonly SoundStyle SwordClash = new("Redemption/Sounds/Custom/SwordClash") { PitchVariance = .1f };
        public static readonly SoundStyle AftonScream = new("Redemption/Sounds/Custom/AftonScream") { PitchVariance = .1f };
        public static readonly SoundStyle SoulfulSkeletonNotice = new("Redemption/Sounds/Custom/SoulfulSkeletonNotice") { PitchVariance = .3f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle SoulfulSkeletonAmbient = new("Redemption/Sounds/Custom/SoulfulSkeletonAmbient") { PitchVariance = .3f };
        public static readonly SoundStyle GreedySkeletonNotice = new("Redemption/Sounds/Custom/GreedySkeletonNotice") { PitchVariance = .3f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle GreedySkeletonAmbient = new("Redemption/Sounds/Custom/GreedySkeletonAmbient") { PitchVariance = .3f };
        public static readonly SoundStyle SkeletonNotice = new("Redemption/Sounds/Custom/SkeletonNotice", 3) { PitchVariance = .3f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle SkeletonAmbient = new("Redemption/Sounds/Custom/SkeletonAmbient", 2) { PitchVariance = .3f };
        public static readonly SoundStyle NebBeam = new("Redemption/Sounds/Custom/NebBeam");
        public static readonly SoundStyle NebMeteor = new("Redemption/Sounds/Custom/NebMeteor");
        public static readonly SoundStyle MetalExplosion = new("Redemption/Sounds/Custom/MetalExplosion");
        public static readonly SoundStyle FireExplosion = new("Redemption/Sounds/Custom/FireExplosion");
        public static readonly SoundStyle ThunderExplosion = new("Redemption/Sounds/Custom/ThunderExplosion");
        public static readonly SoundStyle Shatter = new("Redemption/Sounds/Custom/Shatter");
        public static readonly SoundStyle HLShotgun1 = new("Redemption/Sounds/Custom/HLShotgun1") { PitchVariance = .1f };
        public static readonly SoundStyle Bell = new("Redemption/Sounds/Custom/Bell") { PitchVariance = .1f, Pitch = -.5f, Volume = .5f };
        public static readonly SoundStyle Kantele1 = new("Redemption/Sounds/Custom/Kantele1");
        public static readonly SoundStyle Kantele2 = new("Redemption/Sounds/Custom/Kantele2");
        public static readonly SoundStyle LegoBreak = new("Redemption/Sounds/Custom/LegoBreak") { Volume = .6f };
        public static readonly SoundStyle MeatImpact = new("Redemption/Sounds/Custom/MeatImpact") { PitchVariance = .1f };

        public static readonly SoundStyle Magic1 = new("Redemption/Sounds/Custom/Magic1") { PitchVariance = .1f };
        public static readonly SoundStyle Magic2 = new("Redemption/Sounds/Custom/Magic2") { PitchVariance = .1f };
        public static readonly SoundStyle Magic5 = new("Redemption/Sounds/Custom/Magic5") { PitchVariance = .1f };
        public static readonly SoundStyle Saint1 = new("Redemption/Sounds/Custom/Saint1") { PitchVariance = .2f };
        public static readonly SoundStyle Saint2 = new("Redemption/Sounds/Custom/Saint2");
        public static readonly SoundStyle Saint3 = new("Redemption/Sounds/Custom/Saint3") { PitchVariance = .1f };
        public static readonly SoundStyle Saint4 = new("Redemption/Sounds/Custom/Saint4");
        public static readonly SoundStyle Saint9 = new("Redemption/Sounds/Custom/Saint9") { PitchVariance = .1f };
        public static readonly SoundStyle Thorn1 = new("Redemption/Sounds/Custom/ThornSound1") { PitchVariance = .2f, Volume = .8f };
        public static readonly SoundStyle WailingSoul = new("Redemption/Sounds/Custom/WailingSoul", 2) { PitchVariance = .1f };
        public static readonly SoundStyle HeavyMagic1 = new("Redemption/Sounds/Custom/HeavyMagic1") { PitchVariance = .1f };
        public static readonly SoundStyle HeavyExplosion1 = new("Redemption/Sounds/Custom/Explosion1") { PitchVariance = .1f };
        public static readonly SoundStyle Ghost1 = new("Redemption/Sounds/Custom/WailingSoul3") { PitchVariance = .1f };
        public static readonly SoundStyle Ghost2 = new("Redemption/Sounds/Custom/WailingSoul4") { PitchVariance = .1f };
        public static readonly SoundStyle Ghost3 = new("Redemption/Sounds/Custom/Ghost", 2) { PitchVariance = .4f, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle FlameRise2 = new("Redemption/Sounds/Custom/FlameRise2") { PitchVariance = .1f };

        public static readonly SoundStyle BoneHit = new("Redemption/Sounds/Tiles/BoneHit", 3);
        public static readonly SoundStyle BrickHit = new("Redemption/Sounds/Tiles/BrickHit", 3);
        public static readonly SoundStyle ChainHit = new("Redemption/Sounds/Tiles/ChainHit", 3);
        public static readonly SoundStyle CrystalHit = new("Redemption/Sounds/Tiles/CrystalHit", 3);
        public static readonly SoundStyle MetalHit = new("Redemption/Sounds/Tiles/MetalHit", 3);
        public static readonly SoundStyle DragonLeadHit = new("Redemption/Sounds/Tiles/DragonLeadHit", 3);
        public static readonly SoundStyle StoneHit = new("Redemption/Sounds/Tiles/StoneHit1") { PitchVariance = .1f };
    }
}
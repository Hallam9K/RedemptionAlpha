using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class PlutoniumBeam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plutonium Beam");
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 1400;
            Projectile.penetrate = 20;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Redemption().EnergyBased = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ >= 10f && Projectile.localAI[0] % 10 == 0)
            {
                Vector2 drawPos = Projectile.Center;
                RedeParticleManager.CreateLaserParticle(drawPos, Projectile.velocity, 2f, Color.LightCyan);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.LightBlue, glowScale: 3);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast, Projectile.position);
            player.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            player.RedemptionScreen().ScreenShakeIntensity += 2;
        }
    }
}

using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class CorruptedDoubleRifle_Beam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Beam");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 700;
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.Redemption().EnergyBased = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ > 30 && !Main.rand.NextBool(6))
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 v = Projectile.position;
                    v -= Projectile.velocity * (i * 0.25f);
                    ParticleManager.NewParticle(v, Vector2.Zero, new LightningParticle(), Color.White, 1, 2);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed, glowScale: 3);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast with { Volume = 0.5f }, Projectile.position);
            player.RedemptionScreen().ScreenShakeIntensity += 3;
        }
    }
}

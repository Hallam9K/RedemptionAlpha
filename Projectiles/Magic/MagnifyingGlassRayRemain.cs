using Microsoft.Xna.Framework;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class MagnifyingGlassRayRemain : ModProjectile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scorching Ray");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.height = 10;
            Projectile.width = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
        public override void AI()
        {
            if (Main.dayTime)
            {
                if (Projectile.timeLeft % 10 == 0)
                    ParticleSystem.NewParticle<EmberParticleRemain>(Projectile.Bottom, RedeHelper.PolarVector(0, 0), Projectile.GetAlpha(Color.White), 1);
                if (Projectile.timeLeft % 30 == 0)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.PolarVector(0, 0), new EmberParticle(), Color.White, .6f);
            }
            else
            {
                if (Projectile.timeLeft % 10 == 0)
                    ParticleSystem.NewParticle<BlueEmberParticleRemain>(Projectile.Bottom, RedeHelper.PolarVector(0, 0), Projectile.GetAlpha(Color.White), 1);
                if (Projectile.timeLeft % 30 == 0)
                    ParticleSystem.NewParticle<BlueEmberParticle>(Projectile.Center, RedeHelper.PolarVector(0, 0), Color.White, .6f);
            }
        }
    }
}

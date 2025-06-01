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
                    RedeParticleManager.CreateEmberBurstParticle(Projectile.Bottom, Vector2.Zero, 1, Main.rand.Next(90, 121));
                if (Projectile.timeLeft % 30 == 0)
                    RedeParticleManager.CreateEmberParticle(Projectile.Center, Vector2.Zero, .6f, Main.rand.Next(90, 121));
            }
            else
            {
                if (Projectile.timeLeft % 10 == 0)
                    RedeParticleManager.CreateEmberBurstParticle(Projectile.Bottom, Vector2.Zero, 1, RedeParticleManager.blueEmberColors, Main.rand.Next(90, 121));
                if (Projectile.timeLeft % 30 == 0)
                    RedeParticleManager.CreateEmberParticle(Projectile.Center, Vector2.Zero, .6f, RedeParticleManager.blueEmberColors, Main.rand.Next(90, 121));
            }
        }
    }
}

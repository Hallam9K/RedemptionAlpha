using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class EnergySphereSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Sphere");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjThunder[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1600;
            Projectile.extraUpdates = 6;
            DrawOffsetX = -6;
            DrawOriginOffsetY = -6;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] is 1)
            {
                Projectile.width = 28;
                Projectile.height = 28;
                if (++Projectile.frameCounter >= 5 * 6)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 4)
                        Projectile.Kill();
                }
                return;
            }
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<EnergySphereDust>());
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= .5f;
            dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowingLanceDust>());
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= .2f;
            Projectile.velocity.Y += 0.005f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.localAI[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
                Projectile.velocity *= 0;
                Projectile.localAI[1] = 1;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            if (Projectile.localAI[0]++ > 1)
            {
                SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
                Projectile.velocity *= 0;
                Projectile.localAI[1] = 1;
            }
            return false;
        }
    }
}
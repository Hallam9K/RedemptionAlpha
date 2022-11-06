using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Particles;

namespace Redemption.Projectiles.Magic
{
    public class CantripEmberS : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ember");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 400;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(253, 221, 3), new Color(253, 62, 3)), new RoundCap(), new DefaultTrailPosition(), 100f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            if (Projectile.wet && !Projectile.lavaWet)
                Projectile.Kill();

            int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.4f);
            Main.dust[dust].noGravity = true;
            Color dustColor = new(253, 221, 3) { A = 0 };
            Main.dust[dust].color = dustColor;
            int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.4f);
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(253, 62, 3) { A = 0 };
            Main.dust[dust2].color = dustColor2;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            for (int i = 0; i < 24; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust2].velocity *= 10f;
                Main.dust[dust2].noGravity = true;
            }
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (!Projectile.wet)
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (int i = 0; i < 6; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);

            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.4f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(253, 221, 3) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.4f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(253, 62, 3) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            for (int i = 0; i < 24; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust2].velocity *= 10f;
                Main.dust[dust2].noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.AddBuff(BuffID.OnFire, 280);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.AddBuff(BuffID.OnFire, 280);
        }
    }
}
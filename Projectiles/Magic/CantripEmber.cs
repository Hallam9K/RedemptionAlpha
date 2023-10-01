using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Effects;
using System.Collections.Generic;
using Redemption.Projectiles.Melee;
using Terraria.Audio;

namespace Redemption.Projectiles.Magic
{
    public class CantripEmber : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ember");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FakeKill();
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }

        private readonly int NUMPOINTS = 20;
        public Color baseColor = new(253, 221, 3);
        public Color endColor = new(253, 62, 3);
        public Color edgeColor = new(253, 221, 3);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 1.4f;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            return true;
        }
        public override void AI()
        {
            if (Projectile.wet && !Projectile.lavaWet)
                FakeKill();

            if (Projectile.alpha < 255)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(253, 221, 3) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(253, 62, 3) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        public int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                if (!Projectile.wet && Projectile.type == ModContent.ProjectileType<Firebreak_Proj>())
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = .4f }, Projectile.position);

                for (int i = 0; i < 3; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);

                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(253, 221, 3) { A = 0 };
                    Main.dust[dust].color = dustColor;
                    int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                    Main.dust[dust2].noGravity = true;
                    Color dustColor2 = new(253, 62, 3) { A = 0 };
                    Main.dust[dust2].color = dustColor2;
                }
                for (int i = 0; i < 12; i++)
                {
                    int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                    Main.dust[dust2].velocity *= 5f;
                    Main.dust[dust2].noGravity = true;
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            for (int i = 0; i < 3; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);

            for (int i = 0; i < 6; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(253, 221, 3) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(253, 62, 3) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            for (int i = 0; i < 12; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust2].velocity *= 5f;
                Main.dust[dust2].noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 160);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 160);
        }
    }
}
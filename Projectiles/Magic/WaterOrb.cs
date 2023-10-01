using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects;
using System.Collections.Generic;
using Redemption.Globals;

namespace Redemption.Projectiles.Magic
{
    public class WaterOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjWater[Type] = true;
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FakeKill();
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }

        private readonly int NUMPOINTS = 50;
        public Color baseColor = new(95, 220, 214);
        public Color endColor = new(34, 78, 146);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 6f;

        float rot = 0.02f;
        public override void AI()
        {
            Projectile.ai[1] += rot;
            if (Projectile.ai[1] > (Projectile.localAI[0] == 0 ? 0.10666f : 0.16f))
            {
                Projectile.localAI[0] = 1;
                rot = -0.02f;
            }
            else if (Projectile.ai[1] < -0.16f)
            {
                rot = 0.02f;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] == 0 ? Projectile.ai[1] : -Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.timeLeft -= 4;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                SoundEngine.PlaySound(SoundID.Item21 with { Volume = 0.5f }, Projectile.position);
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemSapphire, 0, 0, Scale: 3);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].noGravity = true;
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
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            return true;
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            SoundEngine.PlaySound(SoundID.Item21 with { Volume = 0.5f }, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemSapphire, 0, 0, Scale: 3);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }           
    }
}
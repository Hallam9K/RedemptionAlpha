using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_EnergyBolt : ModRedeProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Bolt");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Redemption().EnergyBased = true;
        }

        private readonly int NUMPOINTS = 16;
        public Color baseColor = new(140, 255, 242);
        public Color endColor = new(194, 255, 242);
        public Color edgeColor = new(158, 56, 248);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 2f;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width / 2, Projectile.height / 2);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + (Projectile.velocity / 2) - Vector2.One, 2, 2, DustType<GlowDust>());
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= .2f;
                    Color dustColor = new(194, 255, 242) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer++ >= 60)
                Projectile.Kill();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0]++ == 0)
                Projectile.velocity *= 3;
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = new(140, 255, 242);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale + .5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * 0.4f, Projectile.rotation, origin, Projectile.scale + 1f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class LunarShot_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lunar Bolt");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjNature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
        }
        private readonly int NUMPOINTS = 14;
        public Color baseColor = new(250, 205, 160);
        public Color endColor = new(255, 255, 218);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 3f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FakeKill();
            Player player = Main.player[Projectile.owner];
            if (!Main.dayTime && Main.moonPhase != 4 && Main.myPlayer == player.whoAmI) {
                if (Main.moonPhase == 0) 
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(8, RedeHelper.RandomRotation()), ModContent.ProjectileType<MoonflareBatIllusion>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack, Main.myPlayer, target.whoAmI);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(8, RedeHelper.RandomRotation()), ModContent.ProjectileType<MoonflareBatIllusion>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
                DustHelper.DrawCircle(Projectile.Center, ModContent.DustType<MoonflareDust>(), 1, 2, 2, nogravity: true);

            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            DustHelper.DrawCircle(Projectile.Center, ModContent.DustType<MoonflareDust>(), 1, 2, 2, nogravity: true);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity *= 2;
                Projectile.localAI[0] = 1;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return true;
        }
    }
}
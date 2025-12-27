using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Effects;
using Redemption.Globals;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class Hardlight_SoSMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SoS Missile");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        private readonly int NUMPOINTS = 30;
        public Color baseColor = new(193, 255, 219);
        public Color endColor = new(0, 160, 170);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 8f;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];
            if (!projAim.active || projAim.type != ProjectileType<Hardlight_SoSCrosshair>())
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<Hardlight_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] > 20)
                Projectile.Move(projAim.Center, 30, 10);

            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (Projectile != proj && proj.identity == projAim.identity)
                {
                    proj.localAI[0]++;
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<Hardlight_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                    Projectile.Kill();
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<Hardlight_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
            Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion with { PitchVariance = .1f }, Projectile.position);

            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, Scale: 2f);
                Main.dust[dustIndex].velocity *= 6f;
                Main.dust[dustIndex].noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 2f);
                Main.dust[dustIndex].velocity *= 6f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/Trail_1").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }
    }
    public class Hardlight_MissileBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 224;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        private float BoomGlowTimer;
        private bool BoomGlow;
        public override void AI()
        {
            if (!BoomGlow)
            {
                BoomGlowTimer += 2;
                if (BoomGlowTimer > 60)
                {
                    BoomGlow = true;
                    BoomGlowTimer = 0;
                }
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.Kill();
            }
            Projectile.scale += 0.02f;
        }
        public override bool? CanHitNPC(NPC target) => Projectile.frame > 2 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 60;
            target.immune[Projectile.owner] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D teleportGlow = Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(Color.White, Color.LightCyan, 1f / BoomGlowTimer * 10f) * (1f / BoomGlowTimer * 10f);
            if (!BoomGlow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, 3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.3f, Projectile.rotation, origin2, 8f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}

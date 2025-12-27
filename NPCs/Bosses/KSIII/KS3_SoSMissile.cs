using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Projectiles;
using Redemption.Textures;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_SoSMissile : ModRedeProjectile
    {
        public override string Texture => "Redemption/Projectiles/Ranged/Hardlight_SoSMissile";
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
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
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
            NPC host = Main.npc[(int)Projectile.ai[2]];

            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];
            if (!projAim.active || projAim.type != ProjectileType<KS3_SoSCrosshair>() || !host.active || host.ai[0] is 9 or 20)
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;

                Projectile.NewProjectile(Terraria.Entity.InheritSource(Main.npc[(int)Projectile.ai[2]]), Projectile.Center, Vector2.Zero, ProjectileType<KS3_MissileBlast>(), Projectile.damage, 0, Main.myPlayer, ai2: Main.npc[(int)Projectile.ai[2]].whoAmI);
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] > 20 && !Projectile.reflected)
                Projectile.Move(projAim.Center, 30, 60);

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion with { PitchVariance = .1f }, Projectile.position);

            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<KS3_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);

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
    public class KS3_MissileBlast : ModRedeProjectile
    {
        public override string Texture => "Redemption/Projectiles/Ranged/Hardlight_MissileBlast";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 224;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
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
        public override bool CanHitPlayer(Player target) => Projectile.frame > 2;
        public override bool? SafeCanHitNPC(NPC target) => Projectile.frame > 2 ? null : false;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            int height = texture.Height() / 5;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width(), height);
            Vector2 origin = new(texture.Width() / 2f, height / 2f);

            Main.EntitySpriteDraw(texture.Value, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White with { A = 0 }), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Asset<Texture2D> teleportGlow = CommonTextures.WhiteGlow;
            Rectangle rect2 = new(0, 0, teleportGlow.Width(), teleportGlow.Height());
            Vector2 origin2 = new(teleportGlow.Width() / 2, teleportGlow.Height() / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(Color.White, Color.LightCyan, 1f / BoomGlowTimer * 10f) * (1f / BoomGlowTimer * 10f);
            if (!BoomGlow)
            {
                Main.EntitySpriteDraw(teleportGlow.Value, position2, new Rectangle?(rect2), colour2 with { A = 0 }, Projectile.rotation, origin2, 3f, 0, 0);
                Main.EntitySpriteDraw(teleportGlow.Value, position2, new Rectangle?(rect2), colour2 with { A = 0 } * 0.3f, Projectile.rotation, origin2, 8f, 0, 0);
            }
        }
    }
}
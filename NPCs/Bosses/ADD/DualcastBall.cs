using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Effects;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class DualcastBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Static Dualcast");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        private readonly int NUMPOINTS = 60;
        public Color baseColor = Color.White;
        public Color endColor = new(241, 215, 108);
        public Color edgeColor = Color.White;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 14f;

        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 1f, Projectile.Opacity * 0.8f, Projectile.Opacity * 0.6f);

            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (Projectile.ai[0] == 0)
            {
                if (offsetLeft)
                {
                    vectorOffset -= 0.06f;
                    if (vectorOffset <= -2.4f)
                    {
                        vectorOffset = -2.4f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.06f;
                    if (vectorOffset >= 2.4f)
                    {
                        vectorOffset = 2.4f;
                        offsetLeft = true;
                    }
                }
            }
            else
            {
                if (offsetLeft)
                {
                    vectorOffset += 0.06f;
                    if (vectorOffset >= 2.4f)
                    {
                        vectorOffset = 2.4f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset -= 0.06f;
                    if (vectorOffset <= -2.4f)
                    {
                        vectorOffset = -2.4f;
                        offsetLeft = true;
                    }
                }
            }
            float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
            Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));

            Projectile.rotation = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + Projectile.velocity) + 1.57f - MathHelper.PiOver4;
            Projectile.spriteDirection = 1;
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
            if (Projectile.timeLeft < 2)
            {
                Projectile.timeLeft = 2;
                FakeKill();
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Lightning2").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return true;
        }
        private int fakeTimer;
        private void FakeKill()
        {
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer++ >= 60)
                Projectile.Kill();
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
            }
            target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 60);
        }
    }
}

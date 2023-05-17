using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class Keeper_SoullessDusksong : ModProjectile
    {
        public override string Texture => "Redemption/Textures/DarkSoulTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Soul");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 122;
            Projectile.height = 122;
            Projectile.timeLeft = 300;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.localAI[0] = 1;
        }
        private readonly int NUMPOINTS = 70;
        public Color baseColor = new(210, 200, 191);
        public Color endColor = Color.Black;
        public Color edgeColor = new(111, 115, 124);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 20f;

        private int CD;
        public override void AI()
        {
            CD--;
            if (Projectile.alpha > 40)
                Projectile.alpha -= 10;
            Projectile.rotation += 0.14f;

            if (Projectile.scale <= .1f)
                FakeKill();

            if (Projectile.scale > Projectile.localAI[0])
                Projectile.scale -= .02f;
            flareScale += Main.rand.NextFloat(-.02f, .02f);
            flareScale = MathHelper.Clamp(flareScale, .9f, 1.1f);
            flareOpacity += Main.rand.NextFloat(-.1f, .1f);
            flareOpacity = MathHelper.Clamp(flareOpacity, 0.6f, 0.8f);

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(210, 200, 191), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(111, 115, 124), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
                SoundEngine.PlaySound(SoundID.NPCDeath51 with { Pitch = -.5f }, Projectile.position);
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dust].velocity *= 2;
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
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (CD > 0)
                return;
            for (int k = 0; k < 40; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 61 * Projectile.scale);
                vector.Y = (float)(Math.Cos(angle) * 61 * Projectile.scale);
                ParticleManager.NewParticle(Projectile.Center + vector, (Projectile.Center + vector).DirectionTo(Projectile.Center) * 5f, new GlowParticle2(), new Color(210, 200, 191), 2 * Projectile.scale, .45f, Main.rand.Next(10, 20));
                ParticleManager.NewParticle(Projectile.Center + vector, (Projectile.Center + vector).DirectionTo(Projectile.Center) * 5f, new GlowParticle2(), new Color(111, 115, 124), 2 * Projectile.scale, .45f, Main.rand.Next(10, 20));
            }
            Projectile.localAI[0] -= 0.2f;
            SoundEngine.PlaySound(SoundID.Item103, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(.4f) * Main.rand.NextFloat(.9f, 1.1f), ModContent.ProjectileType<ShadowBolt_Soulless>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
            CD = 5;
        }
        private float flareScale;
        private float flareOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Redemption.GlowTrail.Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.ShadowDye);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawOrigin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(61, 61);
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.6f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.4f, -Projectile.rotation, origin, Projectile.scale * 1.4f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteEyeFlare").Value;
            Rectangle rect2 = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);

            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity, 0, origin2, Projectile.scale + flareScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity * 0.75f, 0, origin2, (Projectile.scale + flareScale) * 1.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * flareOpacity * 0.5f, 0, origin2, (Projectile.scale + flareScale) * 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(210, 200, 191), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(111, 115, 124), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
            SoundEngine.PlaySound(SoundID.NPCDeath51 with { Pitch = -.5f }, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<VoidFlame>(), Scale: 2);
                Main.dust[dust].velocity *= 2;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}

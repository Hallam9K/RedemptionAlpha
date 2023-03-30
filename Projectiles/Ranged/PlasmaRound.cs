using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Effects;
using System.Collections.Generic;

namespace Redemption.Projectiles.Ranged
{
    public class PlasmaRound : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plasma Round");
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.Redemption().EnergyBased = true;
        }
        private readonly int NUMPOINTS = 16;
        public Color baseColor = new(255, 182, 49);
        public Color endColor = new(255, 212, 140);
        public Color edgeColor = new(158, 56, 248);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 4f;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width / 2, Projectile.height / 2);
            BlastSpawn(oldVelocity / 2);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate <= 1)
                FakeKill();
            BlastSpawn(Projectile.velocity / 3);
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
        private void BlastSpawn(Vector2 vel)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel, Vector2.Zero, ModContent.ProjectileType<PlasmaRound_Blast>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

            DustHelper.DrawCircle(Projectile.Center + vel, DustID.OrangeTorch, 1, 2, 2, nogravity: true);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast, Projectile.position);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
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
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, new Color(255, 182, 49), 1f / Projectile.localAI[0] * 10f) * (1f / Projectile.localAI[0] * 10f);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour), Projectile.rotation, origin, 1.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * 0.4f, Projectile.rotation, origin, 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class PlasmaRound_Blast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 6;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().EnergyBased = true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.frame < 5 ? null : false;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 5;
            target.immune[Projectile.owner] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.6f, Projectile.rotation, origin, Projectile.scale + 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
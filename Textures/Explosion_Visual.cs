using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Textures
{
    public class Explosion_Visual : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
        }

        private float GlowTimer;
        private bool Glow;
        public Color color;
        public int dustID;
        public float dustScale;
        public float scale;
        public bool noDust;
        public Texture2D texture;
        public override void AI()
        {
            if (Glow)
            {
                GlowTimer += 3;
                if (GlowTimer > 60)
                {
                    Glow = false;
                    GlowTimer = 0;
                }
            }
            if (Projectile.localAI[0]++ == 0)
            {
                Glow = true;
                Projectile.alpha = 255;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += Projectile.ai[0];
                if (!noDust)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), Scale: 2);
                        Main.dust[dust].velocity *= 6;
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(color.R, color.G, color.B) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    for (int i = 0; i < Projectile.ai[1]; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: dustScale);
                        Main.dust[dust].velocity *= 10;
                        Main.dust[dust].noGravity = true;
                    }
                    for (int i = 0; i < Projectile.ai[1]; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: dustScale);
                        Main.dust[dust].velocity *= 15;
                        Main.dust[dust].noGravity = true;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int g = 0; g < 6; g++)
                        {
                            int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 2f;
                        }
                    }
                }
            }
            if (Projectile.localAI[0] >= 20)
                Projectile.Kill();
        }
        public override void PostDraw(Color lightColor)
        {
            if (texture == null)
                texture = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = texture;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(color, color, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (Glow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}

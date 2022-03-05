using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolyHandGrenadeOfAnglon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Hand Grenade of Anglon");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
        }

        private float TeleGlowTimer;
        private bool TeleGlow;
        public override void AI()
        {
            if (TeleGlow)
            {
                TeleGlowTimer += 3;
                if (TeleGlowTimer > 60)
                {
                    TeleGlow = false;
                    TeleGlowTimer = 0;
                }
            }

            if (Projectile.localAI[0]++ == 0)
            {
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f, 0.85f, 4);
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f);
            }

            if (Projectile.localAI[0] == 200)
            {
                SoundEngine.PlaySound(SoundID.Tink);
                Projectile.frame = 1;
                if (Main.netMode != NetmodeID.Server)
                    Gore.NewGore(Projectile.position + new Vector2(13, 2), RedeHelper.SpreadUp(5),
                        ModContent.Find<ModGore>("Redemption/HolyGrenadePin").Type);
            }
            if (Projectile.localAI[0] == 320)
            {
                TeleGlow = true;
                Projectile.alpha = 255;
                Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 20;
                SoundEngine.PlaySound(SoundID.Item14);
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), Scale: 2);
                    Main.dust[dust].velocity *= 6;
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(255, 216, 0) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 15;
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].velocity *= 20;
                    Main.dust[dust].noGravity = true;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int g = 0; g < 6; g++)
                    {
                        int goreIndex = Gore.NewGore(Projectile.Center, default, Main.rand.Next(61, 64));
                        Main.gore[goreIndex].scale = 1.5f;
                        Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                        Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                    }
                }
            }
            if (Projectile.localAI[0] >= 380)
                Projectile.Kill();
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(Color.White, Color.Orange, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, 4f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Projectile.rotation, origin2, 4f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}

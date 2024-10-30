using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

        public ref float ShakeAmount => ref Projectile.ai[0];
        public ref float DustAmount => ref Projectile.ai[1];
        public int DustType
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public string TexturePath = string.Empty;
        public Color Color;
        public float DustScale;
        public float Scale;
        public bool NoDust;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TexturePath);
            writer.Write(Color.PackedValue);
            writer.Write(DustScale);
            writer.Write(Scale);
            writer.Write(NoDust);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TexturePath = reader.ReadString();
            Color.PackedValue = reader.ReadUInt32();
            DustScale = reader.ReadSingle();
            Scale = reader.ReadSingle();
            NoDust = reader.ReadBoolean();
        }

        private float glowTimer;
        private bool glow;

        public override void AI()
        {
            if (glow)
            {
                glowTimer += 3;
                if (glowTimer > 60)
                {
                    glow = false;
                    glowTimer = 0;
                }
            }
            if (Projectile.localAI[0]++ == 0)
            {
                glow = true;
                Projectile.alpha = 255;
                if (ShakeAmount > 0)
                {
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += ShakeAmount;
                }
                if (!NoDust)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), Scale: 2);
                        Main.dust[dust].velocity *= 6;
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(Color.R, Color.G, Color.B) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    for (int i = 0; i < DustAmount; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: DustScale);
                        Main.dust[dust].velocity *= 10;
                        Main.dust[dust].noGravity = true;
                    }
                    for (int i = 0; i < DustAmount; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: DustScale);
                        Main.dust[dust].velocity *= 15;
                        Main.dust[dust].noGravity = true;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int g = 0; g < 6; g++)
                        {
                            int goreIndex = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, default, Main.rand.Next(61, 64));
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
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D teleportGlow = string.IsNullOrEmpty(TexturePath) ?
                ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value :
                ModContent.Request<Texture2D>(TexturePath).Value;

            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(Color, Color, 1f / glowTimer * 10f) * (1f / glowTimer * 10f);
            if (glow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Projectile.rotation, origin2, Scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}

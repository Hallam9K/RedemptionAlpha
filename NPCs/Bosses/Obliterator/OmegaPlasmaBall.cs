using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.NPCs.Bosses.Cleaver;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OmegaPlasmaBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Plasma Orb");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1600;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1 * Projectile.Opacity, 0.3f * Projectile.Opacity, 0.3f * Projectile.Opacity);
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.localAI[0] == 0)
            {
                RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed);
                Projectile.localAI[0] = 1;
                Projectile.scale = 0.1f;
            }
            Projectile.scale += 0.02f;
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.1f, 1);
            Projectile.velocity *= 0.98f;
            if (Projectile.scale >= 1)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.type == Type || proj.hostile || !proj.friendly || proj.damage < 5)
                        continue;

                    if (!Projectile.Hitbox.Intersects(proj.Hitbox) || proj.ProjBlockBlacklist())
                        continue;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.BallFire, Projectile.position);
                    DustHelper.DrawCircle(proj.Center, DustID.LifeDrain, 1, 4, 4, nogravity: true);
                    for (int j = 0; j < 4; j++)
                        Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<OmegaBlast>(), 110, RedeHelper.PolarVector(Main.rand.NextFloat(9, 19), (Main.player[RedeHelper.GetNearestAlivePlayer(Projectile)].Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), false, SoundID.Item1);
                    proj.Kill();
                    Projectile.Kill();
                }
            }
            Vector2 move = Vector2.Zero;
            float distance = 50f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.player[k].active && !Main.player[k].dead)
                {
                    Vector2 newMove = Main.player[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                AdjustMagnitude(ref Projectile.velocity);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(4, 4), ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: 3);
            dust2.noGravity = true;
            Color dustColor = new(Color.IndianRed.R, Color.IndianRed.G, Color.IndianRed.B) { A = 0 };
            dust2.color = dustColor;

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain);
                dust.velocity = -Projectile.DirectionTo(dust.position) * 2f;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f) * Projectile.Opacity;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, position, new Rectangle?(rect), RedeColor.RedPulse * 0.3f, Projectile.rotation, origin, Projectile.scale);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
                vector *= 8f / magnitude;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;

namespace Redemption.Projectiles.Magic
{
    public class Hardlight_ManaDrone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mana Drone Mk.I");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            float soundVolume = Projectile.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, Projectile.position);
                Projectile.soundDelay = 10;
            }

            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0)
            {
                DustHelper.DrawCircle(Projectile.Center, DustID.Frost, 2, 2, 2, 1, 2, nogravity: true);
                Projectile.localAI[0] = 1;
            }
            if (Projectile.timeLeft > 120)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.Move(new Vector2(player.Center.X + 100, player.Center.Y - 80), 14, 20);

                    if (Projectile.localAI[1]++ >= 60 && Projectile.localAI[1] <= 720)
                    {
                        if (++Projectile.ai[1] >= 5)
                        {
                            Projectile.ai[1] = 0;
                            if (Projectile.ai[0] > 3)
                            {
                                for (int i = 0; i < 10; ++i)
                                {
                                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                                    dust.velocity = -player.DirectionTo(dust.position) * 10;
                                    dust.noGravity = true;
                                }
                                int steps = (int)Projectile.Distance(player.Center) / 8;
                                for (int i = 0; i < steps; i++)
                                {
                                    if (Main.rand.NextBool(2))
                                    {
                                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(Projectile.Center, player.Center, (float)i / steps), 2, 2, DustID.ManaRegeneration);
                                        dust.velocity = -player.DirectionTo(dust.position) * 2;
                                        dust.noGravity = true;
                                    }
                                }
                                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
                                player.ManaEffect(25);
                                player.statMana += 25;
                            }
                            if (++Projectile.ai[0] >= 8)
                                Projectile.ai[0] = 0;
                        }
                    }
                    else if (Projectile.localAI[1] > 720)
                        Projectile.timeLeft = 120;
                }
            }
            else
            {
                Projectile.velocity.Y -= 0.4f;
                Projectile.velocity.X *= 0.9f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D meter = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Meter").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            int height2 = meter.Height / 8;
            int y2 = (int)(height2 * Projectile.ai[0]);
            Rectangle rect2 = new(0, y2, meter.Width, height2);
            Vector2 drawOrigin2 = new(meter.Width / 2, meter.Height / 2);
            Main.EntitySpriteDraw(meter, Projectile.Center - Main.screenPosition + new Vector2(0, 82), new Rectangle?(rect2), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin2, Projectile.scale, effects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 4.4f;
            }
        }
    }
}
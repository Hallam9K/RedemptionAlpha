using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Erhan_Bible : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Magic/HolyBible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Bible");
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        private bool openBook;
        private float godrayFade = 1;
        private int TimerRand;
        private Vector2 playerOrigin;
        public ref float AITimer => ref Projectile.localAI[1];
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Player player = Main.player[host.target];
            if (!host.active || (host.type != ModContent.NPCType<Erhan>() && host.type != ModContent.NPCType<ErhanSpirit>()))
                Projectile.Kill();
            drawTimer++;
            Projectile.timeLeft = 10;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.alpha -= 4;
                    if (Projectile.alpha <= 0)
                    {
                        Projectile.localAI[0]++;
                    }
                    break;
                case 1:
                    Projectile.velocity *= 0.98f;
                    godrayFade -= 0.015f;
                    if (godrayFade <= 0.3)
                    {
                        openBook = true;
                        SoundEngine.PlaySound(SoundID.Item68, Projectile.position);
                        RedeDraw.SpawnExplosion(Projectile.Center, Color.White, scale: 2, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                        Projectile.velocity *= 0;
                        Projectile.localAI[0]++;
                    }
                    break;
                case 2:
                    if (AITimer++ >= 60)
                    {
                        AITimer = 0;
                        Projectile.localAI[0] = Main.rand.Next(3, 5);
                    }
                    break;
                case 3: // Seeds of Virtue
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                    switch (Projectile.ai[1])
                    {
                        case 0:
                            host.ai[1] = 0;
                            host.ai[2] = 1;
                            host.netUpdate = true;
                            Projectile.ai[1]++;
                            break;
                        case 1:
                            if (AITimer++ <= 180)
                            {
                                Projectile.Move(new Vector2(player.Center.X - 500, player.Center.Y - 270), 15, 40, false);
                                if (Projectile.Center.X < player.Center.X - 500)
                                    AITimer = 180;
                            }
                            else
                            {
                                Projectile.Move(new Vector2(player.Center.X + 500, player.Center.Y - 270), 15, 40, false);
                                if (Projectile.Center.X > player.Center.X + 500 || AITimer >= 360)
                                    AITimer = 0;
                            }
                            if (TimerRand++ % 30 == 0)
                            {
                                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120));
                                Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<Bible_Seed>(), Projectile.damage, RedeHelper.SpreadUp(10), false, SoundID.Item42);
                            }
                            if (TimerRand >= 460)
                            {
                                SoundEngine.PlaySound(SoundID.Item68, Projectile.position);
                                RedeDraw.SpawnExplosion(Projectile.Center, Color.White, scale: 2, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                                Projectile.Kill();
                            }
                            break;
                    }
                    break;
                case 4: // Dual Phalanx
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                    switch (Projectile.ai[1])
                    {
                        case 0:
                            host.ai[1] = 0;
                            host.ai[2] = 2;
                            host.netUpdate = true;
                            Projectile.ai[1]++;
                            break;
                        case 1:
                            if (AITimer++ == 0)
                            {
                                playerOrigin = player.Center;
                            }
                            if (AITimer < 120)
                                Projectile.Move(new Vector2(playerOrigin.X - 600, player.Center.Y - 270), 18, 20, false);
                            else if (AITimer >= 120 && AITimer < 220)
                                Projectile.velocity *= 0.5f;

                            if (AITimer >= 130 && AITimer % 7 == 0 && AITimer <= 170 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slice3").WithPitchVariance(0.1f), Projectile.position);
                                SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);
                                int p = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HolyPhalanx_Proj2>(), Projectile.damage / 4, 3, Main.myPlayer, Projectile.whoAmI, TimerRand * 60);
                                Main.projectile[p].localAI[0] += TimerRand * 7;
                                TimerRand++;
                            }
                            if (AITimer >= 220)
                            {
                                Projectile.Move(new Vector2(playerOrigin.X + 600, player.Center.Y - 270), 6, 40, false);
                            }
                            if (AITimer >= 580)
                            {
                                SoundEngine.PlaySound(SoundID.Item68, Projectile.position);
                                RedeDraw.SpawnExplosion(Projectile.Center, Color.White, scale: 2, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                                Projectile.Kill();
                            }
                            break;
                    }
                    break;
                case 5: // Crossrays
                    break;
                case 6: // Graceful Cover
                    break;
                case 7: // To The Heavens!
                    break;
                case 8: // Tough Read
                    break;
            }
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (openBook)
                texture = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Magic/HolyBible_Proj").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = Projectile.scale * (1 + fluctuate);

                Color godrayColor = Color.Lerp(new Color(255, 255, 120), Color.White * Projectile.Opacity, 0.5f);
                godrayColor.A = 0;
                RedeDraw.DrawGodrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, godrayColor * godrayFade, 100 * modifiedScale * Projectile.Opacity, 30 * modifiedScale * Projectile.Opacity, 16);
            }

            float time = Main.GlobalTimeWrappedHourly;
            float timer = drawTimer / 240f + time * 0.04f;
            time %= 4f;
            time /= 2f;
            if (time >= 1f)
                time = 2f - time;
            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, 8f).RotatedBy(radians) * time, null, new Color(255, 255, 120, 50) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, 4f).RotatedBy(radians) * time, null, new Color(255, 255, 120, 77) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
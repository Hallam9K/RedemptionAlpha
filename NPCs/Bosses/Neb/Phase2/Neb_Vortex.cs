using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class Neb_Vortex : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.NebulaArcanum;
        public override void SetStaticDefaults()
        {
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.scale = 1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private float shockwaveTimer;
        public override void AI()
        {
            Projectile.localAI[0] += .001f;
            Projectile.rotation += Projectile.localAI[0];
            Projectile.scale += .02f;
            Projectile.scale = MathHelper.Min(Projectile.scale, 10);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += Projectile.localAI[0] * 4;
            shockwaveTimer -= (float)Math.PI / 120;
            if (shockwaveTimer <= 0) shockwaveTimer = MathHelper.PiOver2;
            float timer = shockwaveTimer;
            Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", Projectile.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 2f)).UseColor(1, 1, 6).UseTargetPosition(Projectile.Center);

            for (int k = 0; k < 3; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * (100 * (Projectile.scale + 1)));
                vector.Y = (float)(Math.Cos(angle) * (100 * (Projectile.scale + 1)));
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center + vector + new Vector2(0, 50), 2, 2, DustType<DustSpark2>(), newColor: Main.DiscoColor with { A = 0 }, Scale: 2f)];
                dust.noGravity = true;
                dust.velocity = dust.position.DirectionTo(Projectile.Center + new Vector2(0, 50)) * (5f * (Projectile.scale + 1));
            }
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.alpha -= 3;
                    if (Projectile.alpha <= 0)
                    {
                        Projectile.ai[0]++;
                    }
                    break;
                case 1:
                    godrayFade += 0.008f;
                    if (godrayFade >= 1)
                    {
                        Projectile.ai[0]++;
                    }
                    break;
                case 2:
                    Projectile.ai[1] += .02f;
                    if (Projectile.ai[1] >= 2)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.NebSound3 with { Pitch = -.4f }, Projectile.Center);
                        RedeDraw.SpawnExplosion(Projectile.Center, Color.White, scale: 12, shakeAmount: 0);
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 70;
                        for (int i = 0; i < 30; i++)
                            ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(25), new RainbowParticle(), Color.White, Main.rand.NextFloat(1.4f, 1.8f), AI4: Main.rand.Next(60, 90));
                        for (int i = 0; i < 60; i++)
                        {
                            Vector2 pos = Projectile.Center + RedeHelper.PolarVector(Main.rand.Next(0, 400), RedeHelper.RandomRotation());
                            Dust dust = Dust.NewDustPerfect(pos, DustType<GlowDust>(), Scale: 5);
                            dust.velocity *= 6;
                            dust.noGravity = true;
                            Color dustColor = Color.White with { A = 0 };
                            dust.color = dustColor;
                        }

                        int radius = 600;
                        RedeHelper.PlayerRadiusDamage(radius, Projectile, NPCHelper.HostileProjDamageInc(Projectile.damage), Projectile.knockBack);
                        Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                        Projectile.Kill();
                    }
                    break;
            }
            if (Projectile.localAI[1]++ > 60)
                Projectile.localAI[1] = 0;
        }
        public override void OnKill(int timeLeft)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
        }
        public override bool CanHitPlayer(Player target)
        {
            return Projectile.ai[0] > 0;
        }
        private float godrayFade;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D texture2 = TextureAssets.Projectile[656].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Color.White * 0.7f * Projectile.Opacity, -Projectile.rotation, origin, Projectile.scale * 1.2f, 0, 0);
            Main.EntitySpriteDraw(texture2, position, new Rectangle?(rect), Color.White * 0.7f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(texture2, position, new Rectangle?(rect), Color.White * 0.4f * Projectile.Opacity, -Projectile.rotation, origin, Projectile.scale * 1.6f, 0, 0);

            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 160), Projectile.Opacity);
            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = Projectile.scale * (1 + fluctuate);

                RedeDraw.DrawBossrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, color * godrayFade, 80 * modifiedScale * godrayFade, 12 * modifiedScale * godrayFade, 17);
            }

            if (Projectile.ai[0] > 1)
            {
                Texture2D circle = Request<Texture2D>("Redemption/Textures/RadialTelegraph3").Value;
                Rectangle rect2 = new(0, 0, circle.Width, circle.Height);
                Vector2 origin2 = new(circle.Width / 2, circle.Height / 2);
                float scale;
                float opacity;

                if (Projectile.localAI[1] <= 30)
                {
                    scale = Projectile.localAI[1] / 32;
                    opacity = 1f;
                }
                else if (Projectile.localAI[1] < 60)
                {
                    opacity = 1f - (Projectile.localAI[1] - 30) / 30;
                    scale = 0.9375f + (Projectile.localAI[1] - 30) / 320;
                }
                else scale = opacity = 0f;

                Main.EntitySpriteDraw(circle, position, new Rectangle?(rect2), Projectile.GetAlpha(color) * Projectile.ai[1], 0, origin2, 3, 0, 0);
                Main.EntitySpriteDraw(circle, position, new Rectangle?(rect2), Projectile.GetAlpha(color) * Projectile.ai[1] * opacity, 0, origin2, 3 * scale, 0, 0);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
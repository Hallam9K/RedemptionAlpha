using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Textures;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public static class RedeDraw
    {
        public static void DrawGodrays(SpriteBatch spriteBatch, Vector2 position, Color rayColor, float baseLength, float width, int numRays) // Code from Spirit Mod
        {
            for (int i = 0; i < numRays; i++)
            {
                Texture2D ray = ModContent.Request<Texture2D>("Redemption/Textures/Ray").Value;
                float rotation = i * (MathHelper.TwoPi / numRays) + (Main.GlobalTimeWrappedHourly * (((i % 3) + 1f) / 3));
                rotation -= MathHelper.PiOver2;

                float length = baseLength * (float)(Math.Sin((Main.GlobalTimeWrappedHourly + i) * 2) / 5 + 1);
                Vector2 rayscale = new(width / ray.Width, length / ray.Height);
                spriteBatch.Draw(ray, position, null, rayColor, rotation,
                    new Vector2(ray.Width / 2, 0), rayscale, 0, 0);
            }
        }
        public static void SpawnRing(Vector2 center, Color color, float flatScale = 0.13f, float multiScale = 0.9f, float glowScale = 2)
        {
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: glowScale);
            dust.noGravity = true;
            Color dustColor = new(color.R, color.G, color.B) { A = 0 };
            dust.color = dustColor;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(null, center, Vector2.Zero, ModContent.ProjectileType<Ring_Visual>(), 0, 0,
                    Main.myPlayer, flatScale, multiScale);
                (Main.projectile[p].ModProjectile as Ring_Visual).color = color;
            }
        }
        public static void SpawnExplosion(Vector2 center, Color color, int dustID = DustID.Torch, float shakeAmount = 7, int dustAmount = 30, float dustScale = 2, float scale = 4f, bool noDust = false, Texture2D tex = null)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(null, center, Vector2.Zero, ModContent.ProjectileType<Explosion_Visual>(), 0, 0,
                    Main.myPlayer, shakeAmount, dustAmount);
                if (Main.projectile[p].ModProjectile is Explosion_Visual explode)
                {
                    explode.color = color;
                    explode.dustID = dustID;
                    explode.dustScale = dustScale;
                    explode.scale = scale;
                    explode.noDust = noDust;
                    explode.texture = tex;
                }
            }
        }
    }
}
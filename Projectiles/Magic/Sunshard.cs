using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Sunshard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redemptive Spark");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.CountsAsHoming[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 140;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public override Color? GetAlpha(Color lightColor) => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightYellow, Color.White, Color.LightYellow);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.velocity.Length() < 5)
                Projectile.velocity *= 1.1f;

            if (Projectile.localAI[0] == 0)
            {
                AdjustMagnitude(ref Projectile.velocity);
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 20 && Projectile.localAI[0] < 40)
            {
                Vector2 move = Vector2.Zero;
                float distance = 70f;
                bool targetted = false;
                foreach (NPC target in Main.npc.Take(Main.maxNPCs))
                {
                    if (!target.CanBeChasedBy() || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0))
                        continue;

                    Vector2 newMove = target.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        targetted = true;
                    }
                }
                if (targetted)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player2 = Main.player[k];
                if (!player2.active || player2.dead || player2.whoAmI == player.whoAmI)
                    continue;

                if (!Projectile.Hitbox.Intersects(player2.Hitbox))
                    continue;

                if (player2.statLife < player2.statLifeMax2)
                {
                    player2.statLife += 3;
                    player2.HealEffect(3);
                }
                if (player2.statLife > player2.statLifeMax2)
                    player2.statLife = player2.statLifeMax2;

                Projectile.Kill();
            }
            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active)
                    continue;

                if (!target.friendly || target.dontTakeDamage || target.immortal)
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                if (target.life < target.lifeMax)
                {
                    target.life += 3;
                    target.HealEffect(3);
                }
                if (target.life > target.lifeMax)
                    target.life = target.lifeMax;

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 10f)
            {
                vector *= 9f / magnitude;
            }
        }

        public override void Kill(int timeLeft)
        {
            DustHelper.DrawCircle(Projectile.Center, DustID.GoldFlame, 1, 4, 4, nogravity: true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class EaglecrestSling_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 10;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.DistanceSQ(player.Center) < 800 * 800)
                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 5;

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                    -Projectile.velocity.X * 0.6f, -Projectile.velocity.Y * 0.6f, Scale: 2);

            Rectangle boom = new((int)Projectile.Center.X - 50, (int)Projectile.Center.Y - 50, 100, 100);
            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active || target.friendly)
                    continue;

                if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                    continue;

                target.immune[Projectile.whoAmI] = 20;
                int hitDirection = Projectile.Center.X > target.Center.X ? -1 : 1;
                BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
            }
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Projectile.velocity.Y += 0.06f;

            if (Projectile.velocity.Length() < 20)
                Projectile.velocity *= 1.1f;

            if (Projectile.localAI[0] == 0)
                AdjustMagnitude(ref Projectile.velocity);

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 10 && Main.myPlayer == Projectile.owner)
            {
                Vector2 move = Vector2.Zero;
                float distance = 2000f;
                bool targetted = false;

                Vector2 newMove = Main.MouseWorld - Projectile.Center;
                float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                if (distanceTo < distance)
                {
                    move = newMove;
                    targetted = true;
                }
                if (targetted)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (6 * Projectile.velocity + move) / 7f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
        }

        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
            {
                vector *= 20f / magnitude;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Terraria.GameContent;

namespace Redemption.Projectiles.Minions
{
    public class Hardlight_MissileDrone : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3_MissileDrone";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Missile Drone Mk.I");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
        }

        Vector2 vector;
        public int shotCount;
        public override void AI()
        {
            if (Projectile.frameCounter++ >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.LookAtEntity(player);

            float soundVolume = Projectile.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, Projectile.position);
                Projectile.soundDelay = 10;
            }

            if (++Projectile.ai[1] % 80 == 0)
            {
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 100);
                vector.Y = (float)(Math.Cos(angle) * 100);
                Projectile.ai[1] = 0;
            }
            if (Projectile.localAI[0]++ == 0)
            {
                for (int m = 0; m < 16; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)16 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
            }
            if (Projectile.localAI[0] > 120 && Projectile.localAI[0] < 500 && shotCount < 4)
            {
                int getNPC = RedeHelper.GetNearestNPC(Projectile.Center);
                if (getNPC != -1 && Projectile.localAI[0] % 30 == 0 && Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, (Main.npc[getNPC].Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(0.2f, 0.2f)), ModContent.ProjectileType<Hardlight_SlayerMissile>(), 242, 4, player.whoAmI, Projectile.whoAmI);
                    shotCount++;
                }
            }
            if (Projectile.localAI[0] >= 500 || shotCount >= 4)
            {
                Projectile.velocity.Y -= 0.5f;
                if (Projectile.DistanceSQ(player.Center) > 1500 * 1500)
                    Projectile.active = false;
            }
            else
            {
                Projectile.Move(vector, 11, 15, true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowMask, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class Hardlight_SlayerMissile : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/SlayerMissile";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drone Missile");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Alarm2 with { Volume = .2f, PitchVariance = .1f }, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            if (Projectile.localAI[0]++ < 20)
            {
                Vector2 move = Vector2.Zero;
                float distance = 1200;
                bool targetted = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
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
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
            {
                vector *= 19f / magnitude;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, height)), lightColor, Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, height)), Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, effects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }

        }
    }
}
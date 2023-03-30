using System;
using Microsoft.Xna.Framework;
using Redemption.Buffs.NPCBuffs;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class badtimekid : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SANS?!");
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Projectile.rotation += 0.16f;
            if (Projectile.localAI[0]++ == 5f)
            {
                SoundEngine.PlaySound(CustomSounds.sans, Projectile.position);
                Projectile.localAI[0] = 1;
            }
            if (Projectile.localAI[0] == 0f)
                AdjustMagnitude(ref Projectile.velocity);
            Vector2 move = Vector2.Zero;
            float distance = 1800f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
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
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 41f)
                vector *= 40f / magnitude;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 1;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(ModContent.BuffType<sansDebuff>(), 10);
        }
    }
}
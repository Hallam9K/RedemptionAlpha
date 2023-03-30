using System;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Nanite_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nanite");
            ElementID.ProjPsychic[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
        }
        private NPC npcInside;
        private Vector2 npcOrigin;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (npcInside != null && (!npcInside.active || npcInside.life <= 0))
                npcInside = null;

            if (npcInside != null)
            {
                if (Projectile.scale <= 0.1f)
                    Projectile.position = npcInside.Center;
                else
                    Projectile.position = npcInside.position + npcOrigin;
                Projectile.scale -= 0.05f;
            }
            else
            {
                Projectile.scale += 0.05f;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile same = Main.projectile[i];
                    if (!same.active || same.whoAmI == Projectile.whoAmI || same.type != Type || !Projectile.Hitbox.Intersects(same.Hitbox))
                        continue;

                    Projectile.position += new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));
                }

                if (Projectile.localAI[0] == 0)
                {
                    AdjustMagnitude(ref Projectile.velocity);
                    Projectile.localAI[0] = 1;
                }
                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.lifeMax > 5 && !npc.immortal && !npc.GetGlobalNPC<RedeNPC>().invisible)
                    {
                        Vector2 newMove = npc.Center - Projectile.Center;
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
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.1f, 1);
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 18f)
                vector *= 17f / magnitude;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return target == npcInside || npcInside == null ? null : false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (npcInside == null)
            {
                npcInside = target;
                float x = MathHelper.Distance(Projectile.position.X, target.position.X);
                float y = MathHelper.Distance(Projectile.position.Y, target.position.Y);
                npcOrigin = new Vector2(x, y);
            }
            Projectile.localNPCImmunity[target.whoAmI] = 60;
            target.immune[Projectile.owner] = 0;
            target.AddBuff(BuffID.Confused, 70);
        }
    }
}
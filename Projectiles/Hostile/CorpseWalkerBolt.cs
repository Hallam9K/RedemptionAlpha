using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Hostile
{
    public class CorpseWalkerBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Redemptive Spark");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjHoly[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Projectile.Kill();
        public override bool? CanHitNPC(NPC target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return target == host.Redemption().attacker && !NPCLists.Undead.Contains(target.type) && !NPCLists.Skeleton.Contains(target.type) ? null : false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return !target.RedemptionPlayerBuff().skeletonFriendly;
        }

        public override Color? GetAlpha(Color lightColor) => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightYellow, Color.White, Color.LightYellow);

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.3f, 0f);
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
                float distance = 800;
                bool targetted = false;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player target = Main.player[p];
                    if (!target.active || target.dead || target.invis || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0))
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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (Projectile.ai[1] != -1)
                    {
                        NPC NPCtarget = Main.npc[(int)Projectile.ai[1]];
                        if (!target.active || target.whoAmI != NPCtarget.whoAmI || !target.chaseable || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0))
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
                }
                if (targetted)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
            NPC host = Main.npc[(int)Projectile.ai[0]];
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == host.whoAmI)
                    continue;

                if (!NPCLists.Undead.Contains(target.type) && !NPCLists.Skeleton.Contains(target.type))
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
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
        public override void OnKill(int timeLeft)
        {
            DustHelper.DrawCircle(Projectile.Center, DustID.GoldFlame, 1, 4, 4, nogravity: true);
        }
    }
}
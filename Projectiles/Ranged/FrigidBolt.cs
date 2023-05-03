using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class FrigidBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjIce[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            if (!IsStickingToTarget)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
                }
            }
            float glow = Projectile.localAI[0] / 400;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightCyan) * glow, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 255, 255, 0)) * glow * .5f, Projectile.rotation, drawOrigin, Projectile.scale + 1f, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (IsStickingToTarget)
            {
                int npcIndex = (int)Projectile.ai[1];
                if (npcIndex >= 0 && npcIndex < Main.maxNPCs && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                        behindNPCsAndTiles.Add(index);
                    else
                        behindNPCs.Add(index);
                    return;
                }
            }
            behindProjectiles.Add(index);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 10;
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return projHitbox.Intersects(targetHitbox);
        }

        public bool IsStickingToTarget
        {
            get { return Projectile.ai[0] == 1f; }
            set { Projectile.ai[0] = value ? 1f : 0f; }
        }

        public float TargetWhoAmI
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.localAI[0] < 400 && origDamage == 0)
            {
                origDamage = Projectile.damage;
                Projectile.damage = 0;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            IsStickingToTarget = true;
            TargetWhoAmI = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.netUpdate = true;
            target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 200);

            int maxStickingJavelins = 5;
            Point[] stickingJavelins = new Point[maxStickingJavelins];
            int javelinIndex = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile currentProjectile = Main.projectile[i];
                if (i != Projectile.whoAmI && currentProjectile.active && currentProjectile.owner == Main.myPlayer && currentProjectile.type == Projectile.type &&
                    currentProjectile.ai[0] == 1f && currentProjectile.ai[1] == target.whoAmI
                )
                {
                    stickingJavelins[javelinIndex++] = new Point(i, currentProjectile.timeLeft);
                    if (javelinIndex >= stickingJavelins.Length)
                    {
                        break;
                    }
                }
            }

            if (javelinIndex >= stickingJavelins.Length)
            {
                int oldJavelinIndex = 0;
                for (int i = 1; i < stickingJavelins.Length; i++)
                {
                    if (stickingJavelins[i].Y < stickingJavelins[oldJavelinIndex].Y)
                    {
                        oldJavelinIndex = i;
                    }
                }
                Main.projectile[stickingJavelins[oldJavelinIndex].X].Kill();
            }
        }
        private int origDamage;
        public override void AI()
        {
            if (IsStickingToTarget)
            {
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                bool killProj = false;
                Projectile.localAI[0]++;
                bool hitEffect = Projectile.localAI[0] % 30f == 0f;
                int projTargetIndex = (int)TargetWhoAmI;
                if (Projectile.localAI[0] >= 400 || projTargetIndex < 0 || projTargetIndex >= 200)
                    killProj = true;
                else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                    if (hitEffect)
                        Main.npc[projTargetIndex].HitEffect(0, 1.0);
                }
                else
                    killProj = true;

                if (killProj)
                {
                    if (Projectile.localAI[0] >= 400)
                    {
                        SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                        SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
                        if (Main.npc[projTargetIndex].knockBackResist > 0 && !Main.npc[projTargetIndex].RedemptionNPCBuff().iceFrozen)
                            Main.npc[projTargetIndex].AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(Main.npc[projTargetIndex].lifeMax, 60, 1780)));
                        for (int i = 0; i < 10; i++)
                            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, Scale: Main.rand.NextFloat(.5f, 1));
                        for (int k = 0; k < 20; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 60);
                            vector.Y = (float)(Math.Cos(angle) * 60);
                            Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, ModContent.DustType<GlowDust>(), 0f, 0f, 100, default, 1f)];
                            dust2.noGravity = true;
                            dust2.noGravity = true;
                            Color dustColor = new(Color.LightCyan.R, Color.LightCyan.G, Color.LightCyan.B) { A = 0 };
                            dust2.color = dustColor;
                            dust2.velocity = -Projectile.DirectionTo(dust2.position) * 6f;
                        }
                        DustHelper.DrawDustImage(Projectile.Center, DustID.Frost, 0.7f, "Redemption/Effects/DustImages/Flake", 3, true, RedeHelper.RandomRotation());
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;
                        Rectangle boom = new((int)Projectile.Center.X - 50, (int)Projectile.Center.Y - 50, 100, 100);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || (!target.CanBeChasedBy() && target.type != NPCID.TargetDummy))
                                continue;

                            if (!target.Hitbox.Intersects(boom))
                                continue;

                            int hitDirection = target.RightOfDir(Projectile);
                            BaseAI.DamageNPC(target, origDamage, Projectile.knockBack * 2, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                            if (Main.rand.NextBool(2) && target.knockBackResist > 0 && !target.RedemptionNPCBuff().iceFrozen)
                                target.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(target.lifeMax, 60, 1780)));
                        }
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile p = Main.projectile[i];
                            if (!p.active || p.type != Type || p.ai[0] != 1f || p.ai[1] != Main.npc[projTargetIndex].whoAmI)
                                continue;

                            int hitDirection = Main.npc[projTargetIndex].RightOfDir(Projectile);
                            BaseAI.DamageNPC(Main.npc[projTargetIndex], origDamage, 0, hitDirection, p, crit: Projectile.HeldItemCrit());
                            p.Kill();
                        }
                    }
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.velocity.Y += 0.03f;
                if (Main.rand.NextBool(4))
                {
                    int sparkle = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, Scale: 1f);
                    Main.dust[sparkle].velocity *= 0.2f;
                    Main.dust[sparkle].noGravity = true;
                    Main.dust[sparkle].color = new Color(255, 255, 255) { A = 0 };
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice,
                    -Projectile.velocity.X * 0.6f, -Projectile.velocity.Y * 0.6f);

            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            return true;
        }
    }
}
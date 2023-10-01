using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Buffs.NPCBuffs;
using System.Collections.Generic;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Redemption.Projectiles.Magic
{
    public class ContagionShard_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/SeedOfInfection/SoI_SplitShard";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenomite Shard");
            Main.projFrames[Projectile.type] = 7;
            ElementID.ProjPoison[Type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        private float glow = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 7;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            if (glow > 0)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Color.LightGreen * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = 0;
                    Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color * glow, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
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
        public override bool? CanHitNPC(NPC target) => !IsStickingToTarget ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            IsStickingToTarget = true;
            TargetWhoAmI = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.netUpdate = true;
            target.AddBuff(ModContent.BuffType<ContagionShardDebuff>(), 900);
            int maxStickingJavelins = 6;
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
        public override void AI()
        {
            if (IsStickingToTarget)
            {
                glow -= 0.1f;
                if (Projectile.frame < 2 && ++Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 7)
                        Projectile.frame = 2;
                }
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                bool killProj = false;
                Projectile.localAI[0]++;
                bool hitEffect = Projectile.localAI[0] % 30f == 0f;
                int projTargetIndex = (int)TargetWhoAmI;
                if (projTargetIndex < 0 || projTargetIndex >= 200 || Projectile.localAI[1] is 1)
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
                    if (Main.myPlayer == Projectile.owner && Projectile.localAI[1] is 1)
                    {
                        BaseAI.DamageNPC(Main.npc[projTargetIndex], Projectile.damage, Projectile.knockBack, Projectile, crit: RedeHelper.HeldItemCrit(Projectile));
                        int spread = 8;
                        for (int i = 0; i < 5; i++)
                        {
                            int rot = spread * i;
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, Projectile.rotation + MathHelper.PiOver2 + MathHelper.ToRadians(rot - spread)), ModContent.ProjectileType<SoI_ShardShot>(), Projectile.damage / 4, Projectile.knockBack / 2, Main.myPlayer, 2, projTargetIndex);
                            Main.projectile[p].timeLeft = 80;
                            Main.projectile[p].hostile = false;
                            Main.projectile[p].friendly = true;
                            Main.projectile[p].tileCollide = true;
                            Main.projectile[p].DamageType = DamageClass.Magic;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                    Projectile.Kill();
                }
            }
            else
            {
                if (++Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 7)
                        Projectile.frame = 2;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy,
                    -Projectile.velocity.X * 0.6f, -Projectile.velocity.Y * 0.6f);

            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 5; ++i)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Scale: 1.5f);
        }
    }
}

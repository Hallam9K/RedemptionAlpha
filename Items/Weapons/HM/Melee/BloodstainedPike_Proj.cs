using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Redemption.Base;
using System;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BloodstainedPike_Proj : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 60f;
        protected virtual float HoldoutRangeMax => 126f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloodstained Pike");
            ElementID.ProjBlood[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hide = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        private readonly List<int> skewered = new();
        private bool killSkewered;

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;
            player.heldProj = Projectile.whoAmI;

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            float halfDuration = duration * 0.5f;
            float progress;

            if (Projectile.timeLeft < halfDuration)
            {
                if (player.channel)
                {
                    player.itemTime = player.itemAnimationMax / 2;
                    player.itemAnimation = player.itemAnimationMax / 2;
                    Projectile.timeLeft = (int)halfDuration;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    { 
                        if (skewered.Contains(i))
                        {
                            NPC npc = Main.npc[i];
                            Projectile.localAI[0]++;
                            if (npc.life <= 0 || !npc.active)
                                skewered.Remove(i);
                            npc.Center = Projectile.Center;
                            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, Scale: 2f);
                            Main.dust[dust].noGravity = true;
                            if (skewered.Count >= 5 || (skewered.Count >= 1 && Projectile.localAI[0] >= 600))
                                killSkewered = true;        
                            if (killSkewered)
                            {
                                for (int k = 0; k < 20; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(npc.Center + vector, 2, 2, DustID.LifeDrain, Scale: 2)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -npc.DirectionTo(dust2.position) * 10f;
                                }
                                BaseAI.KillNPCWithLoot(npc);
                                skewered.Remove(i);
                                if (skewered.Count == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.position);
                                    if (Main.myPlayer == Projectile.owner)
                                    {
                                        int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodstainedPike_Proj2>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                                        Main.projectile[p].rotation = Projectile.rotation;
                                        Main.projectile[p].spriteDirection = Projectile.spriteDirection;
                                        Main.projectile[p].netUpdate = true;
                                    }
                                    Projectile.Kill();
                                }
                            }
                        }
                    }
                }
                else
                    skewered.Clear();

                progress = Projectile.timeLeft / halfDuration;
            }
            else
                progress = (duration - Projectile.timeLeft) / halfDuration;

            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (skewered.Count < 5 && player.channel && (Projectile.timeLeft > player.itemAnimationMax / 2 || player.velocity.Length() > 1) && (target.life <= 500 || target.life <= target.lifeMax / 10) && target.knockBackResist > 0 && target.width < 100 && target.height < 100 && !target.dontTakeDamage && !target.immortal)
            {
                if (target.life > 0)
                    skewered.Add(target.whoAmI);
            }
        }
        public override void OnKill(int timeLeft)
        {
            skewered.Clear();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !skewered.Contains(target.whoAmI) ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(64, (Projectile.Center - player.Center).ToRotation());
            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class BloodstainedPike_Proj2 : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/BloodstainedPike_Proj";
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloodstained Pike");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjBlood[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.scale = 1.2f;
        }

        public override bool? CanCutTiles() => false;
        private NPC target;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.LookByVelocity();
            if (Projectile.timeLeft < 300)
            {
                Projectile.timeLeft = 2;
                Projectile.LookByVelocity();
                Projectile.rotation += Projectile.velocity.Length() / 50 * Projectile.spriteDirection;
                Projectile.Move(player.Center, Projectile.localAI[1], 1);
                Projectile.localAI[1] *= 1.01f;
                if (Projectile.DistanceSQ(player.Center) < 20 * 20)
                    Projectile.Kill();
                return;
            }
            Projectile.localAI[1] = 10;
            if (RedeHelper.ClosestNPC(ref target, 800, Projectile.Center, true, player.MinionAttackTargetNPC))
            {
                Projectile.Move(new Vector2(target.Center.X, target.Center.Y), 20, 40);
                Projectile.localAI[0]--;
                if (Projectile.DistanceSQ(target.Center) < 300 * 300 && Projectile.localAI[0] <= 0)
                {
                    Projectile.velocity = RedeHelper.PolarVector(-60, (Projectile.Center - target.Center).ToRotation());
                    Projectile.localAI[0] = 30;
                }
            }
            else
                Projectile.Move(new Vector2(player.Center.X + 20 * -player.direction, player.Center.Y - 42), 10, 70);

            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
        }
        public override void OnKill(int timeLeft)
        {
            RedeDraw.SpawnRing(Projectile.Center, Color.Red, 0.2f, 0.9f, 4);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.4f, 1.2f, 1.4f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + Vector2.UnitY * Projectile.gfxOffY - new Vector2(22, 33);
                Color color = Color.Red * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, scale, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}
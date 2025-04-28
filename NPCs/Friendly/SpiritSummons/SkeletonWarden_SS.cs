using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.NPCs.PreHM;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class SkeletonWarden_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonWarden";
        public enum ActionState
        {
            Idle,
            Wander,
            Block,
            Slam,
            Defend,
            Retreat,
            SoulMove = 10
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public Vector2 ShieldOffset;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Skeleton Warden");
            Main.npcFrameCount[NPC.type] = 17;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 54;
            NPC.damage = 18;
            NPC.friendly = true;
            NPC.defense = 11;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.RedemptionGuard().GuardPoints = 20;
            NPC.Redemption().spiritSummon = true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 2);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                if (NPC.RedemptionGuard().GuardBroken)
                    AIState = ActionState.Retreat;
                else
                    AIState = ActionState.Block;
            }
        }
        private bool blocked;
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (blocked && NPC.RedemptionGuard().GuardPoints >= 0)
            {
                NPC.HitSound = SoundID.DD2_SkeletonHurt with { Volume = 0 };
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig, 0.1f, true, DustID.DungeonSpirit, default, 10, 1, 40);
                blocked = false;
            }
            else
                NPC.HitSound = SoundID.DD2_SkeletonHurt;
            blocked = false;
        }
        private readonly List<int> projBlocked = new();
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardBroken)
                return;

            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            if (NPC.frame.Y >= 13 * 64)
                ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            else
            {
                if (NPC.RightOf(projectile) && NPC.spriteDirection == 1 || (projectile.RightOf(NPC) && NPC.spriteDirection == -1))
                    return;
            }

            if (!projBlocked.Contains(projectile.whoAmI) && (!projectile.active || projectile.friendly))
                return;

            Rectangle projectileHitbox = projectile.Hitbox;
            if (projectile.Redemption().swordHitbox != default)
                projectileHitbox = projectile.Redemption().swordHitbox;
            if (projectile.Colliding(projectileHitbox, ShieldHitbox))
            {
                projBlocked.Remove(projectile.whoAmI);
                if (!projectile.ProjBlockBlacklist() && projectile.penetrate > 1)
                    projectile.timeLeft = Math.Min(projectile.timeLeft, 2);
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                {
                    NPC.HitSound = SoundID.DD2_SkeletonHurt with { Volume = 0 };
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig, 0.1f, true, DustID.DungeonSpirit, default, 10, 1, 40);
                    blocked = false;
                }
                else
                    NPC.HitSound = SoundID.DD2_SkeletonHurt;
            }
        }
        public Vector2 SetEyeOffset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                4 => new Vector2(0, -4),
                7 => new Vector2(0, -2),
                11 => new Vector2(0, -2),
                _ => new Vector2(0, 0),
            };
        }

        public override bool CheckActive() => false;
        private int runCooldown;
        public override void ModifyTypeName(ref string typeName)
        {
            if (NPC.ai[3] != -1)
                typeName = Main.player[(int)NPC.ai[3]].name + "'s " + Lang.GetNPCNameValue(Type);
        }
        private NPC defending;
        public override void OnSpawn(IEntitySource source)
        {
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            SSBase.SpiritBasicAI(NPC, player);
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;
            NPC.TargetClosest();

            NPC.LookByVelocity();
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            Rectangle ShieldRaisedHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            if (!NPC.RedemptionGuard().GuardBroken)
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.damage <= 0 || !projectile.hostile || projectile.ProjBlockBlacklist())
                        continue;

                    if (NPC.frame.Y >= 13 * 64)
                    {
                        if (projectile.Colliding(projectile.Hitbox, ShieldRaisedHitbox))
                        {
                            if (!projectile.ProjBlockBlacklist() && projectile.penetrate != -1)
                            {
                                blocked = true;
                                NPC.SimpleStrikeNPC(projectile.damage, 1);
                                projectile.Kill();
                            }
                            if (!projBlocked.Contains(projectile.whoAmI))
                                projBlocked.Add(projectile.whoAmI);
                        }
                    }
                    else
                    {
                        if ((NPC.RightOf(projectile) && NPC.spriteDirection == -1 || (projectile.RightOf(NPC) && NPC.spriteDirection == 1)) && projectile.Colliding(projectile.Hitbox, ShieldHitbox))
                        {
                            if (!projectile.ProjBlockBlacklist() && projectile.penetrate != -1)
                            {
                                blocked = true;
                                NPC.SimpleStrikeNPC(projectile.damage, 1);
                                projectile.Kill();
                            }
                            if (!projBlocked.Contains(projectile.whoAmI))
                                projBlocked.Add(projectile.whoAmI);
                        }
                    }
                }
            }
            if (Main.rand.NextBool(4000) && !Main.dedServ)
                SoundEngine.PlaySound(AmbientSound, NPC.position);

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand || NPC.DistanceSQ(player.Center) > 200 * 200)
                    {
                        moveTo = NPC.FindGround(20);
                        if (NPC.DistanceSQ(player.Center) > 200 * 200)
                            moveTo = (player.Center + new Vector2(20 * NPC.RightOfDir(player), 0)) / 16;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.9f * SpeedMultiplier, 6, 6, NPC.Center.Y > moveTo.Y * 16, player);
                    break;

                case ActionState.Defend:
                    if (defending == null || !defending.active || NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false, !HasEyes) && !NPC.Sight(defending, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 2, NPC.frame.Y >= 13 * 64 ? ShieldRaisedHitbox : ShieldHitbox, NPC.damage, 11f, false, 25);
                    }

                    moveTo = defending.Center + new Vector2((defending.width + 40) * defending.spriteDirection, 0);
                    if (NPC.Center.X + 20 > moveTo.X && NPC.Center.X - 20 < moveTo.X)
                        AIState = ActionState.Block;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, moveTo, 0.2f, 2.4f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > attacker.Center.Y, attacker);
                    break;

                case ActionState.Block:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    SightCheck();

                    if (NPC.velocity.Y < 0 && AITimer == 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0 && AITimer == 0)
                        NPC.velocity.X = 0;

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false, !HasEyes) && !NPC.Sight(defending, VisionRange, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 2, NPC.frame.Y >= 13 * 64 ? ShieldRaisedHitbox : ShieldHitbox, NPC.damage, 11f, false, 25);
                    }

                    if (defending == null)
                    {
                        NPC.LookAtEntity(attacker);
                        break;
                    }
                    if (defending.active)
                        NPC.LookAtEntity(defending, true);

                    if (defending.active && (defending.velocity.X != 0 || NPC.DistanceSQ(defending.Center) >= (defending.width + 40) * (defending.width + 40)))
                    {
                        AIState = ActionState.Defend;
                    }
                    break;
                case ActionState.Retreat:
                    SightCheck();
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, NPCHelper.RunAwayVector(NPC, attacker), 0.4f, 2.2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.1f : 1), 12, 8, NPC.Center.Y > player.Center.Y, attacker);
                    break;
                case ActionState.SoulMove:
                    SSBase.SoulMoveState(NPC, ref AITimer, player, ref TimerRand, ref runCooldown, ref moveTo, yOffset: 4);
                    break;
            }
            if (AIState is not ActionState.SoulMove)
            {
                if (SSBase.NoSpiritEffect(NPC))
                    NPC.alpha = 0;
                else
                {
                    NPC.alpha += Main.rand.Next(-10, 11);
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;

            if (AIState is ActionState.Block && NPC.velocity.Length() == 0 && !NPC.RedemptionGuard().GuardBroken && attacker != null && attacker.Center.Y < NPC.Center.Y - NPC.height + 40 && attacker.Center.X > NPC.Center.X - 100 && attacker.Center.X < NPC.Center.X + 100)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 13 * frameHeight)
                    NPC.frame.Y = 13 * frameHeight;
                if (++NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 16 * frameHeight)
                        NPC.frame.Y = 13 * frameHeight;
                }
                return;
            }

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X > -.1f && NPC.velocity.X < .1f)
                {
                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 5 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
            ShieldOffset = SetEyeOffset(ref frameHeight);
        }
        public int GetNearestNPC(bool nearestUndead = false)
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == NPCType<SkeletonWarden_SS>() || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (!nearestUndead && (target.friendly || target.lifeMax <= 5 || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                    continue;

                if (nearestUndead && target.type != NPCType<SkeletonNoble_SS>())
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public void SightCheck()
        {
            RedeNPC globalNPC = NPC.Redemption();

            int gotNPC = GetNearestNPC();
            if (defending != null && globalNPC.attacker == null && defending.GetGlobalNPC<RedeNPC>().attacker != Main.LocalPlayer)
                globalNPC.attacker = defending.GetGlobalNPC<RedeNPC>().attacker;

            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
            {
                if (AIState != ActionState.Block && !NPC.RedemptionGuard().GuardBroken)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(NoticeSound, NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Block;
                    NPC.netUpdate = true;
                }
                if (AIState != ActionState.Retreat && NPC.RedemptionGuard().GuardBroken)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(NoticeSound, NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Retreat;
                    NPC.netUpdate = true;
                }
            }
            gotNPC = GetNearestNPC(true);
            if (defending == null && gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
            {
                defending = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Defend;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D ShieldTex = Request<Texture2D>(Texture + "_Shield").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            bool noSpiritEffect = SSBase.NoSpiritEffect(NPC);
            Color color = noSpiritEffect ? drawColor : Color.White;
            if (!noSpiritEffect)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!NPC.RedemptionGuard().GuardBroken && NPC.frame.Y < 13 * 64)
                spriteBatch.Draw(ShieldTex, NPC.Center - screenPos, null, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 - new Vector2(0, ShieldOffset.Y), NPC.scale, effects, 0);

            if (!noSpiritEffect)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.2f, 0.4f));
        }
    }
}
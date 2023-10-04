using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.NPCs.PreHM;
using ParticleLibrary;
using Redemption.Dusts;
using Redemption.Particles;
using Terraria.Graphics.Shaders;

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

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
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
            Player player = Main.player[(int)NPC.ai[3]];
            NPC.damage = (int)(NPC.damage * player.GetTotalDamage(DamageClass.Summon).Additive);

            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            if (!player.active || player.dead || !SSBase.CheckActive(player))
                NPC.SimpleStrikeNPC(999, 1);
            NPC.TargetClosest();
            NPC.LookByVelocity();
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            Rectangle ShieldRaisedHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            if (!NPC.RedemptionGuard().GuardBroken)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (!projectile.active || projectile.damage <= 0 || !projectile.hostile || projectile.ProjBlockBlacklist())
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
                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Ambient"), NPC.position);

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
                    if (defending == null || !defending.active || globalNPC.attacker == null || !globalNPC.attacker.active || NPC.TargetCheck(2) || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false, !HasEyes) && !NPC.Sight(defending, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(globalNPC.attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        if ((NPC.frame.Y >= 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldRaisedHitbox)) ||
                            (NPC.frame.Y < 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldHitbox)))
                        {
                            if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                            {
                                attackerNPC.immune[NPC.whoAmI] = 25;
                                int hitDirection = attackerNPC.RightOfDir(NPC);
                                attackerNPC.velocity.X += NPC.velocity.X * attackerNPC.knockBackResist;
                                BaseAI.DamageNPC(attackerNPC, NPC.damage, 11, hitDirection, NPC);
                            }
                        }
                    }

                    moveTo = defending.Center + new Vector2((defending.width + 40) * defending.spriteDirection, 0);
                    if (NPC.Center.X + 20 > moveTo.X && NPC.Center.X - 20 < moveTo.X)
                        AIState = ActionState.Block;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, moveTo, 0.2f, 2.4f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Block:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();

                    if (NPC.velocity.Y < 0 && AITimer == 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0 && AITimer == 0)
                        NPC.velocity.X = 0;

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false, !HasEyes) && !NPC.Sight(defending, VisionRange, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(globalNPC.attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        if ((NPC.frame.Y >= 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldRaisedHitbox)) ||
                            (NPC.frame.Y < 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldHitbox)))
                        {
                            if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                            {
                                attackerNPC.immune[NPC.whoAmI] = 25;
                                int hitDirection = attackerNPC.RightOfDir(NPC);
                                attackerNPC.velocity.X += NPC.velocity.X * attackerNPC.knockBackResist;
                                BaseAI.DamageNPC(attackerNPC, NPC.damage, 11, hitDirection, NPC);
                            }
                        }
                    }

                    if (defending == null)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
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
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y), 0.4f, 2.2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.1f : 1), 12, 8, NPC.Center.Y > player.Center.Y, globalNPC.attacker);
                    break;
                case ActionState.SoulMove:
                    NPC.alpha = 255;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    AITimer = 0;

                    ParticleManager.NewParticle(NPC.Center + RedeHelper.Spread(10) + NPC.velocity, Vector2.Zero, new SpiritParticle(), Color.White, 0.6f * 1, 0, 1);
                    for (int i = 0; i < 2; i++)
                    {
                        int dust = Dust.NewDust(NPC.Center + NPC.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= .1f;
                        Color dustColor = new(188, 244, 227) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }

                    if (NPC.Hitbox.Intersects(player.Hitbox) && Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 2);
                            Main.dust[dust].velocity *= 2f;
                            Main.dust[dust].noGravity = true;
                        }

                        NPC.alpha = 0;
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;
                        NPC.velocity *= 0f;

                        moveTo = NPC.FindGround(20);
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.Move(player.Center - new Vector2(0, 4), 20, 20);
                    break;
            }
            if (Main.myPlayer == player.whoAmI && NPC.DistanceSQ(player.Center) > 2000 * 2000)
            {
                NPC.Center = player.Center;
                NPC.velocity *= 0.1f;
                NPC.netUpdate = true;
            }
            if (AIState is not ActionState.SoulMove)
            {
                NPC.alpha += Main.rand.Next(-10, 11);
                NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
            }

            if (!Main.rand.NextBool(40))
                return;
            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.Spread(2), new SpiritParticle(), Color.White, 1);
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            RedeNPC globalNPC = NPC.Redemption();

            if (AIState is ActionState.Block && NPC.velocity.Length() == 0 && !NPC.RedemptionGuard().GuardBroken && globalNPC.attacker.Center.Y < NPC.Center.Y - NPC.height + 40 && globalNPC.attacker.Center.X > NPC.Center.X - 100 && globalNPC.attacker.Center.X < NPC.Center.X + 100)
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
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == ModContent.NPCType<SkeletonWarden_SS>() || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (!nearestUndead && (target.friendly || target.lifeMax <= 5 || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                    continue;

                if (nearestUndead && target.type != ModContent.NPCType<SkeletonNoble_SS>())
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
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Block;
                    NPC.netUpdate = true;
                }
                if (AIState != ActionState.Retreat && NPC.RedemptionGuard().GuardBroken)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
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
            Texture2D Glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D ShieldTex = ModContent.Request<Texture2D>(Texture + "_Shield").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!NPC.RedemptionGuard().GuardBroken && NPC.frame.Y < 13 * 64)
                spriteBatch.Draw(ShieldTex, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2 - new Vector2(0, ShieldOffset.Y), NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.2f, 0.4f));
        }
    }
}
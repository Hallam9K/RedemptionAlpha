using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.Localization;

namespace Redemption.NPCs.PreHM
{
    public class SkeletonWarden : SkeletonBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Block,
            Slam,
            Defend,
            Retreat
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public Vector2 ShieldOffset;
        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 54;
            NPC.damage = 18;
            NPC.friendly = false;
            NPC.defense = 11;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 170;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SkeletonWardenBanner>();
            NPC.RedemptionGuard().GuardPoints = 20;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                string SkeleType = Personality == PersonalityState.Greedy ? "Greedy" : "Epidotrian";

                if (Personality == PersonalityState.Soulful)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                            NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                        Main.dust[dust].velocity *= 5f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, Personality == PersonalityState.Greedy ? DustID.GoldCoin : DustID.Bone,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore2").Type, 1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore").Type, 1);

                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SkeletonWardenGore" + (i + 1)).Type, 1);

                if (Personality == PersonalityState.Greedy)
                {
                    for (int i = 0; i < 8; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, RedeHelper.Spread(2), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                }
            }
            if (Personality == PersonalityState.Greedy && CoinsDropped < 10 && Main.rand.NextBool(3))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<AncientGoldCoin>());
                CoinsDropped++;
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, Personality == PersonalityState.Greedy ? DustID.GoldCoin : DustID.Bone,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

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
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig, 0.1f, true, DustID.WoodFurniture, default, 10, 1, 40);
                blocked = false;
            }
            else
                NPC.HitSound = SoundID.DD2_SkeletonHurt;
            blocked = false;
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardBroken)
                return;

            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            if (NPC.frame.Y >= 13 * 64)
                ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            else
            {
                if (NPC.RightOf(player) && NPC.spriteDirection == 1 || (player.RightOf(NPC) && NPC.spriteDirection == -1))
                    return;
            }

            if (item.noMelee || item.damage <= 0)
                return;

            if (player.Redemption().meleeHitbox.Intersects(ShieldHitbox))
            {
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                {
                    NPC.HitSound = SoundID.DD2_SkeletonHurt with { Volume = 0 };
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig, 0.1f, true, DustID.WoodFurniture, default, 10, 1, 40);
                    blocked = false;
                }
                else
                    NPC.HitSound = SoundID.DD2_SkeletonHurt;
            }
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

            if (!projBlocked.Contains(projectile.whoAmI) && (!projectile.active || !projectile.friendly))
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
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig, 0.1f, true, DustID.WoodFurniture, default, 10, 1, 40);
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
        private int runCooldown;
        private NPC defending;
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            Rectangle ShieldRaisedHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            if (!NPC.RedemptionGuard().GuardBroken)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (!projectile.active || !projectile.friendly || projectile.ProjBlockBlacklist())
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
            if (Main.rand.NextBool(1500) && !Main.dedServ)
                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Ambient"), NPC.position);

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
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
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.9f * SpeedMultiplier, 6, 6, NPC.Center.Y > moveTo.Y * 16);
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
                        if (globalNPC.attacker is not Player || !NPC.PlayerDead() && !(globalNPC.attacker as Player).RedemptionPlayerBuff().skeletonFriendly)
                        {
                            NPC.LookAtEntity(globalNPC.attacker);
                            NPC.velocity.Y = -2;
                            NPC.velocity.X = 5 * NPC.spriteDirection;
                            AITimer = 20;
                            NPC.netUpdate = true;
                        }
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
                            else if (globalNPC.attacker is Player attackerPlayer2)
                            {
                                int hitDirection = attackerPlayer2.RightOfDir(NPC);
                                BaseAI.DamagePlayer(attackerPlayer2, NPC.damage, 11, hitDirection, NPC);
                            }
                        }
                    }

                    moveTo = defending.Center + new Vector2((defending.width + 40) * defending.spriteDirection, 0);
                    if (NPC.Center.X + 20 > moveTo.X && NPC.Center.X - 20 < moveTo.X)
                        AIState = ActionState.Block;

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup with { Pitch = .3f }, NPC.position);
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
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
                        if (globalNPC.attacker is not Player || !NPC.PlayerDead() && !(globalNPC.attacker as Player).RedemptionPlayerBuff().skeletonFriendly)
                        {
                            NPC.LookAtEntity(globalNPC.attacker);
                            NPC.velocity.Y = -2;
                            NPC.velocity.X = 5 * NPC.spriteDirection;
                            AITimer = 20;
                            NPC.netUpdate = true;
                        }
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
                            else if (globalNPC.attacker is Player attackerPlayer3)
                            {
                                int hitDirection = attackerPlayer3.RightOfDir(NPC);
                                BaseAI.DamagePlayer(attackerPlayer3, NPC.damage, 11, hitDirection, NPC);
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
                    if (globalNPC.attacker is Player attackerPlayer && (NPC.PlayerDead() || attackerPlayer.RedemptionPlayerBuff().skeletonFriendly))
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
            }
            if (Personality != PersonalityState.Greedy)
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height,
                DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            RedeNPC globalNPC = NPC.Redemption();

            NPC.frame.X = Personality switch
            {
                PersonalityState.Soulful => NPC.frame.Width * 1,
                PersonalityState.Greedy => NPC.frame.Width * 2,
                _ => 0,
            };
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
                if (NPC.velocity.X == 0)
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
        public int GetNearestNPC(int[] WhitelistNPC = default, bool nearestUndead = false)
        {
            WhitelistNPC ??= new int[] { NPCID.Guide };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == ModContent.NPCType<SkeletonWarden>() || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (nearestUndead && !NPCLists.Undead.Contains(target.type) && !NPCLists.Skeleton.Contains(target.type))
                    continue;

                if (!nearestUndead && !WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
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
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC(nearestUndead: true);
            if (!player.RedemptionPlayerBuff().skeletonFriendly && NPC.Sight(player, VisionRange, HasEyes, HasEyes, false))
            {
                if (AIState != ActionState.Block && !NPC.RedemptionGuard().GuardBroken)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Block;
                    NPC.netUpdate = true;
                }
                if (AIState != ActionState.Retreat && NPC.RedemptionGuard().GuardBroken)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Retreat;
                    NPC.netUpdate = true;
                }
            }
            if (defending == null && gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
            {
                defending = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Defend;
                NPC.netUpdate = true;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new(Main.rand);
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 8);
            choice.Add(PersonalityState.Aggressive, 4);
            choice.Add(PersonalityState.Soulful, 1);
            choice.Add(PersonalityState.Greedy, 0.5);

            Personality = choice;
            if (Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D ShieldTex = ModContent.Request<Texture2D>(Texture + "_Shield").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!NPC.RedemptionGuard().GuardBroken && NPC.frame.Y < 13 * 64)
                spriteBatch.Draw(ShieldTex, NPC.Center - screenPos, null, drawColor, NPC.rotation, NPC.frame.Size() / 2 - new Vector2(0, ShieldOffset.Y), NPC.scale, effects, 0);
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback.Base += 6;
        }
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.6f, 1.4f));
                else
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.8f));
            }
            else if (Main.rand.NextBool(2))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.6f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 4, 1, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 2, 3, 12));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 2));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SkeletonWarden"))
            });
        }
    }
}
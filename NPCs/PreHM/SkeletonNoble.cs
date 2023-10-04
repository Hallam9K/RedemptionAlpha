using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.PreHM.CommonGuard;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Melee;
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
using Terraria.Localization;

namespace Redemption.NPCs.PreHM
{
    public class SkeletonNoble : SkeletonBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Stab,
            Slash
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.damage = 28;
            NPC.friendly = false;
            NPC.defense = 15;
            NPC.lifeMax = 72;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 350;
            NPC.knockBackResist = 0.4f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SkeletonNobleBanner>();
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

                for (int i = 0; i < 10; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SkeletonNobleGore" + (i + 1)).Type, 1);

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
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.NPCHit4, .25f, false, DustID.Web, default, damage: NPC.lifeMax / 4);
            }
        }

        private int runCooldown;

        private int AniFrameY;
        private int AniFrameX;
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;

            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            CustomFrames(70);

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Stab)
                NPC.LookByVelocity();

            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 78 : NPC.Center.X), (int)(NPC.Center.Y - 66), 78, 94);

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

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    if (globalNPC.attacker is Player attackerPlayer && (NPC.PlayerDead() || attackerPlayer.RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 80 * 80)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = Main.rand.NextBool(2) ? ActionState.Stab : ActionState.Slash;
                        NPC.netUpdate = true;
                    }

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup with { Pitch = .3f }, NPC.position);
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, Personality == PersonalityState.Greedy ? new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y) : globalNPC.attacker.Center, 0.2f, 1.7f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1),
                        6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    break;

                case ActionState.Slash:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    NPC.LookAtEntity(globalNPC.attacker);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AniFrameY is 4 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC.immune[NPC.whoAmI] = 10;
                            int hitDirection = attackerNPC.RightOfDir(NPC);
                            BaseAI.DamageNPC(attackerNPC, damage, 9, hitDirection, NPC);
                            if (Main.rand.NextBool(3))
                                attackerNPC.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
                        }
                        else if (globalNPC.attacker is Player attackerPlayer2)
                        {
                            int hitDirection = attackerPlayer2.RightOfDir(NPC);
                            BaseAI.DamagePlayer(attackerPlayer2, damage, 9, hitDirection, NPC);
                            if (Main.rand.NextBool(3))
                                attackerPlayer2.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
                        }
                    }
                    break;

                case ActionState.Stab:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AITimer == 0)
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkeletonNoble_HalberdProj>(), damage,
                            RedeHelper.PolarVector(8, (globalNPC.attacker.Center - NPC.Center).ToRotation()), SoundID.Item1, NPC.whoAmI,
                            Personality == PersonalityState.Greedy ? 1 : 0);
                        AITimer = 1;
                    }
                    break;
            }
            if (Personality != PersonalityState.Greedy)
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height,
                DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        private void CustomFrames(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.Stab:
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 10)
                        NPC.frame.Y = 13 * frameHeight;
                    else if (NPC.frameCounter < 20)
                        NPC.frame.Y = 14 * frameHeight;
                    else if (NPC.frameCounter < 40)
                        NPC.frame.Y = 15 * frameHeight;
                    else
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Alert;
                    }
                    if (AIState is ActionState.Slash)
                        HeadOffset = SetHeadOffsetY();
                    else
                        HeadOffset = SetHeadOffset(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;

                case ActionState.Slash:
                    if (++NPC.frameCounter >= 7)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY is 1)
                            SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                        if (AniFrameY is 4)
                        {
                            SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;
                            NPC.velocity.X = 2 * NPC.spriteDirection;
                        }
                        if (AniFrameY > 5)
                        {
                            AniFrameY = 0;
                            AIState = ActionState.Alert;
                        }
                    }
                    if (AIState is ActionState.Slash)
                        HeadOffset = SetHeadOffsetY();
                    else
                        HeadOffset = SetHeadOffset(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private int HeadOffsetX;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            NPC.frame.X = Personality switch
            {
                PersonalityState.Soulful => NPC.frame.Width,
                PersonalityState.Greedy => NPC.frame.Width * 2,
                _ => 0,
            };
            AniFrameX = Personality switch
            {
                PersonalityState.Soulful => 1,
                PersonalityState.Greedy => 2,
                _ => 0,
            };
            HeadX = Personality switch
            {
                PersonalityState.Greedy => 1,
                _ => 0,
            };
            if (AIState is ActionState.Slash or ActionState.Stab)
                return;
            AniFrameY = 0;

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
            if (AIState is ActionState.Slash)
                HeadOffset = SetHeadOffsetY();
            else
                HeadOffset = SetHeadOffset(ref frameHeight);
            HeadOffsetX = SetHeadOffsetX(ref frameHeight);
        }
        public override int SetHeadOffset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                1 => -2,
                2 => -2,
                3 => -2,
                5 => -2,
                8 => -2,
                9 => -2,
                12 => -2,
                14 => -2,
                _ => 0,
            };
        }
        public int SetHeadOffsetY()
        {
            return AniFrameY switch
            {
                1 => -2,
                2 => -2,
                5 => 4,
                _ => 0,
            };
        }
        public int SetHeadOffsetX(ref int frameHeight)
        {
            if (AIState is ActionState.Slash)
            {
                return AniFrameY switch
                {
                    1 => 2,
                    4 => -8,
                    5 => -2,
                    _ => 0,
                };
            }
            else
            {
                return (NPC.frame.Y / frameHeight) switch
                {
                    4 => -2,
                    8 => -2,
                    12 => -2,
                    14 => -10,
                    15 => -2,
                    _ => 0,
                };
            }
        }
        public int GetNearestNPC(int[] WhitelistNPC = default, bool friendly = false)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.Guide };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (friendly)
                {
                    if (target.friendly || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type] || NPCLists.SkeletonHumanoid.Contains(target.type))
                        continue;
                }
                else
                {
                    if (!WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
                        continue;
                }

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
            int gotNPC = GetNearestNPC(!HasEyes ? (Personality == PersonalityState.Aggressive ? NPCLists.HasLostSoul.ToArray() :
                new int[] { ModContent.NPCType<LostSoulNPC>() }) : default);
            if (Personality != PersonalityState.Calm)
            {
                if (!player.RedemptionPlayerBuff().skeletonFriendly && NPC.Sight(player, VisionRange, HasEyes, HasEyes, false))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
                if (!HasEyes && Personality == PersonalityState.Aggressive && Main.rand.NextBool(1800))
                {
                    if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, false, false))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                        globalNPC.attacker = Main.npc[gotNPC];
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = ActionState.Alert;
                        NPC.netUpdate = true;
                    }
                    return;
                }
                gotNPC = GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default);

                if (player.RedemptionPlayerBuff().skeletonFriendly)
                    gotNPC = GetNearestNPC(friendly: true);

                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<int> head = new(Main.rand);
            head.Add(0); // 24%
            head.Add(1, 0.6); // 14.5%
            head.Add(2, 0.6);
            head.Add(3, 0.4); // 9.6%
            head.Add(4, 0.4);
            head.Add(5, 0.1); // 2.4%
            head.Add(6, 0.1);
            head.Add(7, 0.1);
            head.Add(8, 0.06); // 1.4%
            head.Add(9, 0.06);
            head.Add(10, 0.06);
            head.Add(11, 0.06);
            head.Add(12, 0.3); // 7.2%
            head.Add(13, 0.3);
            HeadType = head;

            if (NPC.ai[3] == 0)
            {
                WeightedRandom<PersonalityState> choice = new(Main.rand);
                choice.Add(PersonalityState.Normal, 10);
                choice.Add(PersonalityState.Calm, 1);
                choice.Add(PersonalityState.Aggressive, 7);
                choice.Add(PersonalityState.Soulful, 1);
                choice.Add(PersonalityState.Greedy, 0.5);

                Personality = choice;
            }
            if (Main.rand.NextBool(2) || Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int HeightH = head.Value.Height / 14;
            int WidthH = head.Value.Width / 2;
            int yH = HeightH * HeadType;
            int xH = WidthH * HeadX;
            Rectangle rectH = new(xH, yH, WidthH, HeightH);

            if (AIState is ActionState.Slash)
            {
                spriteBatch.Draw(head.Value, NPC.Center - screenPos, new Rectangle?(rectH), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -35 : -35) + (HeadOffsetX * NPC.spriteDirection), -6 + -HeadOffset), NPC.scale, effects, 0);

                int Height = NobleSlashAni.Value.Height / 6;
                int Width = NobleSlashAni.Value.Width / 3;
                int y = Height * AniFrameY;
                int x = Width * AniFrameX;
                Rectangle rect = new(x, y, Width, Height);
                Vector2 origin = new(Width / 2f, Height / 2f);
                spriteBatch.Draw(NobleSlashAni.Value, NPC.Center - screenPos - new Vector2(0, 17), new Rectangle?(rect), drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

                if (HasEyes && HeadType < 12)
                    spriteBatch.Draw(NobleSlashGlow.Value, NPC.Center - screenPos - new Vector2(0, 17), new Rectangle?(rect), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(head.Value, NPC.Center - screenPos, new Rectangle?(rectH), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -36 : -34) + (HeadOffsetX * NPC.spriteDirection), -4 + HeadOffset), NPC.scale, effects, 0);

                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                if (HasEyes && HeadType < 12)
                    spriteBatch.Draw(Glow, NPC.Center - screenPos - new Vector2(0, 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.8f, 2.2f));
                else
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.4f, 1.2f));
            }
            else if (Main.rand.NextBool(2))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.8f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NoblesHalberd>(), 25));
            npcLoot.Add(ItemDropRule.OneFromOptions(25, ModContent.ItemType<CommonGuardHelm1>(), ModContent.ItemType<CommonGuardHelm2>(),
                ModContent.ItemType<CommonGuardPlateMail>(), ModContent.ItemType<CommonGuardGreaves>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 3, 3, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 2, 9, 18));
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

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SkeletonNoble"))
            });
        }
    }
}
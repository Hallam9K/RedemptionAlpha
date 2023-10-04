using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
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
using Terraria.Localization;
using Redemption.Items.Weapons.PreHM.Ritualist;

namespace Redemption.NPCs.PreHM
{
    public class SkeletonAssassin : SkeletonBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Hiding,
            Stalk,
            Alert,
            Stab
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
            NPC.width = 24;
            NPC.height = 50;
            NPC.damage = 23;
            NPC.friendly = false;
            NPC.defense = 8;
            NPC.lifeMax = 58;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 120;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SkeletonAssassinBanner>();
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
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SkeletonAssassinGore1").Type, 1);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SkeletonAssassinGore2").Type, 1);

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

            if (AIState is ActionState.Idle or ActionState.Wander or ActionState.Hiding or ActionState.Stalk)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;

            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            AIState = Main.rand.NextBool(2) ? ActionState.Hiding : ActionState.Idle;
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            CustomFrames(56);

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Stab)
                NPC.LookByVelocity();
            Rectangle KnifeHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 46 : NPC.Center.X + 10), (int)(NPC.Center.Y - 12), 36, 18);

            if (Main.rand.NextBool(3500) && !Main.dedServ)
                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Ambient"), NPC.position);

            if (AIState is ActionState.Hiding or ActionState.Stalk)
            {
                if (NPC.alpha < 240)
                    NPC.alpha += 4;
            }
            else
            {
                if (NPC.alpha > 0)
                    NPC.alpha -= 8;
            }
            if (NPC.alpha > 200)
                NPC.Redemption().invisible = true;

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
                        if (Main.rand.NextBool(3))
                        {
                            TimerRand = Main.rand.Next(120, 260);
                            AIState = ActionState.Wander;
                        }
                        else
                        {
                            TimerRand = Main.rand.Next(80, 280);
                            AIState = ActionState.Hiding;
                        }
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Hiding:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    AITimer++;
                    if (AITimer >= TimerRand * 6)
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
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Stalk:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }
                    if (globalNPC.attacker is Player attackerPlayer && (NPC.PlayerDead() || attackerPlayer.RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }

                    if (HasEyes && globalNPC.attacker.direction == NPC.direction * -1 && NPC.Sight(globalNPC.attacker, 200, true, true))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 50 * 50)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 3 * NPC.spriteDirection;
                        AIState = ActionState.Stab;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange + 200, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 0.8f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    if (globalNPC.attacker is Player attackerPlayer2 && (NPC.PlayerDead() || attackerPlayer2.RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 50 * 50)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 3 * NPC.spriteDirection;
                        AIState = ActionState.Stab;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup with { Pitch = .3f }, NPC.position);
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, Personality == PersonalityState.Greedy ? new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y) : globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    break;

                case ActionState.Stab:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = NPC.alpha < 10 ? ActionState.Wander : ActionState.Hiding;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    AITimer++;

                    if (AITimer == 10)
                        SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                    if (AITimer >= 10 && globalNPC.attacker.Hitbox.Intersects(KnifeHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC.immune[NPC.whoAmI] = 20;
                            int hitDirection = attackerNPC.RightOfDir(NPC);
                            BaseAI.DamageNPC(attackerNPC, damage * (NPC.alpha > 50 ? 2 : 1), 4, hitDirection, NPC);
                            if (Main.rand.NextBool(3))
                                attackerNPC.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
                        }
                        else if (globalNPC.attacker is Player attackerPlayer3)
                        {
                            int hitDirection = attackerPlayer3.RightOfDir(NPC);
                            BaseAI.DamagePlayer(attackerPlayer3, damage * (NPC.alpha > 50 ? 2 : 1), 4, hitDirection, NPC);
                            if (Main.rand.NextBool(3) && globalNPC.attacker is Player)
                                attackerPlayer3.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
                        }
                    }
                    break;
            }
            if (Personality != PersonalityState.Greedy || NPC.alpha > 50)
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height,
                DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        private void CustomFrames(int frameHeight)
        {
            if (AIState is ActionState.Stab)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                    NPC.frame.Y = 13 * frameHeight;
                else if (NPC.frameCounter < 20)
                    NPC.frame.Y = 14 * frameHeight;
                else if (NPC.frameCounter < 30)
                    NPC.frame.Y = 15 * frameHeight;
                else
                {
                    AIState = ActionState.Alert;
                    NPC.frameCounter = 0;
                }
                return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
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

            if (AIState is ActionState.Stab)
                return;
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
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Stalk;
                    NPC.netUpdate = true;
                }
                if (!HasEyes && Personality == PersonalityState.Aggressive && Main.rand.NextBool(1800))
                {
                    if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, false, false))
                    {
                        globalNPC.attacker = Main.npc[gotNPC];
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = ActionState.Stalk;
                        NPC.netUpdate = true;
                    }
                    return;
                }
                gotNPC = GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default);

                if (player.RedemptionPlayerBuff().skeletonFriendly)
                    gotNPC = GetNearestNPC(friendly: true);

                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
                {
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Stalk;
                    NPC.netUpdate = true;
                }
            }
        }
        public void ChoosePersonality()
        {
            if (NPC.ai[3] == 0)
            {
                WeightedRandom<PersonalityState> choice = new(Main.rand);
                choice.Add(PersonalityState.Normal, 10);
                choice.Add(PersonalityState.Calm, 3);
                choice.Add(PersonalityState.Aggressive, 8);
                choice.Add(PersonalityState.Soulful, 1);
                choice.Add(PersonalityState.Greedy, 0.5);

                Personality = choice;
            }
            if (Main.rand.NextBool(3) || Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.6f, 1.2f));
                else
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.6f));
            }
            else if (Main.rand.NextBool(3))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 3, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 3, 3, 9));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 3));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SkeletonAssassin"))
            });
        }
    }
}
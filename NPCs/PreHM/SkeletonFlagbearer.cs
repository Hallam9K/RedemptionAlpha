using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
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

namespace Redemption.NPCs.PreHM
{
    public class SkeletonFlagbearer : SkeletonBase
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Retreat,
            Follow,
            Dance
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 46;
            NPC.damage = 10;
            NPC.friendly = false;
            NPC.defense = 9;
            NPC.lifeMax = 46;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 140;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
        }
        public override void HitEffect(int hitDirection, double damage)
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
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore" + (i + 1)).Type, 1);

                if (Personality == PersonalityState.Greedy)
                {
                    for (int i = 0; i < 8; i++)
                        Gore.NewGore(NPC.position, RedeHelper.Spread(2), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                }
            }
            if (Personality == PersonalityState.Greedy && CoinsDropped < 10 && Main.rand.NextBool(3))
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<AncientGoldCoin>());
                CoinsDropped++;
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, Personality == PersonalityState.Greedy ? DustID.GoldCoin : DustID.Bone,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                AIState = ActionState.Retreat;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Begin:
                    ChoosePersonality();
                    SetStats();

                    TimerRand = Main.rand.Next(80, 280);
                    AIState = ActionState.Idle;
                    break;

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
                    }

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Retreat:
                    SightCheck();
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                    }
                    if (globalNPC.attacker is Player && (PlayerDead() || (globalNPC.attacker as Player).GetModPlayer<BuffPlayer>().skeletonFriendly))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 100 : NPC.Center.X - 100, NPC.Center.Y), 0.4f, 2.4f * SpeedMultiplier * (NPC.GetGlobalNPC<BuffNPC>().rallied ? 1.1f : 1), 12, 8, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Follow:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 140 * 140)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Dance;
                    }

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup, (int)NPC.position.X, (int)NPC.position.Y, 1, 0.3f);
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2.4f * SpeedMultiplier * (NPC.GetGlobalNPC<BuffNPC>().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y);

                    break;

                case ActionState.Dance:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || !AttackerIsUndead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                    }

                    NPC.LookAtEntity(globalNPC.attacker);

                    NPC.velocity *= 0;

                    AITimer++;
                    if (AITimer % 60 == 0)
                    {
                        DustHelper.DrawCircle(NPC.Center, DustID.ViciousPowder, 16, 2, 2, nogravity: true);
                        foreach (NPC target in Main.npc.Take(Main.maxNPCs))
                        {
                            if (!target.active || target.whoAmI == NPC.whoAmI)
                                continue;

                            if (!NPCTags.Undead.Has(target.type) && !NPCTags.Skeleton.Has(target.type))
                                continue;

                            if (NPC.DistanceSQ(target.Center) > 300 * 300)
                                continue;

                            target.AddBuff(ModContent.BuffType<FlagbearerBuff>(), 600);
                        }
                    }

                    if (NPC.DistanceSQ(globalNPC.attacker.Center) >= 300 * 300)
                    {
                        NPC.frameCounter = 0;
                        AIState = ActionState.Follow;
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
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 6;
            NPC.frame.X = Personality switch
            {
                PersonalityState.Soulful => NPC.frame.Width * (AIState is ActionState.Dance ? 3 : 2),
                PersonalityState.Greedy => NPC.frame.Width * (AIState is ActionState.Dance ? 5 : 4),
                _ => AIState is ActionState.Dance ? NPC.frame.Width : 0,
            };
            if (AIState is ActionState.Dance)
            {
                if (++NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 7 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
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
        }
        public int GetNearestNPC(int[] WhitelistNPC = default, bool nearestUndead = false)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.TargetDummy };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == ModContent.NPCType<SkeletonFlagbearer>() || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (nearestUndead && !NPCTags.Undead.Has(target.type) && !NPCTags.Skeleton.Has(target.type))
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
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            int gotNPC = GetNearestNPC(nearestUndead: true);
            if (AIState != ActionState.Retreat && !player.GetModPlayer<BuffPlayer>().skeletonFriendly && NPC.Sight(player, VisionRange, HasEyes, HasEyes))
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Retreat;
            }
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Follow;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new(Main.rand);
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 6);
            choice.Add(PersonalityState.Aggressive, 7);
            choice.Add(PersonalityState.Soulful, 2);
            choice.Add(PersonalityState.Greedy, 1);

            Personality = choice;
            if (Main.rand.NextBool(3) || Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/" + GetType().Name + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 33), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos - new Vector2(0, 33), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.4f, 1.2f));
                else
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
            }
            else if (Main.rand.NextBool(3))
                RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.2f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 2, 1, 6));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 50));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OldTophat>(), 500));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 3));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(
                    "Every squad needs a guy holding a big flag. This stubby skeleton's dance can rally any undead who lay witness! Non-undead just look at the silly dance with embarrassment...")
            });
        }
    }
}
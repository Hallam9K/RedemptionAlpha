using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Hostile;
using Redemption.Tiles.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class EpidotrianSkeleton : ModNPC
    {
        public enum PersonalityState
        {
            Normal, Aggressive, Calm, Greedy, Soulful
        }
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert
        }

        private bool HasEyes;
        private int CoinsDropped;

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public PersonalityState Personality
        {
            get => (PersonalityState)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.Skeletons[NPC.type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 18;
            NPC.friendly = false;
            NPC.defense = 7;
            NPC.lifeMax = 54;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 95;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
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
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 2);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        private int VisionRange;
        private int VisionIncrease;
        private float SpeedMultiplier = 1f;
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

                case ActionState.Alert:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (Personality != PersonalityState.Greedy)
                        NPC.DamageHostileAttackers(0, 4, HasEyes ? default : NPCLists.HasLostSoul);

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup, (int)NPC.position.X, (int)NPC.position.Y, 1, 0.3f);
                        Gore.NewGore(NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, Personality == PersonalityState.Greedy ? new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 100
                        : NPC.Center.X - 100, NPC.Center.Y) : globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);

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
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.X = Personality switch
            {
                PersonalityState.Soulful => NPC.frame.Width,
                PersonalityState.Greedy => NPC.frame.Width * 2,
                _ => 0,
            };

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
        public int GetNearestNPC(int[] WhitelistNPC = default)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.TargetDummy };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            foreach (NPC target in Main.npc)
            {
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (!WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
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
            GetNearestNPC(!HasEyes ? (Personality == PersonalityState.Aggressive ? NPCLists.HasLostSoul.ToArray() :
                new int[] { ModContent.NPCType<LostSoulNPC>() }) : default);
            if (Personality != PersonalityState.Calm)
            {
                if (NPC.Sight(player, VisionRange, HasEyes, HasEyes))
                {
                    SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
                if (!HasEyes && Personality == PersonalityState.Aggressive && Main.rand.NextBool(500))
                {
                    if (NPC.Sight(Main.npc[GetNearestNPC(NPCLists.HasLostSoul.ToArray())], VisionRange, false, false))
                    {
                        SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                        globalNPC.attacker = Main.npc[GetNearestNPC(NPCLists.HasLostSoul.ToArray())];
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }
                    return;
                }
                if (NPC.Sight(Main.npc[GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default)], VisionRange, HasEyes, HasEyes))
                {
                    SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                    globalNPC.attacker = Main.npc[GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default)];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new();
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 8);
            choice.Add(PersonalityState.Aggressive, 8);
            choice.Add(PersonalityState.Soulful, 2);
            choice.Add(PersonalityState.Greedy, 1);

            Personality = choice;
            if (Main.rand.NextBool(3) || Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public void SetStats()
        {
            switch (Personality)
            {
                case PersonalityState.Calm:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                    NPC.life = (int)(NPC.life * 0.9f);
                    NPC.damage = (int)(NPC.damage * 0.8f);
                    SpeedMultiplier = 0.8f;
                    break;
                case PersonalityState.Aggressive:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.05f);
                    NPC.life = (int)(NPC.life * 1.05f);
                    NPC.damage = (int)(NPC.damage * 1.05f);
                    NPC.value = (int)(NPC.value * 1.25f);
                    VisionIncrease = 100;
                    SpeedMultiplier = 1.1f;
                    break;
                case PersonalityState.Soulful:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.4f);
                    NPC.life = (int)(NPC.life * 1.4f);
                    NPC.defense = (int)(NPC.defense * 1.15f);
                    NPC.damage = (int)(NPC.damage * 1.25f);
                    NPC.value *= 2;
                    VisionIncrease = 300;
                    SpeedMultiplier = 1.3f;
                    break;
                case PersonalityState.Greedy:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.2f);
                    NPC.life = (int)(NPC.life * 1.2f);
                    NPC.defense = (int)(NPC.defense * 1.25f);
                    NPC.damage = (int)(NPC.damage * 0.6f);
                    NPC.value = 4;
                    SpeedMultiplier = 1.8f;
                    break;
            }
            if (HasEyes)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.1f);
                NPC.life = (int)(NPC.life * 1.1f);
                NPC.defense = (int)(NPC.defense * 1.05f);
                NPC.damage = (int)(NPC.damage * 1.05f);
                NPC.value = (int)(NPC.value * 1.1f);
                VisionRange = 600 + VisionIncrease;
            }
            else
                VisionRange = 200 + VisionIncrease;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/" + GetType().Name + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override bool? CanHitNPC(NPC target) => AIState == ActionState.Alert && Personality != PersonalityState.Greedy;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Alert && Personality != PersonalityState.Greedy;
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.6f, 1.2f));
                else
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.6f));
            }
            else if (Main.rand.NextBool(3))
                RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 1, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ItemID.BoneSword, 204));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 3));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(
                    "These skeletons are from Epidotra's mainland, they are slightly taller and smarter than the Terrarian ones. Their strength is dependent on their soul, which is also known as Willpower.")
            });
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Globals.Player;
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

namespace Redemption.NPCs.PreHM
{
    public class EpidotrianSkeleton : SkeletonBase
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert
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
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();
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
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore2").Type, 1);

                Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore").Type, 1);

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
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
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

            if (Main.rand.NextBool(500) && !Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + SoundString + "Ambient"), NPC.position);

            switch (AIState)
            {
                case ActionState.Begin:
                    ChoosePersonality();
                    SetStats();

                    TimerRand = Main.rand.Next(80, 280);
                    AIState = ActionState.Idle;
                    NPC.alpha = 0;
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
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    if (globalNPC.attacker is Player && (PlayerDead() || (globalNPC.attacker as Player).GetModPlayer<BuffPlayer>().skeletonFriendly))
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
                        if (Main.netMode != NetmodeID.Server)
                            Gore.NewGore(NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, Personality == PersonalityState.Greedy ? new Vector2(globalNPC.attacker.Center.X < NPC.Center.X ? NPC.Center.X + 100
                        : NPC.Center.X - 100, NPC.Center.Y) : globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier * (NPC.GetGlobalNPC<BuffNPC>().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);

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
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
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
        }

        public int GetNearestNPC(int[] WhitelistNPC = default, bool friendly = false)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.TargetDummy };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (friendly)
                {
                    if (target.friendly || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type] || NPCTags.SkeletonHumanoid.Has(target.type))
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
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            int gotNPC = GetNearestNPC(!HasEyes ? (Personality == PersonalityState.Aggressive ? NPCLists.HasLostSoul.ToArray() :
                new int[] { ModContent.NPCType<LostSoulNPC>() }) : default);
            if (Personality != PersonalityState.Calm)
            {
                if (!player.GetModPlayer<BuffPlayer>().skeletonFriendly && NPC.Sight(player, VisionRange, HasEyes, HasEyes))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
                if (!HasEyes && Personality == PersonalityState.Aggressive && Main.rand.NextBool(1800))
                {
                    if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, false, false))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                        globalNPC.attacker = Main.npc[gotNPC];
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }
                    return;
                }
                gotNPC = GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default);

                if (player.GetModPlayer<BuffPlayer>().skeletonFriendly)
                    gotNPC = GetNearestNPC(friendly: true);

                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + SoundString + "Notice"), NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new(Main.rand);
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 8);
            choice.Add(PersonalityState.Aggressive, 8);
            if (TimerRand == 0)
            {
                choice.Add(PersonalityState.Soulful, 1);
                choice.Add(PersonalityState.Greedy, 0.5);
            }

            Personality = choice;
            if (Main.rand.NextBool(3) || Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 4, 1, 2));
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(
                    "These skeletons are from Epidotra's mainland, they are slightly taller and smarter than the island's ones. Their strength is dependent on their soul, which is also known as Willpower.")
            });
        }
    }
}
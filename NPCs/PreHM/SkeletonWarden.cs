using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
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
    public class SkeletonWarden : SkeletonBase
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Block,
            Slam,
            Defend
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
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
                AITimer = 0;
                AIState = ActionState.Block;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        private NPC defending;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            NPC.TargetClosest();
            NPC.LookByVelocity();
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 26 : NPC.Center.X + 8), (int)(NPC.Center.Y - 22), 16, 52);
            Rectangle ShieldRaisedHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X - 22), (int)(NPC.Center.Y - 32), 52, 22);
            foreach (Projectile projectile in Main.projectile)
            {
                if (!projectile.active || !projectile.friendly || projectile.penetrate == -1 || projectile.damage > 60 || ProjectileLists.IsTechnicallyMelee.Contains(projectile.type))
                    continue;

                if (NPC.frame.Y >= 13 * 64)
                {
                    if (projectile.Hitbox.Intersects(ShieldRaisedHitbox))
                    {
                        projectile.friendly = false;
                        projectile.Kill();
                    }
                }
                else
                {
                    if (projectile.Hitbox.Intersects(ShieldHitbox))
                    {
                        projectile.friendly = false;
                        projectile.Kill();
                    }
                }
            }

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
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.9f * SpeedMultiplier, 6, 6, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Defend:
                    if (defending == null || !defending.active || globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes) && !NPC.Sight(defending, VisionRange, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(globalNPC.attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        if ((NPC.frame.Y >= 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldRaisedHitbox)) ||
                            (NPC.frame.Y < 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldHitbox)))
                        {
                            if (globalNPC.attacker is NPC && (globalNPC.attacker as NPC).immune[NPC.whoAmI] <= 0)
                            {
                                (globalNPC.attacker as NPC).immune[NPC.whoAmI] = 25;
                                int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                                BaseAI.DamageNPC(globalNPC.attacker as NPC, NPC.damage, 11, hitDirection, NPC);
                            }
                            else if (globalNPC.attacker is Player)
                            {
                                int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                                BaseAI.DamagePlayer(globalNPC.attacker as Player, NPC.damage, 11, hitDirection, NPC);
                            }
                        }
                    }

                    moveTo = defending.Center + new Vector2((defending.width + 40) * defending.spriteDirection, 0);
                    if (NPC.Center.X + 20 > moveTo.X && NPC.Center.X - 20 < moveTo.X)
                        AIState = ActionState.Block;

                    if (Personality == PersonalityState.Greedy && Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.CoinPickup, (int)NPC.position.X, (int)NPC.position.Y, 1, 0.3f);
                        Gore.NewGore(NPC.position, RedeHelper.Spread(1), ModContent.Find<ModGore>("Redemption/AncientCoinGore").Type, 1);
                    }
                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo, 0.2f,
                        2.4f * SpeedMultiplier * (NPC.GetGlobalNPC<BuffNPC>().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y);

                    break;

                case ActionState.Block:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 360)
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                    }

                    SightCheck();

                    if (NPC.velocity.Y < 0 && AITimer == 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0 && AITimer == 0)
                        NPC.velocity.X = 0;

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes) && !NPC.Sight(defending, VisionRange, false, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(80) && NPC.DistanceSQ(globalNPC.attacker.Center) < 100 * 100)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.velocity.Y = -2;
                        NPC.velocity.X = 5 * NPC.spriteDirection;
                        AITimer = 20;
                    }
                    if (AITimer > 0)
                    {
                        AITimer--;
                        if ((NPC.frame.Y >= 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldRaisedHitbox)) ||
                            (NPC.frame.Y < 13 * 64 && globalNPC.attacker.Hitbox.Intersects(ShieldHitbox)))
                        {
                            if (globalNPC.attacker is NPC && (globalNPC.attacker as NPC).immune[NPC.whoAmI] <= 0)
                            {
                                (globalNPC.attacker as NPC).immune[NPC.whoAmI] = 25;
                                int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                                BaseAI.DamageNPC(globalNPC.attacker as NPC, NPC.damage, 11, hitDirection, NPC);
                            }
                            else if (globalNPC.attacker is Player)
                            {
                                int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                                BaseAI.DamagePlayer(globalNPC.attacker as Player, NPC.damage, 11, hitDirection, NPC);
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
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();

            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.X = Personality switch
            {
                PersonalityState.Soulful => NPC.frame.Width * 1,
                PersonalityState.Greedy => NPC.frame.Width * 2,
                _ => 0,
            };
            if (AIState is ActionState.Block && NPC.velocity.Length() == 0 && globalNPC.attacker.Center.Y < NPC.Center.Y - NPC.height + 40 && globalNPC.attacker.Center.X > NPC.Center.X - 100 &&
                globalNPC.attacker.Center.X < NPC.Center.X + 100)
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
        }
        public int GetNearestNPC(int[] WhitelistNPC = default, bool nearestUndead = false)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.TargetDummy };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            foreach (NPC target in Main.npc.Take(Main.maxNPCs))
            {
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == ModContent.NPCType<SkeletonWarden>() || target.dontTakeDamage || target.type == NPCID.OldMan)
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
            if (AIState != ActionState.Block && NPC.Sight(player, VisionRange, HasEyes, HasEyes))
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Block;
            }
            if (defending == null && gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes))
            {
                defending = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Defend;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new();
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 8);
            choice.Add(PersonalityState.Aggressive, 4);
            choice.Add(PersonalityState.Soulful, 2);
            choice.Add(PersonalityState.Greedy, 1);

            Personality = choice;
            if (Personality == PersonalityState.Soulful)
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/" + GetType().Name + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                if (Personality == PersonalityState.Soulful)
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.6f, 1.4f));
                else
                    RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.8f));
            }
            else if (Main.rand.NextBool(2))
                RedeHelper.SpawnNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.6f));
            if (Personality == PersonalityState.Greedy)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(6, 12));
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 1, 1, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrassboneShards>(), 2, 2, 8));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ItemID.BoneSword, 204));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 2));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(
                    "A blindfolded skeleton from Anglon. It defends other undead with its tower shield and slams any attackers that get too close.")
            });
        }
    }
}
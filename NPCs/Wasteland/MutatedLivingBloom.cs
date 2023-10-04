using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Plants;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Armor.Vanity.Intruder;
using System.IO;
using Redemption.NPCs.PreHM;
using Terraria.Localization;

namespace Redemption.NPCs.Wasteland
{
    public class MutatedLivingBloom : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Threatened,
            RootAttack
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = ModContent.NPCType<LivingBloom>();
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 56;
            NPC.defense = 13;
            NPC.damage = 60;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 800;
            NPC.knockBackResist = 0.4f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MutatedLivingBloomBanner>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
        public NPC npcTarget;
        public Vector2 moveTo;
        public int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 180);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RegenCheck();
            if ((globalNPC.attacker is Player || (globalNPC.attacker is NPC spirit && spirit.Redemption().spiritSummon)) && AIState > ActionState.Wander)
                NPC.chaseable = true;

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0 && !NPCLists.Plantlike.Contains(npcTarget.type) && !npcTarget.Redemption().invisible)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Threatened;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Wander:
                    if (NPC.ClosestNPCToNPC(ref npcTarget, 160, NPC.Center) && npcTarget.lifeMax > 5 && npcTarget.damage > 0 && !NPCLists.Plantlike.Contains(npcTarget.type) && !npcTarget.Redemption().invisible)
                    {
                        globalNPC.attacker = npcTarget;
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Threatened;
                        NPC.netUpdate = true;
                    }

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1, 6, 4, false);
                    break;

                case ActionState.Threatened:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 1))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, -1, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPCHelper.HorizontallyMove(NPC, new Vector2(NPC.Center.X + (50 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y),
                        0.5f, 2, 6, 4, false);

                    if (Main.rand.NextBool(200) && NPC.velocity.Y == 0)
                    {
                        AITimer = 0;
                        AIState = ActionState.RootAttack;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.RootAttack:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 1))
                        AIState = ActionState.Wander;

                    for (int i = 0; i < 2; i++)
                    {
                        int dustIndex = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.LifeDrain);
                        Main.dust[dustIndex].velocity.Y -= 4f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer == 5)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30)) / 16, (int)(globalNPC.attacker.Center.Y / 16) - 2);
                        NPC.Shoot(new Vector2(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30), (tilePosY * 16) + 30), ModContent.ProjectileType<MutatedLivingBloomRoot>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || target.whoAmI == NPC.whoAmI || target.whoAmI == globalNPC.attacker.whoAmI || target.Redemption().invisible)
                                continue;

                            if (target.lifeMax < 5 || target.damage == 0 || NPC.DistanceSQ(target.Center) > 400 * 400 || target.type == NPC.type)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<MutatedLivingBloomRoot>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                        }
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            Player target = Main.player[p];
                            if (globalNPC.attacker is NPC)
                                continue;

                            if (!target.active || NPC.DistanceSQ(target.Center) > 400 * 400)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<MutatedLivingBloomRoot>(), NPC.damage, Vector2.Zero, NPC.whoAmI);
                        }
                    }
                    else if (AITimer >= 80)
                    {
                        AIState = ActionState.Threatened;
                    }
                    break;
            }
        }

        int regenTimer;
        public void RegenCheck()
        {
            int regenCooldown = NPC.wet && !NPC.lavaWet ? 30 : 40;
            if ((NPC.wet && !NPC.lavaWet) || NPC.HasBuff(BuffID.Wet) || (Main.raining && NPC.position.Y < Main.worldSurface && Framing.GetTileSafely(NPC.Center).WallType == WallID.None))
            {
                regenTimer++;
                if (regenTimer % regenCooldown == 0 && NPC.life < NPC.lifeMax)
                {
                    NPC.life += 5;
                    NPC.HealEffect(5);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.RootAttack:
                    NPC.frame.Y = 7 * frameHeight;
                    return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 4 * frameHeight;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 5 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                if (NPC.velocity.Y < 0)
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    if (++NPC.frameCounter >= 3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 9 * frameHeight;
                    }
                }
                else
                    NPC.frame.Y = 10 * frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.OneFromOptions(2,
                new int[] { ItemID.Daybloom, ItemID.Blinkroot, ItemID.Moonglow, ItemID.Waterleaf, ModContent.ItemType<Nightshade>() }));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnglonicMysticBlossom>(), 100));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.MutatedLivingBloom"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = ActionState.Threatened;
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/MutatedLivingBloomGore" + (i + 1)).Type, 1);

                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Ash,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Ash, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}
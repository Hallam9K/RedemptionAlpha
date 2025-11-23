using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items.Critters;
using Redemption.Items.Placeable.Banners;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Critters
{
    public class Forret : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert
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
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 30;
            NPC.lifeMax = 24;
            NPC.damage = 18;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<ForretItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ForretBanner>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Forret"))
            });
        }
        private int runCooldown;
        public Vector2 moveTo;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();

            NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0f;

                    AITimer++;

                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
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
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 12, 6, false);
                    break;

                case ActionState.Alert:
                    if (Main.rand.NextBool(10))
                        SightCheck();

                    if (NPC.ThreatenedCheck(ref runCooldown, 180))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }
                    AITimer--;
                    if (NPC.Sight(globalNPC.attacker, 80, true, true) && AITimer <= 0 && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        NPC.velocity.X = 8f * globalNPC.attacker.RightOfDir(NPC);
                        NPC.velocity.Y = -Main.rand.NextFloat(2f, 5f);
                        AITimer = Main.rand.Next(120, 181);
                        NPC.netUpdate = true;
                    }
                    if (NPC.velocity.Y != 0 && Math.Abs(NPC.velocity.X) > 5)
                        NPC.GetGlobalNPC<HitboxNPC>().DamageInHitbox(NPC, NPC.Hitbox, NPC.damage, 4.5f);

                    if (!NPC.Sight(globalNPC.attacker, 260 + 100, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
                    NPCHelper.HorizontallyMove(NPC, NPCHelper.RunAwayVector(NPC, globalNPC.attacker), 0.2f, 3.5f, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;

        public void SightCheck()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            NPC target = null;
            if (NPC.Sight(player, 260, true, true) && !player.RedemptionPlayerBuff().devilScented)
            {
                globalNPC.attacker = player;
                if (AIState != ActionState.Alert)
                {
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
            }
            if (RedeHelper.ClosestNPCToNPC(NPC, ref target, 260, NPC.Center) && NPC.Sight(target, 260, true, true))
            {
                globalNPC.attacker = target;
                if (AIState != ActionState.Alert)
                {
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 0;
                        if (Main.rand.NextBool(20))
                            NPC.frame.Y = frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.75f;
                    if (NPC.frameCounter is >= 5 or <= -5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 5 * frameHeight)
                            NPC.frame.Y = 2 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDaySnowCritter.Chance * 1.8f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState is not ActionState.Alert)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 3; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ForretGore" + (i + 1)).Type);
                }
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.BorealWood,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.BrownMoss,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}
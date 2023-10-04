using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Armor.Single;
using Redemption.Items.Critters;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class Kabucra : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Hop,
            Hide
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
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 20;
            NPC.defense = 5;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit38;
            NPC.DeathSound = SoundID.NPCDeath47;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<KabucraItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<KabucraBanner>();
        }
        public NPC npcTarget;
        public Vector2 moveTo;
        public int hopCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 180);
        }
        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();
            NPC.defense = 5;
            NPC.knockBackResist = 0.5f;
            NPC.catchItem = (short)ModContent.ItemType<KabucraItem>();
            if (hopCooldown > 0)
                hopCooldown--;

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    HopCheck();

                    if (HideCheck() && Main.rand.NextBool(60))
                    {
                        NPC.velocity.X = 0;
                        AITimer = 0;
                        AIState = ActionState.Hide;
                    }

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }
                    break;

                case ActionState.Wander:
                    HopCheck();

                    if (HideCheck() && Main.rand.NextBool(100))
                    {
                        NPC.velocity.X = 0;
                        AITimer = 0;
                        AIState = ActionState.Hide;
                    }

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0)
                    {
                        NPCHelper.HorizontallyMove(NPC, new Vector2(npcTarget.Center.X < NPC.Center.X ? NPC.Center.X + 50 : NPC.Center.X - 50, NPC.Center.Y), 0.5f, 2, 4, 2, false);
                        return;
                    }

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }

                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 4, 2, false);
                    break;

                case ActionState.Hop:
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        moveTo = NPC.FindGround(10);
                        hopCooldown = 180;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }
                    break;

                case ActionState.Hide:
                    NPC.catchItem = ItemID.Seashell;
                    NPC.velocity.X *= 0.9f;
                    if (!HideCheck() && Main.rand.NextBool(200))
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                    }
                    break;
            }
            if (NPC.frame.Y >= 4 * 22)
            {
                NPC.defense = 90;
                NPC.knockBackResist = 0.1f;
            }
        }
        public bool HideCheck()
        {
            bool spooked = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy() || target.whoAmI == NPC.whoAmI || target.type == ModContent.NPCType<DevilsTongue>())
                    continue;

                if (NPC.Sight(target, 200, false, true) && BaseAI.HitTileOnSide(NPC, 3))
                {
                    spooked = true;
                }
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (NPC.Sight(target, 200, false, true) && BaseAI.HitTileOnSide(NPC, 3) && !target.RedemptionPlayerBuff().devilScented)
                    spooked = true;
            }
            return spooked;
        }
        public void HopCheck()
        {
            if (hopCooldown == 0 && BaseAI.HitTileOnSide(NPC, 3) &&
                RedeHelper.ClosestNPC(ref npcTarget, 50, NPC.Center) && npcTarget.damage > 0)
            {
                NPC.velocity.X *= npcTarget.Center.X < NPC.Center.X ? 1.4f : -1.4f;
                NPC.velocity.Y = Main.rand.NextFloat(-1f, -4f);
                AIState = ActionState.Hop;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += NPC.velocity.X * 0.5f;
                if (NPC.frameCounter is >= 3 or <= -3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 2 * frameHeight)
                        NPC.frame.Y = 0;
                }
                return;
            }
            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.frame.Y > 2 * frameHeight)
                    {
                        if (NPC.frameCounter++ >= 7)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y -= frameHeight;
                        }
                    }
                    else
                        NPC.frame.Y = 0;
                    break;
                case ActionState.Wander:
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 2 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                    break;
                case ActionState.Hop:
                    NPC.frame.Y = frameHeight;
                    break;
            }
            if (AIState is ActionState.Hide)
            {
                if (NPC.frame.Y < 3 * frameHeight)
                    NPC.frame.Y = 3 * frameHeight;

                if (NPC.frameCounter++ >= 7)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KabucraShell>(), 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.Seashell, 1, 1, 2));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Kabucra"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState is ActionState.Idle)
            {
                moveTo = NPC.FindGround(10);
                AITimer = 0;
                TimerRand = Main.rand.Next(120, 260);
                AIState = ActionState.Wander;
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/KabucraGore").Type);

                for (int i = 0; i < 2; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 6; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.BeachShell,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.BeachShell, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}

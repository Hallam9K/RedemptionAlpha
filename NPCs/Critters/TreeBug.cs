using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Redemption.NPCs.Critters
{
    public class TreeBug : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Hop,
            Eat
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

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 12;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.catchItem = (short)ModContent.ItemType<TreeBugItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TreeBugBanner>();
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

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0 && !npcTarget.Redemption().invisible)
                    {
                        moveTo = NPC.FindGround(10);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    Point tileBelow = NPC.Bottom.ToTileCoordinates();
                    Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

                    if (Main.rand.NextBool(500) && tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType] && TileLists.WoodLeaf.Contains(tile.TileType))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(180, 300);
                        AIState = ActionState.Eat;
                    }
                    break;

                case ActionState.Wander:
                    HopCheck();

                    if (RedeHelper.ClosestNPC(ref npcTarget, 100, NPC.Center) && npcTarget.damage > 0 && !npcTarget.Redemption().invisible)
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

                case ActionState.Eat:
                    NPC.velocity.X *= 0.5f;
                    tileBelow = NPC.Bottom.ToTileCoordinates();
                    tile = Main.tile[tileBelow.X, tileBelow.Y];
                    AITimer++;

                    if (AITimer % 30 == 0 && NPC.life < NPC.lifeMax)
                    {
                        NPC.life++;
                        NPC.HealEffect(1);
                    }

                    if (AITimer >= TimerRand || tile is not { HasUnactuatedTile: true } || !Main.tileSolid[tile.TileType] || !TileLists.WoodLeaf.Contains(tile.TileType))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                    }
                    break;
            }
        }

        public void HopCheck()
        {
            if (hopCooldown == 0 && BaseAI.HitTileOnSide(NPC, 3) &&
                RedeHelper.ClosestNPC(ref npcTarget, 32, NPC.Center) && npcTarget.damage > 0)
            {
                NPC.velocity.X *= npcTarget.Center.X < NPC.Center.X ? 1.4f : -1.4f;
                NPC.velocity.Y = Main.rand.NextFloat(-2f, -5f);
                AIState = ActionState.Hop;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Eat)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter < 20)
                    NPC.frame.Y = 4 * frameHeight;
                else if (NPC.frameCounter < 40)
                    NPC.frame.Y = 5 * frameHeight;
                else
                    NPC.frameCounter = 0;
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreeBugShell>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldDayGrassCritter.Chance;
            float multiplier = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType == TileID.Grass ? 1.7f : 0f;

            return baseChance * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.TreeBug"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState is ActionState.Idle or ActionState.Eat)
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

                int goreType1 = ModContent.Find<ModGore>("Redemption/TreeBugGore1").Type;
                int goreType2 = ModContent.Find<ModGore>("Redemption/TreeBugGore2").Type;

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, goreType1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, goreType2);

                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}
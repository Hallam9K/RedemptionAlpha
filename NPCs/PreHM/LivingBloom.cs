using Microsoft.Xna.Framework;
using Redemption.Base;
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
using Terraria.ModLoader.Utilities;
using Redemption.BaseExtension;
using Redemption.Items.Accessories.PreHM;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Utilities;
using Redemption.Tiles.Tiles;
using Terraria.GameContent;
using Terraria.Localization;
using System;
using Terraria.GameContent.UI;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.NPCs.PreHM
{
    public class LivingBloom : ModNPC
    {
        public static Asset<Texture2D> flowerTex;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            flowerTex = ModContent.Request<Texture2D>(Texture + "_Flower");
        }
        public override void Unload()
        {
            flowerTex = null;
        }
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
        public byte FlowerType;
        public byte BodyType;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 52;
            NPC.defense = 3;
            NPC.damage = 13;
            NPC.lifeMax = 45;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 20;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<LivingBloomBanner>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
            writer.Write(FlowerType);
            writer.Write(BodyType);
            writer.Write(pettingPlayer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
            FlowerType = reader.ReadByte();
            BodyType = reader.ReadByte();
            pettingPlayer = reader.ReadByte();
        }
        public NPC npcTarget;
        public Vector2 moveTo;
        public int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            ChooseType();

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

            if (AITimer % 30 == 0)
            {
                Point grass = new Vector2(NPC.Center.X, NPC.Bottom.Y - 4).ToTileCoordinates();
                GrassCheck(grass.X, grass.Y);
            }

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (!IsBeingPet && AITimer >= TimerRand)
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
                        int dustIndex = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.DryadsWard, 0f, 0f, 100, default, 1);
                        Main.dust[dustIndex].velocity.Y -= 4f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer == 5)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30)) / 16, (int)(globalNPC.attacker.Center.Y / 16) - 2);
                        NPC.Shoot(new Vector2(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30), (tilePosY * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || target.whoAmI == NPC.whoAmI || target.whoAmI == globalNPC.attacker.whoAmI || target.Redemption().invisible)
                                continue;

                            if (target.lifeMax < 5 || target.damage == 0 || NPC.DistanceSQ(target.Center) > 400 * 400 || target.type == NPC.type || NPCLists.Plantlike.Contains(target.type))
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Center.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
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
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
                        }
                    }
                    else if (AITimer >= 80)
                    {
                        AIState = ActionState.Threatened;
                    }
                    break;
            }
            PettingBehaviour();
        }
        private int pettingPlayer = -1;
        private bool IsBeingPet => pettingPlayer != -1;
        private int pettingTimer;
        private bool petFinish;
        private void PettingBehaviour()
        {
            if (petFinish && pettingTimer > 0 && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.velocity.Y -= 4;
                pettingTimer = 0;
            }
            if (AIState is ActionState.Idle)
            {
                if (NPC.getRect().Contains(Main.MouseWorld.ToPoint()))
                {
                    Player player = Main.LocalPlayer;
                    int pettingRange = 70;

                    if (player.Distance(NPC.Center) < pettingRange)
                    {
                        if (Main.mouseRight && Main.mouseRightRelease)
                        {
                            if (!IsBeingPet)
                            {
                                pettingTimer = 0;
                                player.Center = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height) + new Vector2(Math.Sign(NPC.DirectionTo(player.Center).X) * 20, -(player.height / 2));
                                player.direction = Math.Sign(player.DirectionTo(NPC.Center).X);
                                player.velocity = Vector2.Zero;

                                if (Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                            }

                            if (pettingPlayer == -1)
                                pettingPlayer = Main.myPlayer;
                            else
                                pettingPlayer = -1;

                            NPC.netUpdate = true;
                        }
                    }
                }
            }
            if (IsBeingPet)
            {
                Player pPlayer = Main.player[pettingPlayer];

                float amount = (float)Math.Sin(Main.timeForVisualEffects / 2f) * 2;
                Player.CompositeArmStretchAmount stretchAmount = (amount > 0) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.ThreeQuarters;
                pPlayer.SetCompositeArmBack(true, stretchAmount, -2.3f * pPlayer.direction);

                if (pPlayer.velocity != Vector2.Zero || NPC.velocity != Vector2.Zero)
                    pettingPlayer = -1;

                if (petFinish)
                    return;
                if (pettingTimer++ == 60)
                    EmoteBubble.NewBubble(136, new WorldUIAnchor(NPC), 120);

                if (pettingTimer >= TimerRand * 2)
                {
                    var item = Main.rand.Next(5) switch
                    {
                        1 => ItemID.Blinkroot,
                        2 => ItemID.Moonglow,
                        3 => ItemID.Waterleaf,
                        4 => ModContent.ItemType<Nightshade>(),
                        _ => ItemID.Daybloom,
                    };
                    EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 120);
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), item);
                    NPC.velocity.Y -= 5;
                    pettingTimer = 10;
                    petFinish = true;
                    pettingPlayer = -1;
                }
            }
        }
        public void ChooseType()
        {
            int ancient = BaseTile.GetTileCount(NPC.Center / 16, new int[] { ModContent.TileType<AncientGrassTile>() }, 4);
            int hallowed = BaseTile.GetTileCount(NPC.Center / 16, new int[] { TileID.HallowedGrass }, 4);
            if (ancient > 0)
            {
                FlowerType = 5;
                BodyType = 3;
                NPC.netUpdate = true;
                return;
            }
            WeightedRandom<byte> flower = new(Main.rand);
            flower.Add(0);
            flower.Add(1, 0.4);
            flower.Add(2, 0.4);
            flower.Add(3, 0.2);
            flower.Add(4, 0.2);
            flower.Add(5, 0.01);
            flower.Add(6, 0.1);
            flower.Add(7, 0.1);
            flower.Add(8, 0.05);
            FlowerType = flower;

            WeightedRandom<byte> body = new(Main.rand);
            body.Add(0);
            body.Add(1, 0.2);
            body.Add(2, hallowed > 0 ? 2 : 0.1);
            if (FlowerType is 5)
                body.Add(2, 10);
            BodyType = body;
            NPC.netUpdate = true;
        }
        public static bool GrassCheck(int X, int Y) // Directly from Flower Boots code, cleaned a bit
        {
            Tile tile = Framing.GetTileSafely(X, Y);
            if (tile == null)
            {
                return false;
            }
            if (!tile.HasTile && tile.LiquidAmount == 0 && Main.tile[X, Y + 1] != null && WorldGen.SolidTile(X, Y + 1))
            {
                tile.TileFrameY = 0;
                tile.Slope = 0;
                tile.IsHalfBlock = false;
                if (Main.tile[X, Y + 1].TileType == TileID.Grass || Main.tile[X, Y + 1].TileType == TileID.GolfGrass)
                {
                    int num = Main.rand.NextFromList(6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24, 27, 30, 33, 36, 39, 42);
                    switch (num)
                    {
                        case 21:
                        case 24:
                        case 27:
                        case 30:
                        case 33:
                        case 36:
                        case 39:
                        case 42:
                            num += Main.rand.Next(3);
                            break;
                    }
                    tile.HasTile = true;
                    tile.TileType = TileID.Plants;
                    tile.TileFrameX = (short)(num * 18);
                    tile.TileColor = Main.tile[X, Y + 1].TileColor;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
                if (Main.tile[X, Y + 1].TileType == TileID.HallowedGrass || Main.tile[X, Y + 1].TileType == TileID.GolfGrassHallowed)
                {
                    if (Main.rand.NextBool(2))
                    {
                        tile.HasTile = true;
                        tile.TileType = TileID.HallowedPlants;
                        tile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                        tile.TileColor = Main.tile[X, Y + 1].TileColor;
                        while (tile.TileFrameX == 90)
                        {
                            tile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                        }
                    }
                    else
                    {
                        tile.HasTile = true;
                        tile.TileType = TileID.HallowedPlants2;
                        tile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                        tile.TileColor = Main.tile[X, Y + 1].TileColor;
                        while (tile.TileFrameX == 90)
                        {
                            tile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                        }
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
                if (Main.tile[X, Y + 1].TileType == 60)
                {
                    tile.HasTile = true;
                    tile.TileType = 74;
                    tile.TileFrameX = (short)(18 * Main.rand.Next(9, 17));
                    tile.TileColor = Main.tile[X, Y + 1].TileColor;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, X, Y);
                    }
                    return true;
                }
            }
            return false;
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
                    NPC.life += 1;
                    NPC.HealEffect(1);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;
            NPC.frame.X = NPC.frame.Width * BodyType;
            if (IsBeingPet)
            {
                NPC.frame.Y = 7 * frameHeight;
                return;
            }
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int Height = flowerTex.Value.Height / 11;
            int Width = flowerTex.Value.Width / 9;
            int y = Height * (NPC.frame.Y / 60);
            int x = Width * FlowerType;
            Rectangle rect = new(x, y, Width, Height);
            spriteBatch.Draw(flowerTex.Value, NPC.Center - screenPos, new Rectangle?(rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.OneFromOptions(2,
                new int[] { ItemID.Daybloom, ItemID.Blinkroot, ItemID.Moonglow, ItemID.Waterleaf, ModContent.ItemType<Nightshade>() }));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantMatter>(), 4, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnglonicMysticBlossom>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ForestCore>(), 60));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType == ModContent.TileType<AncientGrassTile>())
            {
                float day = SpawnCondition.OverworldDay.Chance * (Main.raining ? 0.28f : 0.18f);
                float cavern = SpawnCondition.Cavern.Chance * 0.18f;
                return MathHelper.Max(day, cavern);
            }
            float baseChance = SpawnCondition.OverworldDay.Chance;
            float multiplier = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType is TileID.Grass or TileID.HallowedGrass ? (Main.raining ? 0.28f : 0.18f) : 0f;

            return baseChance * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.LivingBloom"))
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

                int goreType1 = ModContent.Find<ModGore>("Redemption/LivingBloomGore" + (FlowerType + 1)).Type;
                int goreType2 = ModContent.Find<ModGore>("Redemption/LivingBloomGore10").Type;

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, goreType1);
                for (int p = 0; p < 2; p++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, goreType2);

                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}
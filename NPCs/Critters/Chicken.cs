using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Critters;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Redemption.NPCs.Critters
{
    public class Chicken : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Peck,
            Sit
        }

        public enum ChickenType
        {
            Normal,
            Red,
            Leghorn,
            Black
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ChickenType ChickType
        {
            get => (ChickenType)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = ModContent.NPCType<LongChicken>();
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
            NPC.width = 26;
            NPC.height = 22;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.catchItem = (short)ModContent.ItemType<ChickenItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ChickenBanner>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChickenEgg>(), 2, 1, 2));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>()));
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (item.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (projectile.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>());
            }
        }
        public Vector2 moveTo;
        private int hopCooldown;
        private int runCooldown;
        private int waterCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            if (AITimer == 0)
            {
                if (Main.rand.NextBool(2000))
                    NPC.SetDefaults(ModContent.NPCType<LongChicken>());
                else
                    ChickType = (ChickenType)Main.rand.Next(4);
            }

            TimerRand = Main.rand.Next(80, 180);
        }
        public override void AI()
        {
            NPC.TargetClosest();
            NPC.LookByVelocity();
            RedeNPC globalNPC = NPC.Redemption();

            if (hopCooldown > 0)
                hopCooldown--;

            if (NPC.wet && !NPC.lavaWet && waterCooldown < 180)
            {
                NPC.velocity.Y -= 0.3f;
                waterCooldown++;
            }

            if (Main.rand.NextBool(500) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.ChickenCluck, NPC.position);

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
                    }

                    if (Main.rand.NextBool(200) && (NPC.collideY || NPC.velocity.Y == 0))
                        AIState = ActionState.Peck;

                    Point tileBelow = NPC.Bottom.ToTileCoordinates();
                    Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

                    if ((NPC.collideY || NPC.velocity.Y == 0) && Main.rand.NextBool(100) && tile.TileType == TileID.HayBlock &&
                        tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(300, 1200);
                        AIState = ActionState.Sit;
                    }

                    SightCheck();
                    break;

                case ActionState.Peck:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    if (NPC.frame.Y > 20 * 28)
                    {
                        NPC.frame.Y = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Sit:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    AITimer++;

                    if (AITimer == TimerRand - 60)
                    {
                        SoundEngine.PlaySound(SoundID.Item16, NPC.position);
                        Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<ChickenEgg>());
                    }
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
                        AIState = ActionState.Wander;
                    }

                    Point tileBelow2 = new Vector2(NPC.Center.X, NPC.Bottom.Y).ToTileCoordinates();
                    Tile tile2 = Main.tile[tileBelow2.X, tileBelow2.Y];
                    if (tile2.TileType != TileID.HayBlock || tile2 is not { HasUnactuatedTile: true } || !Main.tileSolid[tile2.TileType])
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 180);
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

                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.2f, 1, 6, 6, false);
                    break;

                case ActionState.Alert:
                    if (Main.rand.NextBool(50))
                        SightCheck();

                    if (NPC.ThreatenedCheck(ref runCooldown, 180))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 200, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    int FeatherType;
                    FeatherType = ChickType switch
                    {
                        ChickenType.Leghorn => ModContent.DustType<ChickenFeatherDust3>(),
                        ChickenType.Red => ModContent.DustType<ChickenFeatherDust2>(),
                        ChickenType.Black => ModContent.DustType<ChickenFeatherDust4>(),
                        _ => ModContent.DustType<ChickenFeatherDust1>(),
                    };

                    if (Main.rand.NextBool(20) && NPC.velocity.Length() >= 2)
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, FeatherType);

                    NPCHelper.HorizontallyMove(NPC, new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y), 0.2f, 2.5f, 8, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;
            }
            NPC.alpha = 0;
            switch (ChickType)
            {
                case ChickenType.Red:
                    NPC.catchItem = (short)ModContent.ItemType<RedChickenItem>();
                    break;
                case ChickenType.Leghorn:
                    NPC.catchItem = (short)ModContent.ItemType<LeghornChickenItem>();
                    break;
                case ChickenType.Black:
                    NPC.catchItem = (short)ModContent.ItemType<BlackChickenItem>();
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;
            NPC.frame.X = ChickType switch
            {
                ChickenType.Red => NPC.frame.Width,
                ChickenType.Leghorn => NPC.frame.Width * 2,
                ChickenType.Black => NPC.frame.Width * 3,
                _ => 0,
            };

            if (AIState is ActionState.Peck)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 14 * frameHeight)
                    NPC.frame.Y = 14 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 20 * frameHeight)
                        NPC.frame.Y = 20 * frameHeight;
                }
                return;
            }
            if (AIState is ActionState.Sit)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 9 * frameHeight)
                    NPC.frame.Y = 9 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 13 * frameHeight)
                        NPC.frame.Y = 13 * frameHeight;
                }
                return;
            }

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y < 1 * frameHeight)
                        NPC.frame.Y = 1 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.75f;
                    if (NPC.frameCounter is >= 5 or <= -5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = 1 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public void SightCheck()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = RedeHelper.GetNearestNPC(NPC.Center);
            if (NPC.Sight(player, 140, true, true) && !player.RedemptionPlayerBuff().ChickenForm && !player.RedemptionPlayerBuff().devilScented)
            {
                globalNPC.attacker = player;
                AITimer = 0;
                if (AIState != ActionState.Alert)
                    AIState = ActionState.Alert;
            }
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 140, true, true))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                if (AIState != ActionState.Alert)
                    AIState = ActionState.Alert;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Chicken"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AIState is not ActionState.Alert)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }

            int FeatherType;
            FeatherType = ChickType switch
            {
                ChickenType.Leghorn => ModContent.DustType<ChickenFeatherDust3>(),
                ChickenType.Red => ModContent.DustType<ChickenFeatherDust2>(),
                ChickenType.Black => ModContent.DustType<ChickenFeatherDust4>(),
                _ => ModContent.DustType<ChickenFeatherDust1>(),
            };

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, FeatherType,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, FeatherType, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}
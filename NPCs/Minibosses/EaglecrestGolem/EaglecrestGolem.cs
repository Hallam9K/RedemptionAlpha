using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    [AutoloadBossHead]
    public class EaglecrestGolem : ModNPC
    {
        public enum ActionState
        {
            Start,
            Idle,
            Slash,
            Roll,
            Laser
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ref float TimerRand2 => ref NPC.ai[3];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Golem");
            Main.npcFrameCount[NPC.type] = 13;

            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1,
                Position = new Vector2(0, 30),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCEarth[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 3200;
            NPC.damage = 30;
            NPC.defense = 18;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.aiStyle = -1;
            NPC.width = 80;
            NPC.height = 80;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Earth] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Thunder] *= .9f;

        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState is ActionState.Roll;
        public override bool CanHitNPC(NPC target) => target.friendly && AIState is ActionState.Roll;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Golem"))
            });
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedEaglecrestGolem)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.EaglecrestDefeat"), 180, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedEaglecrestGolem, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<StonePuppet>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GolemEye>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestHead>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestJavelin>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestSling>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestGlove>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GathicStone>(), 1, 14, 34));
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(AniFrameY);
                writer.Write(summonTimer);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                AniFrameY = reader.ReadInt32();
                summonTimer = reader.ReadInt32();
            }
        }
        private int AniFrameY;
        private int summonTimer;
        private float FlareTimer;
        private bool Flare;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            DespawnHandler();

            if (AIState != ActionState.Slash && AIState != ActionState.Laser)
                NPC.LookAtEntity(player);

            float moveInterval = NPC.life < NPC.lifeMax / 2 ? 0.06f : 0.04f;
            float moveSpeed = NPC.life < NPC.lifeMax / 2 ? 4f : 2f;
            if (NPC.life < NPC.lifeMax / 10)
            {
                moveInterval = 0.07f;
                moveSpeed = 6f;
            }

            switch (AIState)
            {
                case ActionState.Start:
                    NPC.target = RedeHelper.GetNearestAlivePlayer(NPC);
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Golem.Name"), 60, 90, 0.8f, 0, Color.Gray, Language.GetTextValue("Mods.Redemption.TitleCard.Golem.Modifier"));

                    TimerRand = Main.rand.Next(300, 700);
                    AIState = ActionState.Idle;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    if (++AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(500, 700);
                        AIState = ActionState.Roll;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(player.Center) <= 400 * 400 && Main.rand.NextBool(150))
                    {
                        NPC.velocity.X = 0;
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Slash;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(player.Center) > 150 * 150 && Main.rand.NextBool(400))
                    {
                        TimerRand2 = 0;
                        AIState = ActionState.Laser;
                        NPC.netUpdate = true;
                    }

                    summonTimer--;
                    if (Main.rand.NextBool(100) && summonTimer <= 0 && NPC.CountNPCS(ModContent.NPCType<EaglecrestRockPile>()) < 1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockPileSummon>(), 0, RedeHelper.SpreadUp(16), false, SoundID.Item1, NPC.whoAmI);
                        }
                        summonTimer = 600;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                    NPCHelper.HorizontallyMove(NPC, player.Center, moveInterval, moveSpeed, 12, 12, NPC.Center.Y > player.Center.Y, player);
                    break;
                case ActionState.Roll:
                    if (!Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) || Collision.SolidCollision(new Vector2(NPC.Center.X, NPC.position.Y - NPC.height / 2 + 10), NPC.width, NPC.height))
                    {
                        TimerRand2++;
                    }
                    else
                        TimerRand2 = 0;

                    if (TimerRand2 >= 80)
                    {
                        AITimer = 800;
                        NPC.Move(player.Center, 9, 40);
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;

                        AITimer++;
                        if (AITimer >= TimerRand)
                        {
                            NPC.velocity.Y -= 6;
                            AITimer = 0;
                            TimerRand = Main.rand.Next(300, 700);
                            AIState = ActionState.Idle;
                            NPC.netUpdate = true;
                        }

                        NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                        NPCHelper.HorizontallyMove(NPC, player.Center, 0.12f, 10, 20, 30, NPC.Center.Y > player.Center.Y, player);
                    }
                    break;
                case ActionState.Laser:
                    NPC.velocity.X = 0;
                    Vector2 origin = NPC.Center - new Vector2(-2 * NPC.spriteDirection, 18);
                    if (++TimerRand2 < 60)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 30);
                            vector.Y = (float)(Math.Cos(angle) * 30);
                            Dust dust2 = Main.dust[Dust.NewDust(origin + vector, 2, 2, DustID.Sandnado)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(origin) * 5f;
                        }
                    }
                    if (TimerRand2 == 60)
                    {
                        NPC.Shoot(origin, ModContent.ProjectileType<GolemEyeRay>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation()
                            + MathHelper.ToRadians(20 * NPC.spriteDirection)), true, SoundID.Item109, NPC.whoAmI);
                    }
                    if (TimerRand2 >= 60)
                    {
                        FlareTimer = 0;
                        Flare = true;
                    }

                    if (TimerRand2 > 120)
                    {
                        TimerRand2 = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (Flare)
            {
                FlareTimer++;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }

            if (AIState is ActionState.Slash)
            {
                NPC.velocity.X = 0;
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AniFrameY is 6)
                    {
                        Player player = Main.player[NPC.target];
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockSlash_Proj>(), NPC.damage, RedeHelper.PolarVector(7,
                            (player.Center - NPC.Center).ToRotation()), true, SoundID.Item71);
                    }
                    if (AniFrameY > 8)
                    {
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                }
                return;
            }
            AniFrameY = 0;

            if (AIState is ActionState.Roll)
            {
                NPC.width = 54;
                NPC.height = 54;
                NPC.rotation += NPC.velocity.X * 0.05f;
                NPC.frame.Y = 12 * frameHeight;
                return;
            }
            else
            {
                NPC.width = 80;
                NPC.height = 80;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 5 * frameHeight;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 11 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 6 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D SlashAni = ModContent.Request<Texture2D>(Texture + "_Slash").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy && AIState is ActionState.Roll)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }

            if (AIState is ActionState.Slash)
            {
                int Height = SlashAni.Height / 9;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, SlashAni.Width, Height);
                Vector2 origin = new(SlashAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(SlashAni, NPC.Center - screenPos - new Vector2(0, 13), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos - new Vector2(-2 * NPC.spriteDirection, 18);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.Orange, NPC.rotation);
            }
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 35; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 3f);
                    Main.dust[dustIndex2].velocity *= 5f;
                }
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore2").Type, 1);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore4").Type, 1);
                for (int i = 0; i < 12; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore5").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore1").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestGolemGore3").Type, 1);
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
            Main.dust[dustIndex].velocity *= 2f;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Chat;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.Thorn
{
    [AutoloadBossHead]
    public class Thorn : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            TeleportStart,
            TeleportEnd,
            BarrierSpawn,
            Death
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
            // DisplayName.SetDefault("Thorn, Bane of the Forest");
            Main.npcFrameCount[NPC.type] = 10;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.BloodButcherer] = false;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 2100;
            NPC.defense = 6;
            NPC.damage = 21;
            NPC.width = 58;
            NPC.height = 88;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Thorn"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThornBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThornTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThornRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ThornMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<RootTendril>(), ModContent.ItemType<CursedThornBow>()));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedThorn)
            {
                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.ThornDowned");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(50, 255, 130));
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), new Color(50, 255, 130));

                RedeWorld.alignment += 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "+2", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns2"), 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedThorn, -1);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(barrierSpawn);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            barrierSpawn = reader.ReadBoolean();
        }

        public List<int> AttackList = new() { 0, 1, 2, 3, 4 };
        public List<int> CopyList = null;
        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }
        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }
        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.xMas)
                typeName = Language.GetTextValue("Mods.Redemption.NPCs.Thorn.XmasName");
        }
        private bool barrierSpawn;
        private Vector2 origin;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (DespawnHandler())
                return;

            if (AIState != ActionState.TeleportStart && AIState != ActionState.TeleportEnd && AIState != ActionState.Death)
                NPC.LookAtEntity(player);

            Vector2 HeartOrigin = new(NPC.Center.X, NPC.Center.Y - 18);

            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player pl = Main.player[p];
                if (!pl.active || pl.dead || !NPC.Hitbox.Intersects(pl.Hitbox))
                    continue;

                if (AIState == ActionState.TeleportStart || AIState == ActionState.TeleportEnd || AIState == ActionState.Death)
                    continue;

                pl.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 10);
            }

            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Thorn.Name"), 60, 90, 0.8f, 0, Color.LawnGreen, Language.GetTextValue("Mods.Redemption.TitleCard.Thorn.Modifier"));
                    AIState = ActionState.TeleportStart;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    if (++AITimer > 60)
                    {
                        AITimer = 0;
                        if (!barrierSpawn && (Main.expertMode ? NPC.life < NPC.lifeMax / 2 : NPC.life < (int)(NPC.lifeMax * 0.35f)))
                        {
                            TimerRand = 0;
                            AIState = ActionState.BarrierSpawn;
                            NPC.netUpdate = true;
                            break;
                        }
                        AttackChoice();
                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Thorns
                        case 0:
                            if (AITimer++ == 0)
                                TimerRand = 500;
                            if (AITimer >= 60 && AITimer % (NPC.life < NPC.lifeMax / 2 ? 15 : 25) == 0 && TimerRand >= 0)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + TimerRand, player.Center.Y - 200), ModContent.ProjectileType<ThornSeed>(),
                                    NPC.damage, Vector2.Zero);
                                if (TimerRand != 0)
                                    NPC.Shoot(new Vector2(player.Center.X + -TimerRand, player.Center.Y - 200), ModContent.ProjectileType<ThornSeed>(),
                                    NPC.damage, Vector2.Zero);

                                TimerRand -= 100;
                                NPC.netUpdate = true;
                            }
                            if (AITimer >= (NPC.life < NPC.lifeMax / 2 ? 200 : 280))
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Vine Whip
                        case 1:
                            if (NPC.life < NPC.lifeMax / 2 ? NPC.DistanceSQ(player.Center) < 400 * 400 : NPC.DistanceSQ(player.Center) < 300 * 300)
                            {
                                AITimer++;

                                if (AITimer == 60)
                                    origin = player.Center;

                                if (AITimer >= 60 && AITimer % 15 == 0 && AITimer <= 180)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<CursedThornVile>(), NPC.damage,
                                        RedeHelper.PolarVector(NPC.life < NPC.lifeMax / 2 ? 18 : 12, (origin - NPC.Center).ToRotation()
                                        + TimerRand - MathHelper.ToRadians(45)), SoundID.Item17);

                                    TimerRand += MathHelper.ToRadians(15);
                                    NPC.netUpdate = true;
                                }

                                if (AITimer >= 200)
                                {
                                    TimerRand = 0;
                                    AITimer = 0;
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                TimerRand = 0;
                                AITimer = 60;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Poison
                        case 2:
                            if (++AITimer == 10)
                            {
                                for (int k = 0; k < 40; k++)
                                {
                                    Vector2 vector;
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.GrassBlades, 0f, 0f, 100, default, 3f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;
                                }
                            }

                            if (NPC.life < NPC.lifeMax / 2 ? (AITimer == 80 || AITimer == 140) : (AITimer == 80 || AITimer == 160))
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ProjectileID.QueenBeeStinger, NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(90) * i), SoundID.Item17);
                            }
                            if (NPC.life < NPC.lifeMax / 2 ? AITimer == 110 : AITimer == 120)
                            {
                                for (int i = 0; i < 8; i++)
                                    NPC.Shoot(NPC.Center, ProjectileID.QueenBeeStinger, NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(45) * i), SoundID.Item17);
                            }
                            if (NPC.life < NPC.lifeMax / 2 && AITimer == 170)
                            {
                                for (int i = 0; i < 18; i++)
                                    NPC.Shoot(NPC.Center, ProjectileID.QueenBeeStinger, NPC.damage, RedeHelper.PolarVector(10, MathHelper.ToRadians(20) * i), SoundID.Item17);
                            }

                            if (AITimer >= 180)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Life Drain
                        case 3:
                            if (NPC.life < (int)(NPC.lifeMax * 0.9f))
                            {
                                if (++AITimer < 60)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Vector2 vector;
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 100);
                                        vector.Y = (float)(Math.Cos(angle) * 100);
                                        Dust dust2 = Main.dust[Dust.NewDust(HeartOrigin + vector, 2, 2, DustID.LifeDrain, 0f, 0f, 100, default, 1f)];
                                        dust2.noGravity = true;
                                        dust2.velocity = dust2.position.DirectionTo(HeartOrigin) * 10f;
                                    }
                                }

                                if (AITimer >= 60 && (NPC.life < NPC.lifeMax / 2 ? AITimer <= 120 : AITimer <= 90))
                                {
                                    if (Main.rand.NextBool(2))
                                        NPC.Shoot(HeartOrigin, ModContent.ProjectileType<LeechingThornSeed>(), NPC.damage,
                                            RedeHelper.PolarVector(11, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), SoundID.Item17, NPC.whoAmI);
                                }

                                if (NPC.life < NPC.lifeMax / 2 ? AITimer >= 240 : AITimer >= 180)
                                {
                                    TimerRand = 0;
                                    AITimer = 0;
                                    AIState = Main.rand.NextBool(3) ? ActionState.Idle : ActionState.TeleportStart;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                TimerRand = 0;
                                AITimer = 60;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Cleave
                        case 4:
                            AITimer++;
                            if (NPC.life < NPC.lifeMax / 2 ? (AITimer >= 60 && AITimer % 30 == 0 && AITimer <= 180) : (AITimer >= 50 && AITimer % 50 == 0 && AITimer <= 200))
                            {
                                for (int i = 0; i < 8; ++i)
                                {
                                    Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sandnado, 0.0f, 0.0f, 100, new Color(), 2f);
                                    dust.velocity = -player.DirectionTo(dust.position) * 10;
                                    dust.noGravity = true;
                                }
                                int steps = (int)NPC.Distance(player.Center) / 8;
                                for (int i = 0; i < steps; i++)
                                {
                                    if (Main.rand.NextBool(2))
                                    {
                                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(NPC.Center, player.Center, (float)i / steps), 2, 2, DustID.Sandnado, Scale: 2);
                                        dust.velocity = -player.DirectionTo(dust.position) * 2;
                                        dust.noGravity = true;
                                    }
                                }
                                for (int k = 0; k < 16; k++)
                                {
                                    Vector2 vector;
                                    double angle = k * (Math.PI * 2 / 16);
                                    vector.X = (float)(Math.Sin(angle) * 60);
                                    vector.Y = (float)(Math.Cos(angle) * 60);
                                    Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                }
                                NPC.Shoot(player.Center, ModContent.ProjectileType<SlashFlashPro>(), NPC.damage, Vector2.Zero);
                            }

                            if (NPC.life < NPC.lifeMax / 2 ? AITimer >= 230 : AITimer >= 250)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = Main.rand.NextBool(4) ? ActionState.Idle : ActionState.TeleportStart;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.BarrierSpawn:
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None, 1200, 2400, 1200);

                    if (AITimer >= 60)
                    {
                        if (AITimer < 180)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                Vector2 vector;
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vector.X = (float)(Math.Sin(angle) * 150);
                                vector.Y = (float)(Math.Cos(angle) * 150);
                                Dust dust2 = Main.dust[Dust.NewDust(HeartOrigin + vector, 2, 2, DustID.MagicMirror, Scale: 2)];
                                dust2.noGravity = true;
                                dust2.velocity = dust2.position.DirectionTo(HeartOrigin) * 15f;
                            }
                        }

                        if (AITimer % 30 == 0 && AITimer < 180)
                        {
                            SoundEngine.PlaySound(SoundID.Item28 with { Pitch = TimerRand }, NPC.position);
                            TimerRand += 0.1f;
                            NPC.netUpdate = true;
                        }

                        if (AITimer == 180)
                        {
                            DustHelper.DrawCircle(HeartOrigin, DustID.MagicMirror, 5, 5, 5, 1, 2, nogravity: true);
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ManaBarrier>(), 0, Vector2.Zero, SoundID.Item29, NPC.whoAmI);
                        }
                    }
                    if (AITimer >= 220)
                    {
                        barrierSpawn = true;
                        NPC.dontTakeDamage = false;
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Idle;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
                case ActionState.Death:
                    if (AITimer++ == 0)
                    {
                        NPC.dontTakeDamage = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    if (AITimer >= 24)
                        NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                    {
                        NPC.dontTakeDamage = false;
                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
            }
        }
        public override void PostAI()
        {
            CustomFrames(96);
        }
        private void CustomFrames(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.TeleportStart:
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 9 * frameHeight)
                            NPC.alpha += 25;
                        if (NPC.frame.Y > 9 * frameHeight)
                        {
                            NPC.frame.Y = 9 * frameHeight;
                            NPC.frameCounter = 0;
                            NPC.position = NPC.FindGroundPlayer(15) - new Vector2(0, NPC.height);
                            AIState = ActionState.TeleportEnd;
                            NPC.velocity.X = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    return;
                case ActionState.TeleportEnd:
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y -= frameHeight;
                        NPC.alpha -= 25;
                        if (NPC.frame.Y <= 4 * frameHeight)
                        {
                            NPC.alpha = 0;
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            AIState = ActionState.Idle;
                            NPC.netUpdate = true;
                        }
                    }
                    return;
                case ActionState.Death:
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 9 * frameHeight;
                    }
                    return;
            }
        }
        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {
                for (int i = 0; i < 40; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                NPC.life = 1;
                AITimer = 0;
                TimerRand = 0;
                AIState = ActionState.Death;
                return false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.TeleportStart or ActionState.TeleportEnd or ActionState.Death)
                return;
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 4 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(16 * NPC.spriteDirection, 0), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        private bool DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    AITimer = 0;
                    TimerRand = 0;
                    AIState = ActionState.Death;
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    NPC.dontTakeDamage = true;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    return true;
                }
            }
            return false;
        }
    }
}
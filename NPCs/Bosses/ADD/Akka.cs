using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Buffs.NPCBuffs;
using Terraria.GameContent;
using Redemption.Items.Usable;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.UI;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Armor.Vanity;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class Akka : ModNPC
    {
        private Player player;
        public enum ActionState
        {
            Start,
            ResetVars,
            Idle,
            Attacks
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AttackID => ref NPC.ai[1];
        public ref float AITimer => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Akka");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    BuffID.Confused,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public int GuardPointMax;
        public override void SetDefaults()
        {
            NPC.lifeMax = 108000;
            NPC.damage = 115;
            NPC.defense = 50;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 8, 0, 0);
            NPC.aiStyle = -1;
            NPC.width = 66;
            NPC.height = 124;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            GuardPointMax = NPC.lifeMax / 50;
            NPC.RedemptionGuard().GuardBroken = true;
            NPC.BossBar = ModContent.GetInstance<AkkaHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest2");
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ItemLists.Blood.Contains(item.type) || ItemLists.Earth.Contains(item.type) || ItemLists.Nature.Contains(item.type))
                    NPC.Redemption().elementDmg *= 0.75f;

                if (ItemLists.Poison.Contains(item.type) || ItemLists.Water.Contains(item.type))
                    NPC.Redemption().elementDmg *= 0.9f;

                if (ItemLists.Fire.Contains(item.type))
                    NPC.Redemption().elementDmg *= 1.25f;

                if (ItemLists.Wind.Contains(item.type))
                    NPC.Redemption().elementDmg *= 1.1f;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ProjectileLists.Blood.Contains(projectile.type) || ProjectileLists.Earth.Contains(projectile.type) || ProjectileLists.Nature.Contains(projectile.type))
                    NPC.Redemption().elementDmg *= 0.75f;

                if (ProjectileLists.Poison.Contains(projectile.type) || ProjectileLists.Water.Contains(projectile.type))
                    NPC.Redemption().elementDmg *= 0.9f;

                if (ProjectileLists.Fire.Contains(projectile.type))
                    NPC.Redemption().elementDmg *= 1.25f;

                if (ProjectileLists.Wind.Contains(projectile.type))
                    NPC.Redemption().elementDmg *= 1.1f;
            }

            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                damage = (int)(damage * .75f);
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool vDmg = false;
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                NPC.RedemptionGuard().GuardHit(NPC, ref vDmg, ref damage, ref knockback, SoundID.Dig with { Pitch = -.1f });
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                    return vDmg;
            }
            NPC.RedemptionGuard().GuardBreakCheck(NPC, DustID.t_LivingWood, SoundID.Item43, 10, 1, 2000);
            return true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,

                new FlavorTextBestiaryInfoElement("Little has been recorded of Akka during her youth, it is believed by the locals that her spirit infused with a great tree in the Spirit Realm, where she would sleep until awoken by her husband to be worshipped once more.")
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AkkaBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AkanKirvesTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<AkkaRelic>()));

            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AkkaMask>(), 7));

            //notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<RootTendril>(), ModContent.ItemType<CursedThornBow>(), ModContent.ItemType<BlightedBoline>()));

            npcLoot.Add(notExpertRule);
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedADD && !NPC.AnyNPCs(ModContent.NPCType<Ukko>()))
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gray, "+0", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("It is unknown how these forgotten deities ruled, perhaps defeating them was for the best, or worst.", 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedADD, -1);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);  //boss life scale in expertmode
            NPC.damage = (int)(NPC.damage * 0.6f);  //boss damage increase in expermode
        }


        private Vector2 MoveVector2;
        private int islandCooldown = 10;
        private int barkskinCooldown;
        private int healingCooldown;
        public int teamCooldown = 10;
        private int TremorTimer;
        public override void AI()
        {
            Target();

            DespawnHandler();

            Player player = Main.player[NPC.target];
            NPC.rotation = 0f;
            NPC.LookAtEntity(player);

            if (player.active && !player.dead)
                NPC.DiscourageDespawn(60);

            bool ukkoActive = false;
            if (NPC.ai[3] > -1 && Main.npc[(int)NPC.ai[3]].active && Main.npc[(int)NPC.ai[3]].type == ModContent.NPCType<Ukko>())
                ukkoActive = true;
            Vector2 EarthProtectPos = new(player.Center.X + (player.Center.X > NPC.Center.X ? -500 : 500), player.Center.Y - 100);
            switch (AIState)
            {
                case ActionState.Start:
                    if (ukkoActive)
                    {
                        NPC ukko = Main.npc[(int)NPC.ai[3]];
                        switch (AttackID)
                        {
                            case 0:
                                if (AITimer++ >= 80)
                                {
                                    NPC.spriteDirection = -1;
                                    if (NPC.DistanceSQ(ukko.Center + new Vector2(100, 0)) < 20 * 20)
                                    {
                                        AITimer = 0;
                                        AttackID = 1;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.MoveToVector2(ukko.Center + new Vector2(100, 0), 35);
                                }
                                break;
                            case 1:
                                NPC.spriteDirection = -1;
                                NPC.velocity *= 0.9f;
                                if (AITimer == 60)
                                {
                                    if (!Main.dedServ)
                                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Akka", 60, 90, 0.8f, 0, Color.PaleGreen, "Ancient Goddess of Nature");

                                    EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 50);
                                }

                                if (AITimer++ >= 120)
                                {
                                    AIState = ActionState.ResetVars; ;
                                    AITimer = 0;
                                    AttackID = 0;
                                    NPC.netUpdate = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (!Main.dedServ)
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Akka", 60, 90, 0.8f, 0, Color.LightGreen, "Ancient Goddess of Nature");

                        AIState = ActionState.ResetVars; ;
                        AITimer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.ResetVars:
                    if (islandCooldown > 0)
                        islandCooldown--;
                    if (barkskinCooldown > 0 && NPC.RedemptionGuard().GuardPoints <= 0)
                        barkskinCooldown--;
                    if (healingCooldown > 0)
                        healingCooldown--;
                    if (teamCooldown > 0)
                        teamCooldown--;
                    Frame = 0;
                    MoveVector2 = Pos();
                    NPC.ai[0]++;
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                    {
                        AITimer = 0;
                        NPC.velocity *= 0;
                        NPC.ai[0]++;
                        AttackID = Main.rand.Next(10);
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.MoveToVector2(MoveVector2, 35);
                    break;
                case ActionState.Attacks:
                    switch (AttackID)
                    {
                        #region Poison Spray
                        case 0:
                            AITimer++;
                            if (AITimer % 3 == 0 && AITimer < 60)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<AkkaPoisonBubble>(), (int)(NPC.damage * 0.95f) / 4, 1);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (AITimer >= 65)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Island Bombardment
                        case 1:
                            if (islandCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer == 30)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Quake with { Volume = 1.4f, PitchVariance = 0.1f });

                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 0f, ModContent.ProjectileType<AkkaIslandSummoner>(), (int)(NPC.damage * 1.5f) / 4, 1, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (AITimer >= 200)
                                {
                                    islandCooldown = 30;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(10);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Earth Tremor
                        case 2:
                            AITimer++;
                            Point point = player.position.ToTileCoordinates();
                            if (AITimer == 5)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Quake with { Volume = 1.2f, PitchVariance = 0.1f });
                            }
                            if (Main.tile[point.X, point.Y + 3].TileType != 0)
                            {
                                player.GetModPlayer<ScreenPlayer>().Rumble(2, 4);
                                TremorTimer++;
                                if (TremorTimer > 50 && TremorTimer % 40 == 0)
                                {
                                    int hitDirection = NPC.Center.X > player.Center.X ? 1 : -1;
                                    player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI),
                                        40, hitDirection, false, false, false, 0);
                                    player.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
                                }
                            }
                            if (AITimer >= 180)
                            {
                                TremorTimer = 0;
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Barkskin
                        case 3:
                            if (NPC.life < (int)(NPC.lifeMax * 0.8f) && barkskinCooldown == 0)
                            {
                                if (AITimer++ == 0)
                                {
                                    if (NPC.RedemptionGuard().GuardPoints > 0)
                                    {
                                        AttackID = Main.rand.Next(10);
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                if (AITimer < 60)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_LivingWood, 0f, 0f, 100, default, 2f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);

                                    NPC.AddBuff(ModContent.BuffType<StoneskinBuff>(), 600);
                                }
                                if (AITimer == 60)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
                                    DustHelper.DrawCircle(NPC.Center, DustID.DryadsWard, 10, 1, 1, 1, 3, nogravity: true);
                                    NPC.RedemptionGuard().GuardPoints = GuardPointMax;
                                    NPC.RedemptionGuard().GuardBroken = false;
                                }
                                if (AITimer >= 90)
                                {
                                    barkskinCooldown = 10;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(10);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Moonbeam
                        case 4:
                            AITimer++;
                            if (AITimer == 5)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.position.Y - 4, 0f, -10f, ModContent.ProjectileType<Moonbeam>(), 0, 0, Main.myPlayer);
                                Main.projectile[p].alpha = 150;
                                Main.projectile[p].hostile = false;
                                Main.projectile[p].netUpdate2 = true;
                            }
                            if (AITimer == 25)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 10f, ModContent.ProjectileType<Moonbeam>(), (int)(NPC.damage * 0.98f) / 4, 1, Main.myPlayer);
                            }
                            if (AITimer >= 70)
                            {
                                AIState = ActionState.ResetVars;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Entangle
                        case 5:
                            AttackID = Main.rand.Next(10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Healing Spirit
                        case 6:
                            if (NPC.life < (int)(NPC.lifeMax * 0.6f) && healingCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer % 10 == 0 && AITimer > 20 && AITimer < 200)
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0f, 0f, ModContent.ProjectileType<AkkaHealingSpirit>(), 0, 0, Main.myPlayer, NPC.whoAmI);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (ukkoActive && AITimer > 20 && AITimer < 200)
                                {
                                    NPC ukko = Main.npc[(int)NPC.ai[3]];
                                    Frame = 1;
                                    NPC.LookAtEntity(ukko);
                                    if (NPC.DistanceSQ(ukko.Center) < 200 * 200)
                                        NPC.velocity *= 0;
                                    else
                                        NPC.MoveToVector2(ukko.Center, 35);
                                }
                                else
                                    Frame = 0;
                                if (AITimer >= 260)
                                {
                                    healingCooldown = 20;
                                    AIState = ActionState.ResetVars;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(10);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Thorn Whip
                        case 7:
                            AttackID = Main.rand.Next(10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region TA: Bubbles
                        case 8:
                            if (teamCooldown == 0 && ukkoActive)
                            {
                                NPC ukko = Main.npc[(int)NPC.ai[3]];
                                if (ukko.ai[1] == 15)
                                {
                                    if (NPC.DistanceSQ(ukko.Center) < 300 * 300)
                                        NPC.velocity *= 0;
                                    else
                                        NPC.MoveToVector2(ukko.Center, 35);

                                    if (AITimer++ % 2 == 0 && AITimer < 70)
                                    {
                                        for (int i = 0; i < 2; i++)
                                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<AkkaBubble>(), NPC.damage, RedeHelper.Spread(16), false, SoundID.Item1);
                                    }
                                    if (AITimer >= 120)
                                    {
                                        teamCooldown = 4;
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                NPC.velocity *= 0;
                                AttackID = Main.rand.Next(10);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region TA: Earth Barrier
                        case 9:
                            if (teamCooldown == 0 && ukkoActive)
                            {
                                NPC ukko = Main.npc[(int)NPC.ai[3]];
                                if (ukko.ai[1] == 16)
                                {
                                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PoisonStaff);
                                    Main.dust[d].velocity.X = 0;
                                    Main.dust[d].velocity.Y -= 5;
                                    Main.dust[d].noGravity = true;
                                    Frame = 1;
                                    NPC.MoveToVector2(EarthProtectPos, 20);
                                    NPC.netUpdate = true;
                                    if (AITimer >= 100)
                                    {
                                        teamCooldown = 6;
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                NPC.velocity *= 0;
                                AttackID = Main.rand.Next(10);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
            }
        }
        private int frameCounters;
        private int magicFrame;
        private int Frame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
            if (Frame == 1)
            {
                frameCounters++;
                if (frameCounters > 6)
                {
                    magicFrame++;
                    frameCounters = 0;
                }
                if (magicFrame >= 6)
                    magicFrame = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public Vector2 Pos()
        {
            Vector2 Pos1 = new(player.Center.X > NPC.Center.X ? player.Center.X + Main.rand.Next(-300, -200) : player.Center.X + Main.rand.Next(200, 300), player.Center.Y + Main.rand.Next(-400, 200));
            return Pos1;
        }
        private void Target()
        {
            player = Main.player[NPC.target];
        }

        private void DespawnHandler()
        {
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0f, -20f);
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
        private float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D magicAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Spell").Value;
            Texture2D magicGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Spell_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGreen, Color.SpringGreen * 0.7f, Color.LightGreen);

            switch (Frame)
            {
                case 0:
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                        for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                        {
                            Vector2 oldPos = NPC.oldPos[i];
                            spriteBatch.Draw(texture, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                        }
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, NPC.Center - screenPos, NPC.frame, Color.LightGreen * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects);

                    spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    break;
                case 1:
                    int magicHeight = magicAni.Height / 6;
                    int magicY = magicHeight * magicFrame;
                    int magicGlowHeight = magicGlow.Height / 6;
                    int magicGlowY = magicGlowHeight * magicFrame;
                    Vector2 glowCenter = NPC.Center + new Vector2(15 * NPC.spriteDirection, -24);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(magicAni, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, magicY, magicAni.Width, magicHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(magicAni.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, magicAni, ref drawTimer, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, magicY, magicAni.Width, magicHeight)), Color.LightGreen * NPC.Opacity, NPC.rotation, new Vector2(magicAni.Width / 2f, magicHeight / 2f), NPC.scale, effects);

                    spriteBatch.Draw(magicAni, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, magicY, magicAni.Width, magicHeight)), drawColor * NPC.Opacity, NPC.rotation, new Vector2(magicAni.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(magicGlow, glowCenter - screenPos, new Rectangle?(new Rectangle(0, magicGlowY, magicGlow.Width, magicGlowHeight)), Color.White * NPC.Opacity, NPC.rotation, new Vector2(magicGlow.Width / 2f, magicGlowHeight / 2f), NPC.scale, effects, 0f);
                    break;
            }
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Dirt, Scale: 2);
                    Main.dust[dustIndex2].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Dirt);
        }
    }
}
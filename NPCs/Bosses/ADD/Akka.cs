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
using Redemption.Items.Weapons.PostML.Magic;
using Redemption.Items.Weapons.PostML.Summon;
using ReLogic.Content;
using Terraria.Localization;
using Redemption.Globals.NPC;
using Redemption.Helpers;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class Akka : ModNPC
    {
        private static Asset<Texture2D> magicAni;
        private static Asset<Texture2D> magicGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            magicAni = ModContent.Request<Texture2D>(Texture + "_Spell");
            magicGlow = ModContent.Request<Texture2D>(Texture + "_Spell_Glow");
        }
        public override void Unload()
        {
            magicAni = null;
            magicGlow = null;
        }
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
            // DisplayName.SetDefault("Akka");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 40),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
            ElementID.NPCEarth[Type] = true;
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossUkkoAkka");

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Blood] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Earth] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Nature] *= .75f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Poison] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Water] *= .9f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= 1.25f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Wind] *= 1.1f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileID.LastPrismLaser)
                modifiers.FinalDamage /= 3;

            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                modifiers.FinalDamage *= .75f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (RedeBossDowned.downedGGBossFirst == 1 && RedeBossDowned.downedGGBossFirst == 2)
                modifiers.FinalDamage *= .75f;

            if (NPC.RedemptionGuard().GuardPoints >= 0 && !NPC.RedemptionGuard().GuardBroken)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Dig with { Pitch = -.1f }, .25f, false, DustID.t_LivingWood, SoundID.Item43, 10, 1, 2000);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Rain,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Akka"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AkkaBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AkanKirvesTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<AkkaRelic>()));

            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<AkkaMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<PoemOfIlmatar>(), ModContent.ItemType<Pihlajasauva>()));

            npcLoot.Add(notExpertRule);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
            if (!RedeBossDowned.downedADD && !NPC.AnyNPCs(ModContent.NPCType<Ukko>()))
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
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.ADDDefeat"), 300, 30, 0, Color.DarkGoldenrod);

                }
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<Ukko>()) && !RedeBossDowned.downedADD && RedeBossDowned.downedGGBossFirst == 0)
                RedeBossDowned.downedGGBossFirst = 3;
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedADD, -1);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
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

            if (NPC.DespawnHandler(0, 20))
                return;
            Player player = Main.player[NPC.target];
            NPC.rotation = 0f;
            NPC.LookAtEntity(player);

            if (player.active && !player.dead)
                NPC.DiscourageDespawn(60);

            bool ukkoActive = false;
            if (NPC.ai[3] > -1 && Main.npc[(int)NPC.ai[3]].active && Main.npc[(int)NPC.ai[3]].type == ModContent.NPCType<Ukko>())
                ukkoActive = true;
            Vector2 EarthProtectPos = new(player.Center.X + (500 * NPC.RightOfDir(player)), player.Center.Y - 100);
            switch (AIState)
            {
                case ActionState.Start:
                    if (ukkoActive || RedeBossDowned.ADDDeath < 2)
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
                                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Akka.Name"), 60, 90, 0.8f, 0, Color.PaleGreen, Language.GetTextValue("Mods.Redemption.TitleCard.Akka.Modifier"));

                                    EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 50);
                                }

                                if (AITimer++ >= 170)
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
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Akka.Name"), 60, 90, 0.8f, 0, Color.PaleGreen, Language.GetTextValue("Mods.Redemption.TitleCard.Akka.Modifier"));

                        AIState = ActionState.ResetVars;
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
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<AkkaPoisonBubble>(), NPCHelper.HostileProjDamage((int)(NPC.damage * 0.95f)), 1);
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

                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 0f, ModContent.ProjectileType<AkkaIslandSummoner>(), NPCHelper.HostileProjDamage((int)(NPC.damage * 1.5f)), 1, Main.myPlayer);
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
                                    int hitDirection = NPC.RightOfDir(player);
                                    player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 40, hitDirection);
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
                                Main.projectile[p].netUpdate = true;
                            }
                            if (AITimer == 25)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 10f, ModContent.ProjectileType<Moonbeam>(), NPCHelper.HostileProjDamage((int)(NPC.damage * 0.98f)), 1, Main.myPlayer);
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
                            if (NPC.life < (int)(NPC.lifeMax * 0.6f) && ukkoActive && Main.npc[(int)NPC.ai[3]].life < (int)(Main.npc[(int)NPC.ai[3]].lifeMax * 0.75f) && healingCooldown == 0)
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
                                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<AkkaBubble>(), NPC.damage, RedeHelper.Spread(16));
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
            Vector2 Pos1 = new(player.Center.X + (Main.rand.Next(200, 300) * NPC.RightOfDir(player)), player.Center.Y + Main.rand.Next(-400, 200));
            return Pos1;
        }
        private void Target()
        {
            player = Main.player[NPC.target];
        }
        private float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGreen, Color.SpringGreen * 0.7f, Color.LightGreen);

            switch (Frame)
            {
                case 0:
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        spriteBatch.End();
                        spriteBatch.BeginAdditive(true);
                        GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                        for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                        {
                            Vector2 oldPos = NPC.oldPos[i];
                            spriteBatch.Draw(texture, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                        }
                        spriteBatch.End();
                        spriteBatch.BeginDefault();
                    }
                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, NPC.Center - screenPos, NPC.frame, Color.LightGreen * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects);

                    spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                    break;
                case 1:
                    int magicHeight = magicAni.Value.Height / 6;
                    int magicY = magicHeight * magicFrame;
                    int magicGlowHeight = magicGlow.Value.Height / 6;
                    int magicGlowY = magicGlowHeight * magicFrame;
                    Vector2 glowCenter = NPC.Center + new Vector2(15 * NPC.spriteDirection, -24);

                    spriteBatch.End();
                    spriteBatch.BeginAdditive(true);
                    GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(magicAni.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, magicY, magicAni.Value.Width, magicHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(magicAni.Value.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0);
                    }
                    spriteBatch.End();
                    spriteBatch.BeginDefault();

                    if (NPC.RedemptionGuard().GuardPoints > 0)
                        RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, magicAni.Value, ref drawTimer, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, magicY, magicAni.Value.Width, magicHeight)), Color.LightGreen * NPC.Opacity, NPC.rotation, new Vector2(magicAni.Value.Width / 2f, magicHeight / 2f), NPC.scale, effects);

                    spriteBatch.Draw(magicAni.Value, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, magicY, magicAni.Value.Width, magicHeight)), drawColor * NPC.Opacity, NPC.rotation, new Vector2(magicAni.Value.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0f);
                    spriteBatch.Draw(magicGlow.Value, glowCenter - screenPos, new Rectangle?(new Rectangle(0, magicGlowY, magicGlow.Value.Width, magicGlowHeight)), Color.White * NPC.Opacity, NPC.rotation, new Vector2(magicGlow.Value.Width / 2f, magicGlowHeight / 2f), NPC.scale, effects, 0f);
                    break;
            }
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
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

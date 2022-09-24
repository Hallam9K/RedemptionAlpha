using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Terraria.DataStructures;
using System;
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

        public override void SetDefaults()
        {
            NPC.lifeMax = 108000;
            NPC.damage = 115;
            NPC.defense = 50;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 8, 0, 0);
            NPC.aiStyle = -1;
            NPC.width = 46;
            NPC.height = 108;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest2");
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                damage = (int)(damage * .75f);
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
            /*npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThornBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThornTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThornRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<BouquetOfThorns>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ThornMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<CursedGrassBlade>(), ModContent.ItemType<RootTendril>(), ModContent.ItemType<CursedThornBow>(), ModContent.ItemType<BlightedBoline>()));

            npcLoot.Add(notExpertRule);*/
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


        public Vector2 MoveVector2;
        public Vector2 MoveVector3;
        public int islandCooldown = 10;
        public int barkskinCooldown;
        public int healingCooldown;
        public int TremorTimer;
        public bool teamAttackCheck;
        public int frameCounters;
        public int magicFrame;
        public override void AI()
        {
            Target();

            DespawnHandler();

            Player player = Main.player[NPC.target];
            NPC.rotation = 0f;
            NPC.LookAtEntity(player);
            /*if (NPC.ai[3] == 1)
            {
                frameCounters++;
                if (frameCounters > 6)
                {
                    magicFrame++;
                    frameCounters = 0;
                }
                if (magicFrame >= 6)
                {
                    magicFrame = 0;
                }
            }*/
            //Vector2 EarthProtectPos = new(player.Center.X > NPC.Center.X ? player.Center.X - 500 : player.Center.X + 500, player.Center.Y - 100);
            switch (AIState)
            {
                case ActionState.Start:
                    if (NPC.ai[3] > -1)
                    {
                        NPC ukko = Main.npc[(int)NPC.ai[3]];
                        if (ukko.active && ukko.type == ModContent.NPCType<Ukko>())
                        {
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
                                        AIState = ActionState.ResetVars;
                                        AITimer = 0;
                                        AttackID = 0;
                                        NPC.netUpdate = true;
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (!Main.dedServ)
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Akka", 60, 90, 0.8f, 0, Color.LightGreen, "Ancient Goddess of Nature");

                        AIState = ActionState.ResetVars;
                        AITimer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.ResetVars:
                    if (islandCooldown > 0)
                        islandCooldown--;
                    if (barkskinCooldown > 0)
                        barkskinCooldown--;
                    if (healingCooldown > 0)
                        healingCooldown--;
                    MoveVector2 = Pos();
                    NPC.ai[0]++;
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(MoveVector2) < 10 * 10)
                    {
                        NPC.velocity *= 0;
                        NPC.ai[0]++;
                        AttackID = Main.rand.Next(8);
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.MoveToVector2(MoveVector2, 35);
                    break;
                case ActionState.Attacks:
                    switch (AttackID)
                    {
                        // Poison Spray
                        #region Poison Spray
                        case 0:
                            AITimer++;
                            if (AITimer % 6 == 0 && AITimer < 60)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<AkkaPoisonBubble>(), 60 / 3, 1);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (AITimer >= 65)
                            {
                                NPC.ai[0] = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Floating Island Yeet
                        #region Island Bombardment
                        case 1:
                            if (islandCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer == 30)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Quake with { Volume = 1.4f, PitchVariance = 0.1f });

                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 0f, ModContent.ProjectileType<AkkaIslandSummoner>(), 200 / 3, 1, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (AITimer >= 200)
                                {
                                    islandCooldown = 30;
                                    NPC.ai[0] = 1;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(8);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Earth Tremor
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
                                NPC.ai[0] = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Barkskin
                        #region Barkskin
                        case 3:
                            if (NPC.life < (int)(NPC.lifeMax * 0.8f) && barkskinCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer < 60)
                                {
                                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_LivingWood, 0f, 0f, 100, default, 2f);
                                    dust.velocity = -NPC.DirectionTo(dust.position);

                                    NPC.AddBuff(ModContent.BuffType<StoneskinBuff>(), 3600);
                                }
                                if (AITimer >= 90)
                                {
                                    barkskinCooldown = 10;
                                    NPC.ai[0] = 1;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(8);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Moonbeam
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
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 1000, 0f, 10f, ModContent.ProjectileType<Moonbeam>(), 100 / 3, 1, Main.myPlayer);
                            }
                            if (AITimer >= 70)
                            {
                                NPC.ai[0] = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Entangle
                        #region Entangle
                        case 5:
                            if (player.ZoneOverworldHeight)
                            {
                                AITimer++;
                                if (AITimer % 10 == 0 && AITimer > 40 && AITimer < 160)
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + Main.rand.Next(-300, 300), player.Center.Y + Main.rand.Next(-300, 0), 0f, 0f, ModContent.ProjectileType<AkkaSeed>(), 90 / 3, 3, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (AITimer >= 200)
                                {
                                    NPC.ai[0] = 1;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(8);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Healing Spirit
                        #region Healing Spirit
                        case 6:
                            if (NPC.life < (int)(NPC.lifeMax * 0.6f) && healingCooldown == 0)
                            {
                                AITimer++;
                                if (AITimer % 10 == 0 && AITimer > 20 && AITimer < 200)
                                {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0f, 0f, ModContent.ProjectileType<AkkaHealingSpirit>(), 0, 0, Main.myPlayer);
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (AITimer >= 260)
                                {
                                    healingCooldown = 20;
                                    NPC.ai[0] = 1;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackID = Main.rand.Next(8);
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        // Thorn Whip
                        #region Thorn Whip
                        case 7:
                            AITimer++;
                            if (AITimer % 30 == 0 && AITimer < 100)
                            {
                                float Speed = 16f;
                                Vector2 vector8 = new(NPC.Center.X, NPC.Center.Y);
                                int damage = 46;
                                int type = ModContent.ProjectileType<Akka_CursedThorn>();
                                float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                int num54 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
                                Main.projectile[num54].netUpdate = true;
                            }
                            if (AITimer >= 120)
                            {
                                NPC.ai[0] = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
            }
        }
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
        public override bool CheckActive()
        {
            return !player.active || player.dead;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D magicAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Spell").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightGreen, Color.SpringGreen * 0.7f, Color.LightGreen);

            //switch (NPC.ai[3])
            //{
            //    case 0:
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
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            /*break;
        case 1:
            int magicHeight = magicAni.Height / 6;
            int magicY = magicHeight * magicFrame;
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                spriteBatch.Draw(magicAni, oldPos + new Vector2(4, -16) + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(new Rectangle(0, magicY, magicAni.Width, magicHeight)), NPC.GetAlpha(shaderColor) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, new Vector2(magicAni.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(magicAni, drawCenter - screenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, magicY, magicAni.Width, magicHeight)), drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(magicAni.Width / 2f, magicHeight / 2f), NPC.scale, effects, 0f);
            break;*/
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
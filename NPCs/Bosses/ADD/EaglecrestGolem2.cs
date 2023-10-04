using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.Particles;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    [AutoloadBossHead]
    public class EaglecrestGolem2 : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/EaglecrestGolem/EaglecrestGolem";
        public enum ActionState
        {
            Start,
            Idle,
            Slash,
            Roll,
            Laser,
            Transform
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

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            if (RedeBossDowned.ADDDeath > 0)
                NPC.lifeMax = 60000;
            else
                NPC.lifeMax = 140000;
            NPC.damage = 110;
            NPC.defense = 48;
            NPC.knockBackResist = 0f;
            NPC.value = 0;
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
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<StonePuppet>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GolemEye>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestJavelin>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EaglecrestSling>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GathicStone>(), 1, 14, 34));
        }
        public override bool CheckActive() => false;
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        private int AniFrameY;
        private int summonTimer;
        private float FlareTimer;
        private bool Flare;
        private Vector2 playerOrg;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.DespawnHandler(1))
                return;
            if (AIState != ActionState.Slash && AIState != ActionState.Laser)
                NPC.LookAtEntity(player);

            float moveInterval = 0.14f;
            float moveSpeed = 9f;
            if (NPC.life < (int)(NPC.lifeMax / 1.5))
            {
                moveInterval = 0.28f;
                moveSpeed = 12f;
            }
            if (NPC.life <= NPC.lifeMax / 2 && AIState < (ActionState)5)
            {
                AIState = ActionState.Transform;
                AITimer = 0;
                TimerRand2 = 0;
                TimerRand = 0;
                NPC.netUpdate = true;
            }

            switch (AIState)
            {
                case ActionState.Start:
                    NPC.target = RedeHelper.GetNearestAlivePlayer(NPC);
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Golem.Name"), 60, 90, 0.8f, 0, Color.Gray, Language.GetTextValue("Mods.Redemption.TitleCard.Golem.Modifier"));

                    TimerRand = Main.rand.Next(300, 600);
                    AIState = ActionState.Idle;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Idle:
                    if (++AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand2 = 0;
                        TimerRand = Main.rand.Next(400, 700);
                        AIState = ActionState.Roll;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(player.Center) <= 800 * 800 && Main.rand.NextBool(150))
                    {
                        NPC.velocity.X = 0;
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Slash;
                        NPC.netUpdate = true;
                    }

                    if (NPC.DistanceSQ(player.Center) > 150 * 150 && Main.rand.NextBool(300))
                    {
                        TimerRand2 = 0;
                        AIState = ActionState.Laser;
                        NPC.netUpdate = true;
                    }

                    summonTimer--;
                    if (Main.rand.NextBool(100) && summonTimer <= 0 && NPC.CountNPCS(ModContent.NPCType<EaglecrestRockPile2>()) < 1)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockPileSummon>(), 0, RedeHelper.SpreadUp(20), NPC.whoAmI);
                        }
                        summonTimer = 600;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                    NPCHelper.HorizontallyMove(NPC, player.Center, moveInterval, moveSpeed, 20, 26, NPC.Center.Y > player.Center.Y, player);
                    break;
                case ActionState.Roll:
                    if (TimerRand2 == 0)
                    {
                        if (NPC.DistanceSQ(player.Center) > 150 * 150 && Main.rand.NextBool(300))
                        {
                            TimerRand2 = 1;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        Vector2 origin2 = NPC.Center;
                        if (++TimerRand2 < 30)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                Vector2 vector;
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vector.X = (float)(Math.Sin(angle) * 30);
                                vector.Y = (float)(Math.Cos(angle) * 30);
                                Dust dust2 = Main.dust[Dust.NewDust(origin2 + vector, 2, 2, DustID.Sandnado)];
                                dust2.noGravity = true;
                                dust2.velocity = dust2.position.DirectionTo(origin2) * 5f;
                            }
                        }
                        if (TimerRand2 == 10)
                            playerOrg = player.Center;
                        if (TimerRand2 == 30)
                        {
                            NPC.Shoot(origin2, ModContent.ProjectileType<GolemEyeRay>(), NPC.damage, RedeHelper.PolarVector(10, (playerOrg - NPC.Center).ToRotation()), SoundID.Item109, NPC.whoAmI);
                        }
                        FlareTimer += 2;
                        if (TimerRand2 >= 30)
                        {
                            FlareTimer = 0;
                            Flare = true;
                        }
                        if (TimerRand2 > 90)
                        {
                            TimerRand2 = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    NPC.Move(player.Center, 20, 35);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.netUpdate = true;
                    AITimer++;
                    if (AITimer >= TimerRand && Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(new Vector2(NPC.Center.X, NPC.position.Y - NPC.height / 2 + 10), NPC.width, NPC.height))
                    {
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;

                        NPC.velocity.Y -= 6;
                        AITimer = 0;
                        TimerRand2 = 0;
                        TimerRand = Main.rand.Next(300, 600);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Laser:
                    Vector2 origin = NPC.Center - new Vector2(-2 * NPC.spriteDirection, 18);
                    if (++TimerRand2 < 30)
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
                    if (TimerRand2 == 30)
                    {
                        NPC.Shoot(origin, ModContent.ProjectileType<GolemEyeRay>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(90 * NPC.spriteDirection)), SoundID.Item109, NPC.whoAmI);
                    }
                    if (TimerRand2 >= 30)
                    {
                        FlareTimer = 0;
                        Flare = true;
                    }

                    if (TimerRand2 > 90)
                    {
                        TimerRand2 = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                    NPCHelper.HorizontallyMove(NPC, player.Center, moveInterval, moveSpeed, 20, 26, NPC.Center.Y > player.Center.Y, player);
                    break;
                case ActionState.Transform:
                    NPC.velocity *= 0.9f;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;
                    NPC.dontTakeDamage = true;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);

                    NPC.velocity.Y = 2f;
                    for (int i = 0; i < 3; i++)
                    {
                        int dustIndex2 = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f);
                        Main.dust[dustIndex2].velocity *= 0f;
                        Main.dust[dustIndex2].noGravity = true;
                    }
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
                    switch (TimerRand)
                    {
                        case 0:
                            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1).UseIntensity(TimerRand2).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
                            player.RedemptionScreen().customZoom = TimerRand2 + 1;
                            player.RedemptionScreen().Rumble(10, (int)TimerRand2 * 3);

                            Vector2 eyePos = NPC.Center - new Vector2(-2 * NPC.spriteDirection, 18);
                            if (AITimer++ == 0)
                            {
                                if (!Main.dedServ)
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
                            }
                            if (AITimer == 60)
                            {
                                for (int i = 0; i < 35; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 2);
                                    Main.dust[dustIndex2].velocity *= 3f;
                                }
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 14;
                                if (!Main.dedServ)
                                {
                                    SoundEngine.PlaySound(CustomSounds.Quake, NPC.position);
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom, NPC.position);
                                }
                                SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.position);
                                flashOpacity = 1;
                            }
                            if (AITimer == 90 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Spark1, NPC.position);
                            if (AITimer >= 90 && AITimer < 130 && !Main.rand.NextBool(3))
                            {
                                DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, eyePos + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                                DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, eyePos + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                            }
                            if (AITimer == 180)
                            {
                                for (int i = 0; i < 35; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 2);
                                    Main.dust[dustIndex2].velocity *= 3f;
                                }
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 24;
                                if (!Main.dedServ)
                                {
                                    SoundEngine.PlaySound(CustomSounds.Quake with { Pitch = 0.1f }, NPC.position);
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Pitch = 0.1f }, NPC.position);
                                }
                                SoundEngine.PlaySound(SoundID.NPCDeath43 with { Pitch = 0.1f }, NPC.position);
                                flashOpacity = 1.5f;
                            }
                            if (AITimer == 210 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.1f }, NPC.position);
                            if (AITimer >= 210 && AITimer < 250 && Main.rand.NextBool(2))
                            {
                                DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                                DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                            }
                            if ((AITimer >= 60 && AITimer < 90) || (AITimer >= 180 && AITimer < 210))
                            {
                                TimerRand2 += 0.005f;
                            }
                            if (AITimer == 290)
                            {
                                for (int i = 0; i < 35; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 2);
                                    Main.dust[dustIndex2].velocity *= 3f;
                                }
                                if (!Main.dedServ)
                                {
                                    SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.2f }, NPC.position);
                                    SoundEngine.PlaySound(CustomSounds.EnergyCharge with { Pitch = 0.1f }, NPC.position);
                                }
                                SoundEngine.PlaySound(SoundID.Thunder with { Pitch = -.2f }, NPC.position);
                            }
                            if (AITimer >= 290)
                            {
                                flashOpacity += 0.06f;
                                TimerRand2 += 0.01f;
                                if (Main.rand.NextBool(10) || (Main.rand.NextBool(5) && AITimer >= 340))
                                {
                                    DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                                    DustHelper.DrawParticleElectricity<LightningParticle>(eyePos, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                                }
                            }
                            if (TimerRand2 >= 2f)
                            {
                                TimerRand = 1;
                                AITimer = 0;
                            }
                            flashOpacity -= 0.05f;
                            flashOpacity = MathHelper.Max(0, flashOpacity);
                            break;
                        case 1:
                            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1).UseIntensity(TimerRand2).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
                            player.RedemptionScreen().customZoom = TimerRand2 + 1;
                            player.RedemptionScreen().Rumble(10, (int)AITimer / 50);

                            Main.rainTime = 3600;
                            Main.raining = true;
                            Main.maxRaining = 0.7f;
                            Main.windSpeedTarget = 1;
                            if (AITimer++ == 10)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Main.StopRain();
                                    Main.SyncRain();
                                    if (Main.netMode != NetmodeID.SinglePlayer)
                                        NetMessage.SendData(MessageID.WorldData);
                                }

                                if (!Main.dedServ)
                                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossUkko");
                            }
                            if (AITimer == 50)
                                Main.BlackFadeIn = 255;
                            if (AITimer >= 50)
                            {
                                if (Main.BlackFadeIn >= 0)
                                    Main.BlackFadeIn += 20;
                                TimerRand2 -= 0.004f;
                                godrayFade += 0.01f;
                                if (NPC.life < NPC.lifeMax - 500)
                                    NPC.life += 500;
                                else
                                    NPC.life = NPC.lifeMax;

                                if (Main.rand.NextBool(5))
                                {
                                    DustHelper.DrawParticleElectricity<LightningParticle>(NPC.Center, NPC.Center + RedeHelper.PolarVector(50 + AITimer, RedeHelper.RandomRotation()), 2f, 20, 0.1f, 1);
                                    DustHelper.DrawParticleElectricity<LightningParticle>(NPC.Center, NPC.Center + RedeHelper.PolarVector(50 + AITimer, RedeHelper.RandomRotation()), 2f, 20, 0.1f, 1);
                                }

                                if (!Main.dedServ)
                                {
                                    if (AITimer == 100)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("A storm is brewing...", 60, 20, 1f, "", 1, Color.LightGoldenrodYellow, Color.Black * 0f, new Vector2(NPC.Center.X - 60, NPC.Center.Y - 400) - Main.screenPosition, null, 0);

                                    if (AITimer == 200)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("... And nature trembles", 60, 20, 1f, "", 1, Color.LightGreen, Color.Black * 0f, new Vector2(NPC.Center.X + 60, NPC.Center.Y - 300) - Main.screenPosition, null, 0);

                                    if (AITimer == 300)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("The Gods' wrath is upon you", 60, 20, 1f, "", 1, Color.IndianRed, Color.Black * 0f, new Vector2(NPC.Center.X, NPC.Center.Y - 200) - Main.screenPosition, null, 0);
                                }
                                if (AITimer == 520)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        if (RedeBossDowned.ADDDeath < 1)
                                            RedeBossDowned.ADDDeath = 1;
                                        if (Main.netMode != NetmodeID.SinglePlayer)
                                            NetMessage.SendData(MessageID.WorldData);
                                    }
                                    RedeDraw.SpawnExplosion(NPC.Center, Color.White * 0.6f, shakeAmount: 30, scale: 10, noDust: true);
                                    Main.NewLightning();
                                    SoundEngine.PlaySound(SoundID.Thunder, NPC.position);

                                    NPC.netUpdate = true;
                                    NPC.Transform(ModContent.NPCType<Ukko>());
                                }
                            }
                            else
                            {
                                Main.BlackFadeIn += 50;
                            }
                            TimerRand2 = MathHelper.Max(0, TimerRand2);
                            break;
                    }
                    break;
            }
        }
        public override void PostAI()
        {
            CustomFrames();
        }
        private void CustomFrames()
        {
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
                if (++NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AniFrameY is 6)
                    {
                        Player player = Main.player[NPC.target];
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockSlash_Proj>(), NPC.damage, RedeHelper.PolarVector(20, (player.Center - NPC.Center).ToRotation()), SoundID.Item71, NPC.whoAmI);
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
                NPC.frame.Y = 12 * 84;
                return;
            }
            else
            {
                NPC.width = 80;
                NPC.height = 80;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (AIState is ActionState.Slash)
                return;
            AniFrameY = 0;

            if (AIState is ActionState.Roll)
                return;
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
        private float flashOpacity;
        private float godrayFade;
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

            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = NPC.scale * (1 + fluctuate);

                Color godrayColor = Color.Lerp(new Color(255, 255, 120), Color.White * NPC.Opacity, 0.5f);
                godrayColor.A = 0;
                RedeDraw.DrawGodrays(Main.spriteBatch, NPC.Center - Main.screenPosition, godrayColor * godrayFade, 70 * modifiedScale * godrayFade, 15 * modifiedScale * godrayFade, 21);
            }

            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, Color.LightGoldenrodYellow * flashOpacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos - new Vector2(-2 * NPC.spriteDirection, 18);
                if (AIState == ActionState.Roll)
                    position = NPC.Center - screenPos;
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.Orange, NPC.rotation);
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

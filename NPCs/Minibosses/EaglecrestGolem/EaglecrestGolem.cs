using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Particles;
using Redemption.Projectiles.Magic;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
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
            Laser,
            Boulders,
            Stomp
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
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;

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
            NPC.damage = 40;
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (AIState is ActionState.Roll or ActionState.Boulders) || (AIState is ActionState.Stomp && TimerRand > 0);
        public override bool CanHitNPC(NPC target) => target.friendly && ((AIState is ActionState.Roll or ActionState.Boulders) || (AIState is ActionState.Stomp && TimerRand > 0));

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
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AniFrameY);
            writer.Write(summonTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AniFrameY = reader.ReadInt32();
            summonTimer = reader.ReadInt32();
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

            if (NPC.DespawnHandler(1))
                return;

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
                        TimerRand2 = 0;
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
                    bool chance = Main.rand.NextBool(2);
                    if (NPC.life <= (int)(NPC.lifeMax * .4f))
                        chance = true;
                    if (AITimer == (int)(TimerRand / 2) && chance && NPC.life <= (int)(NPC.lifeMax * .7f))
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                        AIState = ActionState.Stomp;
                        NPC.netUpdate = true;
                    }

                    summonTimer--;
                    if (Main.rand.NextBool(100) && summonTimer <= 0 && NPC.CountNPCS(ModContent.NPCType<EaglecrestRockPile>()) < 1)
                    {
                        int amt = 2;
                        if (NPC.life <= (int)(NPC.lifeMax * .65f))
                            amt = 3;
                        if (NPC.life <= NPC.lifeMax / 3)
                            amt = 4;
                        for (int i = 0; i < amt; i++)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockPileSummon>(), 0, RedeHelper.SpreadUp(16), NPC.whoAmI);
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

                    if (BaseAI.HitTileOnSide(NPC, 3) && (NPC.velocity.X >= 2 || NPC.velocity.X <= -2))
                        Dust.NewDust(new Vector2(NPC.Center.X, NPC.Bottom.Y) - Vector2.One, 2, 2, DustID.Stone, -NPC.velocity.X, -2, Scale: Main.rand.NextFloat(.8f, 1.5f));
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
                            break;
                        }

                        NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                        NPCHelper.HorizontallyMove(NPC, player.Center, 0.12f, 10, 20, 30, NPC.Center.Y > player.Center.Y, player);

                        Rectangle tooHighCheck = new((int)NPC.Center.X - 300, (int)NPC.Center.Y - 900, 600, 800);
                        if (Main.rand.NextBool(player.Hitbox.Intersects(tooHighCheck) ? 100 : 1000))
                        {
                            AITimer = 0;
                            TimerRand = 0;
                            AIState = ActionState.Boulders;
                            NPC.netUpdate = true;
                        }
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
                        NPC.Shoot(origin, ModContent.ProjectileType<GolemEyeRay>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(20 * NPC.spriteDirection)), SoundID.Item109, NPC.whoAmI);
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
                case ActionState.Boulders:
                    switch (TimerRand)
                    {
                        case 0:
                            NPC.velocity.X *= .96f;
                            if (AITimer++ >= 40)
                            {
                                NPC.velocity.X = 0;
                                AITimer = 0;
                                TimerRand++;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Quake with { Volume = 0.5f }, NPC.position);
                            }
                            break;
                        case 1:
                            AITimer += .01f;
                            NPC.rotation += AITimer * NPC.spriteDirection;
                            glowOpacity += .02f;

                            Dust.NewDust(new Vector2(NPC.Center.X, NPC.Bottom.Y) - Vector2.One, 2, 2, DustID.Stone, AITimer * 30, -2, Scale: Main.rand.NextFloat(.8f, 1.5f));
                            Dust.NewDust(new Vector2(NPC.Center.X, NPC.Bottom.Y) - Vector2.One, 2, 2, DustID.Stone, -AITimer * 30, -2, Scale: Main.rand.NextFloat(.8f, 1.5f));
                            if (!Main.dedServ && AITimer >= .24f && AITimer <= .26f)
                                SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = -.4f }, NPC.position);
                            if (AITimer >= .25f && AITimer < .5f && Main.rand.NextBool(6))
                            {
                                origin = NPC.Center - new Vector2(-2 * NPC.spriteDirection, 18);
                                DustHelper.DrawParticleElectricity<LightningParticle>(origin, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                                DustHelper.DrawParticleElectricity<LightningParticle>(origin, NPC.Center + RedeHelper.PolarVector(180, RedeHelper.RandomRotation()), 1f, 20, 0.1f, 1);
                            }
                            if (glowOpacity >= 0.9f)
                            {
                                customScale.Y -= .04f;
                                customScale.X += .06f;
                            }
                            if (glowOpacity > 1)
                            {
                                AITimer = 0;
                                TimerRand++;
                                customScale = new Vector2(-.2f, .2f);
                                NPC.noTileCollide = true;
                                NPC.velocity.Y -= MathHelper.Max(10, MathHelper.Distance(NPC.Center.Y, player.Center.Y) / 22);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom with { Pitch = 0.1f }, NPC.position);

                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 20;

                                for (int i = 0; i < 35; i++)
                                {
                                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone, Scale: 2);
                                    Main.dust[dustIndex2].velocity.Y += NPC.velocity.Y;
                                    Main.dust[dustIndex2].velocity.X *= .2f;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -4; i <= 4; i += 2)
                                    {
                                        if (i != 0)
                                        {
                                            SoundEngine.PlaySound(SoundID.Item88, NPC.Center);
                                            int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(i, NPC.velocity.Y), ModContent.ProjectileType<EaglecrestBoulder_Proj>(), NPCHelper.HostileProjDamage(NPC.damage), 3, Main.myPlayer, 0, 1);
                                            Main.projectile[proj].hostile = true;
                                            Main.projectile[proj].friendly = false;
                                            Main.projectile[proj].DamageType = DamageClass.Default;
                                            Main.projectile[proj].netUpdate = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (NPC.velocity.Y > 0)
                                NPC.noTileCollide = false;
                            NPC.rotation += .3f;
                            glowOpacity -= .1f;
                            if (customScale.X < 0)
                                customScale.X += .05f;
                            if (customScale.Y > 0)
                                customScale.Y -= .05f;

                            if (BaseAI.HitTileOnSide(NPC, 3))
                            {
                                customScale = Vector2.Zero;
                                NPC.noTileCollide = false;
                                glowOpacity = 0;
                                NPC.velocity.Y -= 8;
                                AITimer = 0;
                                TimerRand = Main.rand.Next(300, 700);
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Stomp:
                    if (NPC.velocity.Y < 0 || Collision.SolidCollision(NPC.Center - Vector2.One, 2, 2))
                        NPC.noTileCollide = true;
                    else
                        NPC.noTileCollide = false;
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer++ == 0)
                                NPC.velocity.Y = -10;
                            if (NPC.velocity.Y >= 0)
                            {
                                AITimer = 0;
                                TimerRand++;
                                NPC.noGravity = true;
                            }
                            break;
                        case 1:
                            NPC.velocity.Y += .3f;
                            if (NPC.velocity.Y > 2)
                                NPC.velocity.Y += .6f;

                            if (glowOpacity > 0 && NPC.life <= NPC.lifeMax / 4)
                                glowOpacity -= .1f;
                            if (AITimer-- <= 0 && BaseAI.HitTileOnSide(NPC, 3, false) && !Collision.SolidCollision(NPC.Center - Vector2.One, 2, 2) && glowOpacity < 1)
                            {
                                AITimer = 4;
                                Vector2 pos = new(player.Center.X, BaseWorldGen.GetFirstTileFloor((int)player.Center.X / 16, (int)player.Center.Y / 16) * 16);
                                Vector2 landingPos = NPCHelper.FindGroundVector(NPC, pos, 30);

                                player.RedemptionScreen().ScreenShakeIntensity += 15;
                                player.RedemptionScreen().ScreenShakeOrigin = NPC.Center;

                                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.position);
                                for (int i = 0; i < 10; i++)
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone,
                                        -NPC.velocity.X * 0.01f, -NPC.velocity.Y * 0.6f, Scale: 2);
                                if (NPC.life <= NPC.lifeMax / 4)
                                {
                                    glowOpacity = 1;
                                    RedeDraw.SpawnExplosion(NPC.Center, new Color(255, 255, 174), shakeAmount: 0, scale: 1, noDust: true);
                                    for (int i = 0; i < 3; i++)
                                        DustHelper.DrawParticleElectricity<LightningParticle>(NPC.Center - new Vector2(0, 400), NPC.Center, 2f, 30, 0.1f, 1);
                                    DustHelper.DrawCircle(NPC.Center - new Vector2(0, 400), DustID.Sandnado, 1, 4, 4, 1, 3, nogravity: true);

                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.Thunderstrike, NPC.position);

                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0, 400), new Vector2(0, 5), ModContent.ProjectileType<EaglecrestJavelin_Thunder>(), NPCHelper.HostileProjDamage((int)(NPC.damage * .95f)), 8, Main.myPlayer);
                                    Main.projectile[proj].DamageType = DamageClass.Default;
                                    Main.projectile[proj].hostile = true;
                                    Main.projectile[proj].friendly = false;
                                    Main.projectile[proj].netUpdate = true;
                                }
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom, NPC.position);

                                PunchCameraModifier camPunch = new(NPC.Center, new Vector2(0f, -1f), 20f, 6f, 30, 1000f, "Eaglecrest Golem");
                                Main.instance.CameraModifiers.Add(camPunch);

                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<EaglecrestGolem_ShockwaveSpawner>(), NPC.damage, Vector2.Zero, NPC.velocity.Y * 5, NPC.velocity.Y);

                                NPC.velocity = RedeHelper.GetArcVel(NPC, landingPos, 0.3f, Main.rand.Next(80, 400), 500, maxXvel: 10);
                                int amt = 2;
                                if (NPC.life <= NPC.lifeMax / 2)
                                    amt = 3;
                                if (NPC.life <= NPC.lifeMax / 4)
                                    amt = 4;
                                if (TimerRand2++ > amt)
                                    TimerRand++;
                            }
                            break;
                        case 2:
                            NPC.velocity.Y += .3f;
                            if (glowOpacity > 0 && NPC.life <= NPC.lifeMax / 4)
                                glowOpacity -= .1f;

                            if (NPC.velocity.Y >= 0)
                            {
                                if (BaseAI.HitTileOnSide(NPC, 3, false))
                                    NPC.velocity.Y -= 8;
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                glowOpacity = 0;
                                TimerRand = Main.rand.Next(300, 700);
                                AITimer = TimerRand / 2;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
            CustomFrames(84);
        }
        private void CustomFrames(int frameHeight)
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
                if (++NPC.frameCounter >= (TimerRand2 is 1 ? 4 : 5))
                {
                    Player player = Main.player[NPC.target];

                    NPC.frameCounter = 0;
                    if (TimerRand2 != 1)
                        AniFrameY++;
                    else
                        AniFrameY--;
                    if (AniFrameY is 6)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<RockSlash_Proj>(), NPC.damage, RedeHelper.PolarVector(16,
                            (player.Center - NPC.Center).ToRotation()), SoundID.Item71, NPC.whoAmI);
                    }
                    if (AniFrameY <= 0)
                    {
                        TimerRand2 = 0;
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    if (AniFrameY == 4 && TimerRand2 > 0 && NPC.life <= NPC.lifeMax / 2 && Main.rand.NextBool(2))
                    {
                        NPC.LookAtEntity(player);
                        TimerRand2 = 2;
                        NPC.netUpdate = true;
                        return;
                    }
                    if (AniFrameY > 8)
                    {
                        if (TimerRand2 <= 0)
                        {
                            NPC.LookAtEntity(player);
                            AniFrameY = 8;
                            TimerRand2 = 1;
                            NPC.netUpdate = true;
                            return;
                        }
                        TimerRand2 = 0;
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                }
                return;
            }
            AniFrameY = 0;

            if ((AIState is ActionState.Roll or ActionState.Boulders) || (AIState is ActionState.Stomp && TimerRand > 0))
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
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Slash)
                return;
            AniFrameY = 0;

            if ((AIState is ActionState.Roll or ActionState.Boulders) || (AIState is ActionState.Stomp && TimerRand > 0))
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
        private float glowOpacity;
        private Vector2 customScale;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D SlashAni = ModContent.Request<Texture2D>(Texture + "_Slash").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new(NPC.scale + customScale.X, NPC.scale + customScale.Y);
            if (!NPC.IsABestiaryIconDummy && ((AIState is ActionState.Roll or ActionState.Boulders) || (AIState is ActionState.Stomp && TimerRand > 0)))
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, scale, effects, 0);
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
                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, scale, effects, 0f);
                if (glowOpacity > 0)
                {
                    int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
                    Main.spriteBatch.End();
                    Main.spriteBatch.BeginAdditive(true);
                    GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

                    for (int k = 0; k < NPCID.Sets.TrailCacheLength[NPC.type]; k++)
                    {
                        Vector2 oldPos = NPC.oldPos[k];
                        Color color = NPC.GetAlpha(new Color(255, 255, 174)) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color * .5f * glowOpacity, oldrot[k], NPC.frame.Size() / 2, scale, effects, 0);
                    }
                    spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(new Color(255, 255, 174)) * glowOpacity, NPC.rotation, NPC.frame.Size() / 2, scale, effects, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.BeginDefault();
                }
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
    public class EaglecrestGolem_ShockwaveSpawner : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ % 2 == 0)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 origin = Projectile.Center;
                    origin.X += Projectile.localAI[0] * 16 * i;
                    int numtries = 0;
                    int x = (int)(origin.X / 16);
                    int y = (int)(origin.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        origin.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        origin.Y = y * 16;
                    }
                    if (numtries >= 20)
                        break;

                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), origin + new Vector2(0, 2), Vector2.Zero, ModContent.ProjectileType<Golem_GroundShock>(), Projectile.damage, 3, Main.myPlayer, Projectile.ai[1]);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.2f }, Main.projectile[proj].Center);
                }
            }
            if (Projectile.localAI[0] >= Projectile.ai[0])
                Projectile.Kill();
        }
    }
}

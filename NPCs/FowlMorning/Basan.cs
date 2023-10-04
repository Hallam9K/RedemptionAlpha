using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Globals.World;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.NPCs.FowlMorning
{
    [AutoloadBossHead]
    public class Basan : ModNPC
    {
        private static Asset<Texture2D> glowMask;
        private static Asset<Texture2D> fireBreath;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glowMask = ModContent.Request<Texture2D>(Texture + "_Glow");
            fireBreath = ModContent.Request<Texture2D>(Texture + "_FirebreathOV");
        }
        public override void Unload()
        {
            glowMask = null;
            fireBreath = null;
        }
        public enum ActionState
        {
            Idle,
            Gust,
            Crow,
            Fly,
            Firebreath
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Hot);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                CustomTexturePath = "Redemption/CrossMod/BossChecklist/Basan",
                Position = new Vector2(0, 10),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCFire[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1400;
            NPC.damage = 30;
            NPC.defense = 4;
            NPC.knockBackResist = 0f;
            NPC.value = 5000;
            NPC.aiStyle = -1;
            NPC.width = 44;
            NPC.height = 84;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 4f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.boss = true;
            tailChain = new BasanScarfPhys();
            tailChain2 = new BasanScarfPhys();
            tailChain3 = new BasanScarfPhys();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/FowlMorning");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= .65f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Water] *= 1.2f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Ice] *= 1.1f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Wind] *= 1.1f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AniType is (int)AnimType.None && NPC.velocity.Y != 0;
        public override bool CanHitNPC(NPC target) => NPC.friendly && AniType is (int)AnimType.None && NPC.velocity.Y != 0;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.expertMode)
                target.AddBuff(BuffID.OnFire, 180);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if (Main.expertMode)
                target.AddBuff(BuffID.OnFire, 180);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Basan"))
            });
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(180, 201);
            NPC.netUpdate = true;
        }
        public int AniType;
        public enum AnimType { None, Gust, Crow, Firebreath }

        private static IPhysChain tailChain;
        private static IPhysChain tailChain2;
        private static IPhysChain tailChain3;
        private int AttackType;
        private Vector2 playerOld;
        public override void AI()
        {
            if (NPC.TryGetGlobalNPC(out NPCPhysChain chain))
            {
                chain.npcPhysChain[0] = tailChain;
                chain.npcPhysChain[1] = tailChain2;
                chain.npcPhysChain[2] = tailChain3;
                chain.npcPhysChainOffset[0] = RedeHelper.PolarVector(-8 * NPC.spriteDirection, NPC.rotation) + RedeHelper.PolarVector(6 * NPC.spriteDirection, NPC.rotation - (float)Math.PI / 2 + (NPC.spriteDirection == 1 ? (float)Math.PI : 0));
                chain.npcPhysChainOffset[1] = RedeHelper.PolarVector(-20 * NPC.spriteDirection, NPC.rotation) + RedeHelper.PolarVector(10 * NPC.spriteDirection, NPC.rotation - (float)Math.PI / 2 + (NPC.spriteDirection == 1 ? (float)Math.PI : 0));
                chain.npcPhysChainOffset[2] = RedeHelper.PolarVector(-32 * NPC.spriteDirection, NPC.rotation) + RedeHelper.PolarVector(16 * NPC.spriteDirection, NPC.rotation - (float)Math.PI / 2 + (NPC.spriteDirection == 1 ? (float)Math.PI : 0));
                chain.npcPhysChainDir[0] = -NPC.spriteDirection;
                chain.npcPhysChainDir[1] = -NPC.spriteDirection;
                chain.npcPhysChainDir[2] = -NPC.spriteDirection;
            }
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.DespawnHandler(3))
                return;

            if (Main.rand.NextBool(3))
            {
                ParticleManager.NewParticle(NPC.Center + new Vector2(2 + Main.rand.Next(26) * NPC.spriteDirection, -40 + Main.rand.Next(0, 6)), new Vector2(Main.rand.Next(-2, 3), -Main.rand.Next(0, 3)), new EmberParticle(), Color.White, .6f, 0, 2, Layer: Particle.Layer.BeforeNPCs);
            }
            if (Main.rand.NextBool(50))
            {
                ParticleManager.NewParticle(NPC.Center + new Vector2(2 + Main.rand.Next(26) * NPC.spriteDirection, -40 + Main.rand.Next(0, 6)), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f), Layer: Particle.Layer.BeforeNPCs);
                ParticleManager.NewParticle(NPC.Center + new Vector2(2 + Main.rand.Next(26) * NPC.spriteDirection, -48 + Main.rand.Next(0, 6)), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f));
            }
            if (Main.rand.NextBool(20))
            {
                ParticleManager.NewParticle(NPC.Center + new Vector2((-60 + Main.rand.Next(24)) * NPC.spriteDirection, -26 + Main.rand.Next(0, 30)), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f), Layer: Particle.Layer.BeforeNPCs);
                ParticleManager.NewParticle(NPC.Center + new Vector2((-60 + Main.rand.Next(24)) * NPC.spriteDirection, -26 + Main.rand.Next(0, 30)), new Vector2(Main.rand.Next(-1, 2), -Main.rand.Next(1, 3)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f));
            }
            if (NPC.life <= NPC.lifeMax / 2)
                attackSpeed = 4;
            if (AIState != ActionState.Firebreath || AITimer < 35)
                NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Idle:
                    float moveSpeed = 2.4f;
                    if (player.Center.DistanceSQ(NPC.Center) > 400 * 400)
                        moveSpeed = 3.6f;

                    if (NPC.DistanceSQ(player.Center) <= 160 * 160)
                        NPC.velocity.X *= .2f;
                    else
                    {
                        NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 28);
                        NPCHelper.HorizontallyMove(NPC, player.Center, .08f, moveSpeed, 18, 18, NPC.Center.Y > player.Center.Y, player);
                    }
                    if (AITimer++ >= TimerRand && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                        NPC.velocity.X = 0;
                        AIState = AttackType switch
                        {
                            1 => ActionState.Crow,
                            2 => ActionState.Fly,
                            3 => ActionState.Firebreath,
                            _ => ActionState.Gust,
                        };
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Gust:
                    switch (TimerRand)
                    {
                        default:
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            AniType = (int)AnimType.Gust;
                            TimerRand++;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (AITimer++ == 4 * attackSpeed)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                                for (int i = 0; i < 2; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Basan_HeatWave>(), NPC.damage, new Vector2((4 + i + TimerRand2) * NPC.spriteDirection, 0), SoundID.DD2_PhantomPhoenixShot with { Volume = 0.5f + (TimerRand2 / 12) }, i);
                            }
                            if (AniType is (int)AnimType.None)
                            {
                                TimerRand2 += NPC.life <= NPC.lifeMax / 2 ? 6 : 4;
                                TimerRand = 0;
                                AITimer = 0;
                                if (TimerRand2 >= (NPC.life <= NPC.lifeMax / 2 ? 24 : 12))
                                {
                                    TimerRand = Main.rand.Next(120, 161);
                                    TimerRand2 = 0;
                                    AIState = ActionState.Idle;
                                    AttackType++;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Crow:
                    switch (TimerRand)
                    {
                        default:
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            AniType = (int)AnimType.Crow;
                            TimerRand++;
                            break;
                        case 1:
                            if (AITimer++ == 0 && !Main.dedServ)
                                SoundEngine.PlaySound(new SoundStyle("Redemption/Sounds/Custom/ChickenCluck2") { Pitch = -.3f, Volume = 1.6f }, NPC.position);
                            int amount = NPC.life <= NPC.lifeMax / 2 ? 5 : 3;
                            if (AITimer >= 35 && AITimer % 3 == 0 && TimerRand2 < amount && NPC.CountNPCS(ModContent.NPCType<GhostfireChicken>()) < 8)
                            {
                                Vector2 spawnPos = NPC.Center + RedeHelper.PolarVector(Main.rand.Next(100, 201), RedeHelper.RandomRotation());
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<GhostfireChicken>());
                                TimerRand2++;
                            }
                            if (AniType is (int)AnimType.None)
                            {
                                TimerRand = Main.rand.Next(120, 161);
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                AttackType++;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Fly:
                    switch (TimerRand2)
                    {
                        case 0:
                            if (AITimer++ == 0)
                            {
                                NPC.velocity.X = 0;
                                NPC.velocity.Y = -10;
                            }
                            if (AITimer > 2 && NPC.velocity.Y >= -1)
                            {
                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                                TimerRand = player.RightOfDir(NPC);
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (AITimer++ < 30)
                                NPC.Move(player.Center + new Vector2(300 * -TimerRand, -Main.rand.Next(200, 251)), 10, 30);
                            else
                                NPC.Move(player.Center + new Vector2(300 * TimerRand, -Main.rand.Next(200, 251)), 10, 40);
                            if (AITimer >= 120 || (TimerRand == -1 ? NPC.Center.X <= player.Center.X - 300 : NPC.Center.X >= player.Center.X + 300))
                            {
                                AITimer = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            NPC.Move(player.Center + new Vector2(0, -40), 10, 30);
                            if (AITimer++ >= 30 && Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(NPC.position + new Vector2(0, 10), NPC.width, NPC.height - 10))
                            {
                                AttackType++;
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;
                                TimerRand = Main.rand.Next(120, 161);
                                AITimer = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Firebreath:
                    switch (TimerRand)
                    {
                        default:
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            AniType = (int)AnimType.Firebreath;
                            TimerRand++;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (AITimer == 35)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.position);
                                playerOld = player.Center;
                            }
                            if (AITimer++ >= 35 && AITimer % 2 == 0)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(28 * NPC.spriteDirection, -18), ModContent.ProjectileType<Basan_Firebreath>(), NPC.damage, RedeHelper.PolarVector(5, (playerOld - NPC.Center).ToRotation() + TimerRand2 - MathHelper.ToRadians(45)));
                                TimerRand2 += MathHelper.ToRadians(2);
                            }
                            if (AITimer >= 120)
                            {
                                AniType = (int)AnimType.Crow;
                                TimerRand++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 2:
                            if (AniType is (int)AnimType.None)
                            {
                                TimerRand = Main.rand.Next(180, 201);
                                TimerRand2 = 0;
                                AIState = ActionState.Idle;
                                AttackType = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
            #region Animations
            switch ((AnimType)AniType)
            {
                case AnimType.None:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.frame.X = 0;
                        NPC.rotation = 0;
                        if (NPC.velocity.X == 0)
                        {
                            if (NPC.frameCounter++ >= 10)
                            {
                                NPC.frameCounter = 0;
                                NPC.frame.Y += frameHeight;
                                if (NPC.frame.Y > 3 * frameHeight)
                                    NPC.frame.Y = 0;
                            }
                        }
                        else
                        {
                            if (NPC.frame.Y < 10 * frameHeight)
                                NPC.frame.Y = 10 * frameHeight;

                            NPC.frameCounter += NPC.velocity.X * 0.5f;
                            if (NPC.frameCounter is >= 5 or <= -5)
                            {
                                NPC.frameCounter = 0;
                                NPC.frame.Y += frameHeight;
                                if (NPC.frame.X == NPC.frame.Width)
                                {
                                    NPC.frame.X = 0;
                                    NPC.frame.Y = 10 * frameHeight;
                                }
                                if (NPC.frame.Y > 16 * frameHeight)
                                {
                                    NPC.frame.X = NPC.frame.Width;
                                    NPC.frame.Y = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        NPC.frame.X = 0;
                        NPC.rotation = NPC.velocity.X * 0.05f;
                        if (NPC.frame.Y < 4 * frameHeight)
                            NPC.frame.Y = 4 * frameHeight;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y == 7 * frameHeight)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, NPC.position);
                                for (int i = 0; i < 20; i++)
                                {
                                    ParticleManager.NewParticle(NPC.Center + new Vector2(-52 + Main.rand.Next(102), -4 + Main.rand.Next(34)), new Vector2(Main.rand.Next(-3, 4), -Main.rand.Next(0, 2)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f), Layer: Particle.Layer.BeforeNPCs);
                                    ParticleManager.NewParticle(NPC.Center + new Vector2(-52 + Main.rand.Next(102), -4 + Main.rand.Next(34)), new Vector2(Main.rand.Next(-3, 4), -Main.rand.Next(0, 2)), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f));
                                }
                            }

                            if (NPC.frame.Y > 9 * frameHeight)
                                NPC.frame.Y = 4 * frameHeight;
                        }
                    }
                    break;
                case AnimType.Gust:
                    NPC.rotation = 0;
                    NPC.frame.X = NPC.frame.Width;
                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;

                    if (NPC.frameCounter++ >= attackSpeed)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y == 5 * frameHeight)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                ParticleManager.NewParticle(NPC.Center + new Vector2(-52 + Main.rand.Next(102), -14 + Main.rand.Next(54)), new Vector2(Main.rand.Next(3, 7) * NPC.spriteDirection, 0), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f), Layer: Particle.Layer.BeforeNPCs);
                                ParticleManager.NewParticle(NPC.Center + new Vector2(-52 + Main.rand.Next(102), -14 + Main.rand.Next(54)), new Vector2(Main.rand.Next(3, 7) * NPC.spriteDirection, 0), new EmberParticle(), Color.White, Main.rand.NextFloat(.4f, 1f));
                            }
                        }
                        if (NPC.frame.Y > 7 * frameHeight)
                        {
                            AniType = (int)AnimType.None;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 0;
                        }
                    }
                    break;
                case AnimType.Crow:
                    NPC.rotation = 0;
                    NPC.frame.X = NPC.frame.Width;
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y = 8 * frameHeight;

                    if (NPC.frameCounter++ >= 7)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 16 * frameHeight)
                        {
                            FireFrameY = 0;
                            AniType = (int)AnimType.None;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 0;
                        }
                    }
                    break;
                case AnimType.Firebreath:
                    NPC.rotation = 0;
                    NPC.frame.X = NPC.frame.Width;
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y = 8 * frameHeight;

                    if (NPC.frameCounter++ >= 7)
                    {
                        NPC.frameCounter = 0;
                        FireFrameY++;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 14 * frameHeight)
                        {
                            FireFrameY = 7;
                            NPC.frame.Y = 13 * frameHeight;
                        }
                    }
                    break;
            }
            #endregion
        }
        private readonly int frameHeight = 116;
        private int attackSpeed = 7;
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frame.X = 0;
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (NPC.frameCounter++ >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
        }
        private int FireFrameY;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 pos = NPC.Center - new Vector2(0, 12);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(glowMask.Value, pos - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            if (AniType is (int)AnimType.Firebreath && NPC.frame.Y >= 8 * 116 && NPC.frame.Y <= 13 * 116)
            {
                int height = fireBreath.Value.Height / 6;
                int y = height * FireFrameY;
                Rectangle rect = new(0, y, fireBreath.Value.Width, height);
                Vector2 firePos = new(pos.X + (NPC.spriteDirection == -1 ? 32 : 52), pos.Y + 42);
                spriteBatch.Draw(fireBreath.Value, firePos - screenPos, new Rectangle?(rect), NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            return false;
        }
        public override bool PreKill()
        {
            if (FowlMorningWorld.FowlMorningActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                FowlMorningWorld.ChickPoints += 500;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BasanRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SpicyDrumstick>(), 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BasanTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CuckooCloak>()));
            npcLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<EggShield>(), ModContent.ItemType<GreneggLauncher>(), ModContent.ItemType<Halbirdhouse>(), ModContent.ItemType<NestWand>(), ModContent.ItemType<ChickendWand>(), ModContent.ItemType<DawnHerald>()));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>(), 1, 6, 8));
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (item.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), Main.rand.Next(6, 8));
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), Main.rand.Next(6, 8));
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life <= 0)
            {
                if (projectile.HasElement(ElementID.Fire))
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), Main.rand.Next(6, 8));
                else if (NPC.FindBuffIndex(BuffID.OnFire) != -1 || NPC.FindBuffIndex(BuffID.OnFire3) != -1)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<FriedChicken>(), Main.rand.Next(6, 8));
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Torch, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust2>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 10f;
                    dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust4>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 10f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Torch, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust2>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust4>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
        }
    }
    internal class BasanScarfPhys : IPhysChain
    {
        public Texture2D GetTexture(Mod mod) => ModContent.Request<Texture2D>("Redemption/NPCs/FowlMorning/Basan_TailFeather").Value;
        public Texture2D GetGlowmaskTexture(Mod mod) => ModContent.Request<Texture2D>("Redemption/NPCs/FowlMorning/Basan_TailFeather_Glow").Value;

        public int NumberOfSegments => 6;
        public int MaxFrames => 4;
        public int FrameCounterMax => 10;
        public bool Glow => false;
        public bool HasGlowmask => true;
        public int Shader => 0;
        public int GlowmaskShader => 0;

        public Color GetColor(PlayerDrawSet drawInfo, Color baseColour)
        {
            return baseColour;
        }

        public Vector2 AnchorOffset => new(-2, 0);

        public Vector2 OriginOffset(int index) //padding
        {
            return index switch
            {
                0 => new Vector2(0, 0),
                1 => new Vector2(-2, 0),
                2 => new Vector2(-4, 0),
                3 => new Vector2(-6, 0),
                4 => new Vector2(-8, 0),
                _ => new Vector2(-10, 0),
            };
        }

        public int Length(int index)
        {
            return index switch
            {
                _ => 12,
            };
        }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(NumberOfSegments, MaxFrames, NumberOfSegments - 1 - index, 0);
        }
        public Vector2 Force(Player player, int index, int dir, float gravDir, float time, NPC npc = null, Projectile proj = null)
        {
            Vector2 force = new(
                -dir * 0.5f,
                Player.defaultGravity * (0.5f + NumberOfSegments * NumberOfSegments * 0.5f / (1 + index))
                );

            if (!Main.gameMenu)
            {
                float windPower = 0.8f * dir * -10;

                // Wave in the wind
                force.X += 16f * -npc.spriteDirection;
                force.Y -= 14;
                if (index is 0)
                    force.Y -= 6;
                else if (index is 1)
                    force.Y -= 3;
                force -= npc.velocity * 2;
                force.Y += (float)(Math.Sin(time * 2f * windPower - index * Math.Sign(force.X)) * 0.15f * windPower) * 6f * dir;
            }
            return force;
        }
    }
}

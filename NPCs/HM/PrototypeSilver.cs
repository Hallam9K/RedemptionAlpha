using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Usable.Potions;
using Redemption.Projectiles.Hostile;
using Terraria.ModLoader.Utilities;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameContent.UI;
using System;
using Redemption.Items.Usable;
using System.IO;
using Terraria.Localization;

namespace Redemption.NPCs.HM
{
    public class PrototypeSilver : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert = 3,
            Laser,
            Grapple,
            Teleport
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
            // DisplayName.SetDefault("Prototype Silver Mk.I");
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 64;
            NPC.friendly = false;
            NPC.damage = 60;
            NPC.defense = 45;
            NPC.lifeMax = 550;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.aiStyle = -1;
            NPC.value = 600;
            NPC.knockBackResist = 0.1f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PrototypeSilverBanner>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
        private Vector2 moveTo;
        private int runCooldown;
        private float shieldAlpha;
        private bool shieldUp;
        private Vector2 p;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
            NPC.Shoot(NPC.Center, ModContent.ProjectileType<PrototypeSilver_Shield>(), 0, Vector2.Zero, CustomSounds.ShieldActivate, NPC.whoAmI);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            if (NPC.ai[3] == 1)
                shieldAlpha += 0.01f;
            else
                shieldAlpha -= 0.01f;
            shieldAlpha = MathHelper.Clamp(shieldAlpha, 0, 0.2f);

            NPC.TargetClosest();
            NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 120);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.8f, 8, 16, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    else
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC others = Main.npc[i];
                            if (!others.active || others.whoAmI == NPC.whoAmI || others.ai[0] >= 3)
                                continue;

                            if (others.type != Type && others.type != ModContent.NPCType<Android>() && others.type != ModContent.NPCType<SpacePaladin>())
                                continue;

                            if (NPC.DistanceSQ(others.Center) >= 900 * 900)
                                continue;

                            others.GetGlobalNPC<RedeNPC>().attacker = globalNPC.attacker;
                            others.ai[1] = 0;
                            others.ai[0] = 3;
                        }
                    }
                    if (!shieldUp && NPC.ai[3] == 0 && NPC.life <= NPC.lifeMax / 2)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<PrototypeSilver_Shield>(), 0, Vector2.Zero, CustomSounds.ShieldActivate, NPC.whoAmI);
                        shieldUp = true;
                    }
                    if (NPC.life <= NPC.lifeMax / 5 && player.Redemption().slayerStarRating <= 3)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Teleport;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 900, true, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.DamageHostileAttackers(0, 8);
                    if (Main.rand.NextBool(100) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 80 * 80)
                    {
                        if (globalNPC.attacker is not Player || (globalNPC.attacker is Player && globalNPC.attacker.Center.Y < NPC.Center.Y - 60))
                        {
                            NPC.LookAtEntity(globalNPC.attacker);
                            AITimer = 0;
                            NPC.frameCounter = 0;
                            NPC.velocity.X = 0;
                            AIState = ActionState.Grapple;
                            NPC.netUpdate = true;
                        }
                    }
                    if (Main.rand.NextBool(100) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 60 * 60)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.Laser;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.15f, 2f, 8, 16, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Laser:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    Vector2 originPos = NPC.Center + new Vector2(8 * NPC.spriteDirection, -21);
                    if (++AITimer < 60)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 20);
                            vector.Y = (float)(Math.Cos(angle) * 20);
                            Dust dust2 = Main.dust[Dust.NewDust(originPos + vector, 2, 2, DustID.Frost)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(originPos) * 2f;
                        }
                    }
                    if (AITimer == 51 || AITimer == 61 || AITimer == 71)
                        p = globalNPC.attacker.Center;
                    if (AITimer == 60 || AITimer == 70 || AITimer == 80)
                    {
                        NPC.Shoot(originPos + new Vector2(-2 * NPC.spriteDirection, 4), ModContent.ProjectileType<PrototypeSilver_Beam>(), NPC.damage, RedeHelper.PolarVector(2, (p - originPos).ToRotation()), CustomSounds.Zap2 with { Pitch = 0.2f, Volume = 0.6f }, NPC.whoAmI);
                        NPC.velocity.X -= 1 * NPC.spriteDirection;
                    }
                    if (AITimer >= 100)
                    {
                        AITimer = 0;
                        AIState = ActionState.Alert;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Grapple:
                    if (AITimer >= 100 && NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AITimer == 0)
                    {
                        Vector2 originPos2 = NPC.Center + new Vector2(-11 * NPC.spriteDirection, -9);
                        NPC.Shoot(originPos2, ModContent.ProjectileType<PrototypeSilver_Hook>(), NPC.damage, Vector2.Zero, CustomSounds.Launch2 with { Volume = 0.6f }, NPC.whoAmI);
                        AITimer = 1;
                    }
                    if (AITimer >= 100)
                    {
                        NPC.frame.Y = 0;
                        AITimer = 0;
                        AIState = ActionState.Alert;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Teleport:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    if (NPC.life > NPC.lifeMax / 5)
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    AITimer++;
                    if (AITimer >= 20)
                    {
                        if (AITimer % 3 == 0)
                        {
                            int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y - 800), NPC.width, NPC.height + 750, DustID.Frost);
                            Main.dust[dust].noGravity = true;
                        }
                    }
                    if (AITimer++ >= 180)
                    {
                        SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, NPC.position);
                        DustHelper.DrawDustImage(NPC.Center, DustID.Frost, 0.12f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                        for (int i = 0; i < 15; i++)
                        {
                            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.SpreadUp(1), new LightningParticle(), Color.White, 3, 0, 2);
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                            Main.dust[dust].velocity *= 6f;
                            Main.dust[dust].noGravity = true;
                        }
                        NPC.netUpdate = true;
                        if (player.Redemption().slayerStarRating <= 3 && !NPC.AnyNPCs(ModContent.NPCType<SlayerSpawner>()))
                        {
                            player.Redemption().slayerStarRating++;
                            NPC.SetDefaults(ModContent.NPCType<SlayerSpawner>());
                        }
                        else
                            NPC.active = false;
                    }
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Grapple)
            {
                NPC.rotation = 0;
                NPC.frame.Y = 12 * frameHeight;
                return;
            }
            else if (AIState is ActionState.Laser)
            {
                NPC.rotation = 0;
                NPC.frame.Y = 0;
                return;
            }

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 7)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 4 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 2 or <= -2)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 11 * frameHeight)
                            NPC.frame.Y = 4 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            if (player.Redemption().slayerStarRating > 0)
            {
                if (NPC.Sight(player, 900, false, true, true))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC others = Main.npc[i];
                        if (!others.active || others.whoAmI == NPC.whoAmI || others.ai[0] >= 3)
                            continue;

                        if (others.type != Type && others.type != ModContent.NPCType<Android>() && others.type != ModContent.NPCType<SpacePaladin>())
                            continue;

                        if (NPC.DistanceSQ(others.Center) >= 900 * 900)
                            continue;

                        others.GetGlobalNPC<RedeNPC>().attacker = player;
                        others.ai[1] = 0;
                        others.ai[0] = 3;
                    }
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
            }
        }
        public static float c = 1f / 255f;
        public Color innerColor = new(100 * c * 0.5f, 242 * c * 0.5f, 170 * c * 0.5f, 0.5f);
        public Color borderColor = new(0 * c, 242 * c, 170 * c, 1f);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (shieldAlpha <= 0)
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(glow, NPC.Center + new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            else
            {
                Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f * shieldAlpha).ToVector4());
                ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, shieldAlpha).ToVector4());

                spriteBatch.End();
                ShieldEffect.Parameters["sinMult"].SetValue(10f);
                ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2f / HexagonTexture.Width, TextureAssets.Npc[NPC.type].Value.Height / 13 / HexagonTexture.Height));
                ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (TextureAssets.Npc[NPC.type].Value.Width / 2), 1f / (TextureAssets.Npc[NPC.type].Value.Height / 2)));
                ShieldEffect.Parameters["frameAmount"].SetValue(13f);
                spriteBatch.BeginDefault(true);
                ShieldEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(glow, NPC.Center + new Vector2(0, 3) - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }

        public override bool CanHitNPC(NPC target) => AIState == ActionState.Alert;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Alert;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonMyofibre>(), 2, 4, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plating>(), 3, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Capacitor>(), 3, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AIChip>(), 6, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnergyCell>(), 10));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<P0T4T0>(), 150));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 16; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
                    Main.dust[dustIndex].velocity *= 5;
                }
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/PrototypeSilverGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDay.Chance * (Main.hardMode && spawnInfo.Player.Redemption().slayerStarRating > 1 ? 0.04f : 0f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 25);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.PrototypeSilver"))
            });
        }
    }
}

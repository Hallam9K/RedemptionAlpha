using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Materials.PreHM;
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
using Redemption.Buffs.NPCBuffs;
using Terraria.Utilities;
using Terraria.UI;
using Redemption.Base;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Projectiles.Hostile;
using Terraria.ModLoader.Utilities;
using Redemption.UI;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameContent.UI;
using Redemption.Projectiles.Ranged;
using System;
using System.Collections.Generic;

namespace Redemption.NPCs.HM
{
    public class SpacePaladin : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert = 3,
            Laser,
            Slash,
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
            DisplayName.SetDefault("Space Paladin Mk.I");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 92;
            NPC.friendly = false;
            NPC.damage = 80;
            NPC.defense = 50;
            NPC.lifeMax = 3450;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.aiStyle = -1;
            NPC.value = 9000;
            NPC.knockBackResist = 0.001f; // TODO: Space Paladin Banner
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<HazmatZombieBanner>();
        }

        private Vector2 moveTo;
        private int runCooldown;
        private float shieldAlpha;
        private bool shieldUp;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 120);
        }
        private readonly List<int> projBlocked = new();
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();

            NPC.TargetClosest();
            if (AIState is not ActionState.Alert)
                NPC.LookByVelocity();

            if (AIState is ActionState.Alert)
            {
                Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 62 : NPC.Center.X), (int)(NPC.Center.Y - 54), 62, 92);
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile proj = Main.projectile[p];
                    if (!proj.active || proj.friendly || (NPC.Center.X > proj.Center.X && NPC.spriteDirection == 1) || (NPC.Center.X < proj.Center.X && NPC.spriteDirection == -1))
                        continue;

                    if (proj.Hitbox.Intersects(ShieldHitbox))
                    {
                        if (!projBlocked.Contains(proj.whoAmI))
                            projBlocked.Add(proj.whoAmI);
                    }
                }
            }

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
                    }

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.6f, 28, 36, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    else
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC others = Main.npc[i];
                            if (!others.active || others.whoAmI == NPC.whoAmI || others.ai[0] >= 3)
                                continue;

                            if (others.type != Type && others.type != ModContent.NPCType<Android>() && others.type != ModContent.NPCType<PrototypeSilver>())
                                continue;

                            if (NPC.DistanceSQ(others.Center) >= 900 * 900)
                                continue;

                            others.GetGlobalNPC<RedeNPC>().attacker = globalNPC.attacker;
                            others.ai[1] = 0;
                            others.ai[0] = 3;
                        }
                    }
                    if (NPC.life <= NPC.lifeMax / 5 && player.Redemption().slayerStarRating <= 4)
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

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.15f, 1.6f, 28, 36, NPC.Center.Y > globalNPC.attacker.Center.Y);
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
                        DustHelper.DrawDustImage(NPC.Center, DustID.Frost, 0.2f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                        for (int i = 0; i < 25; i++)
                        {
                            ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(NPC), RedeHelper.SpreadUp(1), new LightningParticle(), Color.White, 4);
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                            Main.dust[dust].velocity *= 6f;
                            Main.dust[dust].noGravity = true;
                        }
                        NPC.netUpdate = true;
                        if (player.Redemption().slayerStarRating <= 4 && !NPC.AnyNPCs(ModContent.NPCType<SlayerSpawner>()))
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
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (NPC.collideY || NPC.velocity.Y == 0)
                {
                    NPC.rotation = 0;
                    if (NPC.velocity.X == 0)
                        NPC.frame.Y = 0;
                    else
                    {
                        if (NPC.frame.Y < 1 * frameHeight)
                            NPC.frame.Y = 1 * frameHeight;

                        NPC.frameCounter += NPC.velocity.X * 0.5f;
                        if (NPC.frameCounter is >= 3 or <= -3)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 11 * frameHeight)
                                NPC.frame.Y = 2 * frameHeight;
                        }
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.frame.Y = 10 * frameHeight;
                }
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

                        if (others.type != Type && others.type != ModContent.NPCType<Android>() && others.type != ModContent.NPCType<PrototypeSilver>())
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
                }
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (AIState is not ActionState.Alert)
                return;
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 62 : NPC.Center.X), (int)(NPC.Center.Y - 54), 62, 92);
            if (item.Hitbox.Intersects(ShieldHitbox) && (NPC.Center.X > player.Center.X && NPC.spriteDirection == -1 || NPC.Center.X < player.Center.X && NPC.spriteDirection == 1))
            {
                SoundEngine.PlaySound(SoundID.NPCHit34, NPC.position);
                damage = 0;
                knockback = 0;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (AIState is not ActionState.Alert)
                return;
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 62 : NPC.Center.X), (int)(NPC.Center.Y - 54), 62, 92);
            if (!projBlocked.Contains(projectile.whoAmI) && (!projectile.active || (NPC.Center.X > projectile.Center.X && NPC.spriteDirection == 1) || (NPC.Center.X < projectile.Center.X && NPC.spriteDirection == -1)))
                return;

            if (projectile.Hitbox.Intersects(ShieldHitbox))
            {
                projBlocked.Remove(projectile.whoAmI);
                if (projectile.penetrate != -1)
                    projectile.Kill();
                SoundEngine.PlaySound(SoundID.NPCHit34, NPC.position);
                damage = 0;
                knockback = 0;
            }
        }
        public static float c = 1f / 255f;
        public Color innerColor = new(100 * c * 0.5f, 242 * c * 0.5f, 170 * c * 0.5f, 0.5f);
        public Color borderColor = new(0 * c, 242 * c, 170 * c, 1f);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D upper = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Upper_Calm").Value;
            Texture2D upperGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Upper_Calm_Glow").Value;
            Texture2D upperA = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Upper_Angry").Value;
            Texture2D upperAGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Upper_Angry_Glow").Value;
            Texture2D shieldBack = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Shield_B").Value;
            Texture2D shieldFront = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Shield_F").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center + new Vector2(0, 3);

            if (AIState is not ActionState.Alert)
                spriteBatch.Draw(shieldBack, pos - screenPos, null, NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 26 : -18, 6), NPC.scale, effects, 0);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, pos - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (AIState is not ActionState.Alert)
            {
                int UpperHeight = upper.Height / 12;
                int UpperY = UpperHeight * NPC.frame.Y / 94;
                Rectangle UpperRect = new(0, UpperY, upper.Width, UpperHeight);
                spriteBatch.Draw(upper, pos - screenPos, UpperRect, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 4 : 10, 24), NPC.scale, effects, 0);
                spriteBatch.Draw(upperGlow, pos - screenPos, UpperRect, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 4 : 10, 24), NPC.scale, effects, 0);
            }
            if (AIState is ActionState.Alert)
            {
                int UpperAHeight = upperA.Height / 12;
                int UpperAY = UpperAHeight * NPC.frame.Y / 94;
                Rectangle UpperARect = new(0, UpperAY, upperA.Width, UpperAHeight);
                spriteBatch.Draw(upperA, pos - screenPos, UpperARect, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 8 : 16, 22), NPC.scale, effects, 0);
                spriteBatch.Draw(upperAGlow, pos - screenPos, UpperARect, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 8 : 16, 22), NPC.scale, effects, 0);

                Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f * 0.6f).ToVector4());
                ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, 0.6f).ToVector4());

                spriteBatch.End();
                ShieldEffect.Parameters["sinMult"].SetValue(10f);
                ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(shieldFront.Width / 2f / HexagonTexture.Width, shieldFront.Height / 2f / HexagonTexture.Height));
                ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (shieldFront.Width / 2), 1f / (shieldFront.Height / 2)));
                ShieldEffect.Parameters["frameAmount"].SetValue(1f);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                ShieldEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(shieldFront, pos - screenPos, null, NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 30 : -24, 10), NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonMyofibre>(), 1, 8, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plating>(), 1, 4, 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Capacitator>(), 2, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AIChip>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<P0T4T0>(), 150));
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 26; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
                    Main.dust[dustIndex].velocity *= 5;
                }
                for (int i = 0; i < 7; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SpacePaladinGore" + (i + 1)).Type, 1);
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
            return SpawnCondition.OverworldDay.Chance * (Main.hardMode && spawnInfo.Player.Redemption().slayerStarRating > 2 ? 0.01f : 0f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement(
                    "The 4th Slayer Unit constructed, after Space Keeper. This unit uses newer shield technology compared to King Slayer's Prototype Multium vessel, and is capable of absorbing far greater impacts; in addition to the thick durable plating and cyber blade, this robot was built for war."),
            });
        }
    }
}
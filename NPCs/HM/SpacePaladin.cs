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
using Redemption.Base;
using Redemption.Projectiles.Hostile;
using Terraria.ModLoader.Utilities;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameContent.UI;
using System;
using System.Collections.Generic;
using Redemption.Items.Usable;
using ReLogic.Content;
using Terraria.Localization;

namespace Redemption.NPCs.HM
{
    public class SpacePaladin : ModNPC
    {
        private static Asset<Texture2D> glow;
        private static Asset<Texture2D> upper;
        private static Asset<Texture2D> upperGlow;
        private static Asset<Texture2D> upperA;
        private static Asset<Texture2D> upperAGlow;
        private static Asset<Texture2D> shieldBack;
        private static Asset<Texture2D> shieldFront;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            upper = ModContent.Request<Texture2D>(Texture + "_Upper_Calm");
            upperGlow = ModContent.Request<Texture2D>(Texture + "_Upper_Calm_Glow");
            upperA = ModContent.Request<Texture2D>(Texture + "_Upper_Angry");
            upperAGlow = ModContent.Request<Texture2D>(Texture + "_Upper_Angry_Glow");
            shieldBack = ModContent.Request<Texture2D>(Texture + "_Shield_B");
            shieldFront = ModContent.Request<Texture2D>(Texture + "_Shield_F");
        }
        public override void Unload()
        {
            glow = null;
            upper = null;
            upperGlow = null;
            upperA = null;
            upperAGlow = null;
            shieldBack = null;
            shieldFront = null;
        }
        public enum ActionState
        {
            Idle,
            Wander,
            Alert = 3,
            Laser,
            Slash,
            Stomp,
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
            // DisplayName.SetDefault("Space Paladin Mk.I");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;


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
            NPC.knockBackResist = 0.001f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SpacePaladinBanner>();
        }

        private Vector2 moveTo;
        private int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
        }
        private readonly List<int> projBlocked = new();
        private Vector2 p;
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
                    if (!proj.active || proj.friendly || (NPC.RightOf(proj) && NPC.spriteDirection == 1) || (proj.RightOf(NPC) && NPC.spriteDirection == -1))
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

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 30, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.6f, 28, 36, NPC.Center.Y > moveTo.Y * 16);
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

                    if (Main.rand.NextBool(150) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 80 * 80)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.Laser;
                        NPC.netUpdate = true;
                    }
                    if (Main.rand.NextBool(150) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 60 * 60)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        TimerRand = 0;
                        AITimer = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.Stomp;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 30, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.15f, 1.6f, 28, 36, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
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

                    Vector2 originPos = NPC.Center + new Vector2(8 * NPC.spriteDirection, -50);
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
                    if (AITimer == 56 || AITimer == 61 || AITimer == 66 || AITimer == 71)
                        p = globalNPC.attacker.Center;
                    if (AITimer == 60 || AITimer == 65 || AITimer == 70 || AITimer == 75)
                    {
                        NPC.Shoot(originPos + new Vector2(-2 * NPC.spriteDirection, 4), ModContent.ProjectileType<PrototypeSilver_Beam>(), NPC.damage, RedeHelper.PolarVector(2, (p - originPos).ToRotation()), CustomSounds.Zap2 with { Pitch = 0.2f, Volume = 0.6f }, NPC.whoAmI);
                        NPC.velocity.X -= 1 * NPC.spriteDirection;
                    }
                    if (AITimer >= 90)
                    {
                        AITimer = 0;
                        AIState = ActionState.Alert;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Stomp:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    switch (TimerRand)
                    {
                        case 0:
                            NPC.knockBackResist = 0f;
                            if (AITimer++ == 20)
                            {
                                NPC.LookAtEntity(globalNPC.attacker);
                                NPC.velocity.Y = -Main.rand.Next(17, 21);
                                NPC.velocity.X = Main.rand.Next(3, 8) * NPC.spriteDirection;
                            }
                            if (AITimer > 20)
                                NPC.rotation += 0.1f * NPC.spriteDirection;

                            if (AITimer > 20 && NPC.velocity.Y >= 0)
                            {
                                NPC.noGravity = true;
                                AITimer = 0;
                                NPC.velocity.Y = 24;
                                NPC.velocity.X = 0;
                                TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (BaseAI.HitTileOnSide(NPC, 3) || NPC.velocity.Y == 0)
                            {
                                NPC.noGravity = false;

                                for (int i = 0; i < 40; i++)
                                    Dust.NewDust(NPC.BottomLeft, Main.rand.Next(NPC.width), 1, DustID.Smoke, 0, -7);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom, NPC.position);
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 20;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 42),
                                            RedeHelper.PolarVector(8, MathHelper.ToRadians(45) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage(NPC.damage), 0, Main.myPlayer);
                                        Main.projectile[proj].tileCollide = false;
                                        Main.projectile[proj].timeLeft = 200;
                                        Main.projectile[proj].netUpdate = true;
                                    }
                                    for (int i = 0; i < 18; i++)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 42),
                                            RedeHelper.PolarVector(7, MathHelper.ToRadians(20) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage(NPC.damage), 0, Main.myPlayer);
                                        Main.projectile[proj].tileCollide = false;
                                        Main.projectile[proj].timeLeft = 200;
                                        Main.projectile[proj].netUpdate = true;
                                    }
                                }
                                slamOrigin = NPC.Center;
                                slam = true;

                                AITimer = 0;
                                TimerRand = 2;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.rotation.SlowRotation(0, (float)Math.PI / 10);
                            }
                            break;
                        case 2:
                            if (AITimer++ >= 20)
                            {
                                NPC.knockBackResist = 0.001f;
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = ActionState.Alert;
                                NPC.netUpdate = true;
                            }
                            break;
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
                        DustHelper.DrawDustImage(NPC.Center, DustID.Frost, 0.2f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                        for (int i = 0; i < 25; i++)
                        {
                            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.SpreadUp(1), new LightningParticle(), Color.White, 4, 0, 2);
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
            if (slam)
                SlamShockActive();
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
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
                if (AIState is not ActionState.Stomp && TimerRand <= 1)
                    NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 10 * frameHeight;
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
                    NPC.netUpdate = true;
                }
            }
        }
        private bool slam;
        private int slamTimer;
        private Vector2 slamOrigin;
        public void SlamShockActive()
        {
            if (slamTimer++ % 1 == 0)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 origin = slamOrigin;
                    origin.X += slamTimer * 32 * i;
                    int numtries = 0;
                    int x = (int)(origin.X / 16);
                    int y = (int)(origin.Y / 16);
                    while (WorldGen.InWorld(x, y) && Framing.GetTileSafely(x, y) != null && !WorldGen.SolidTile2(x, y) && Framing.GetTileSafely(x - 1, y) != null && !WorldGen.SolidTile2(x - 1, y) && Framing.GetTileSafely(x + 1, y) != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        origin.Y = y * 16;
                    }
                    while (WorldGen.InWorld(x, y) && (WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        origin.Y = y * 16;
                    }
                    if (numtries >= 20)
                        break;

                    NPC.Shoot(origin - new Vector2(0, 8), ModContent.ProjectileType<SpacePaladin_GroundShock>(), NPC.damage, Vector2.Zero, SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.2f });
                }
            }
            if (slamTimer >= 60)
            {
                slam = false;
                slamTimer = 0;
                NPC.netUpdate = true;
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (AIState is not ActionState.Alert && AIState is not ActionState.Laser)
                return;
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 62 : NPC.Center.X), (int)(NPC.Center.Y - 54), 62, 92);
            if (item.noMelee || item.damage <= 0 || (NPC.RightOf(player) && NPC.spriteDirection == 1) || (player.RightOf(NPC) && NPC.spriteDirection == -1))
                return;

            if (player.Redemption().meleeHitbox.Intersects(ShieldHitbox))
            {
                SoundEngine.PlaySound(SoundID.NPCHit34, NPC.position);
                modifiers.SetMaxDamage(1);
                modifiers.Knockback *= 0;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (AIState is not ActionState.Alert && AIState is not ActionState.Laser)
                return;
            Rectangle ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 62 : NPC.Center.X), (int)(NPC.Center.Y - 54), 62, 92);
            if (!projBlocked.Contains(projectile.whoAmI) && (!projectile.active || (NPC.RightOf(projectile) && NPC.spriteDirection == 1) || (projectile.RightOf(NPC) && NPC.spriteDirection == -1)))
                return;

            Rectangle projectileHitbox = projectile.Hitbox;
            if (projectile.Redemption().swordHitbox != default)
                projectileHitbox = projectile.Redemption().swordHitbox;
            if (projectile.Colliding(projectileHitbox, ShieldHitbox))
            {
                projBlocked.Remove(projectile.whoAmI);
                if (projectile.penetrate > 1)
                    projectile.timeLeft = Math.Min(projectile.timeLeft, 2);
                SoundEngine.PlaySound(SoundID.NPCHit34, NPC.position);
                modifiers.SetMaxDamage(1);
                modifiers.Knockback *= 0;
            }
        }
        public static float c = 1f / 255f;
        public Color innerColor = new(100 * c * 0.5f, 242 * c * 0.5f, 170 * c * 0.5f, 0.5f);
        public Color borderColor = new(0 * c, 242 * c, 170 * c, 1f);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center + new Vector2(0, 3);

            if (AIState is not ActionState.Alert && AIState is not ActionState.Laser)
                spriteBatch.Draw(shieldBack.Value, pos - screenPos, null, NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 26 : -18, 6), NPC.scale, effects, 0);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glow.Value, pos - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (AIState is not ActionState.Alert && AIState is not ActionState.Laser)
            {
                int UpperHeight = upper.Value.Height / 12;
                int UpperY = UpperHeight * NPC.frame.Y / 94;
                Rectangle UpperRect = new(0, UpperY, upper.Value.Width, UpperHeight);
                spriteBatch.Draw(upper.Value, pos - screenPos, UpperRect, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 4 : 10, 24), NPC.scale, effects, 0);
                spriteBatch.Draw(upperGlow.Value, pos - screenPos, UpperRect, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 4 : 10, 24), NPC.scale, effects, 0);
            }
            if (AIState is ActionState.Alert || AIState is ActionState.Laser)
            {
                int UpperAHeight = upperA.Value.Height / 12;
                int UpperAY = UpperAHeight * NPC.frame.Y / 94;
                Rectangle UpperARect = new(0, UpperAY, upperA.Value.Width, UpperAHeight);
                spriteBatch.Draw(upperA.Value, pos - screenPos, UpperARect, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 8 : 16, 22), NPC.scale, effects, 0);
                spriteBatch.Draw(upperAGlow.Value, pos - screenPos, UpperARect, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 8 : 16, 22), NPC.scale, effects, 0);

                Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
                ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
                ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
                ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f * 0.6f).ToVector4());
                ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, 0.6f).ToVector4());

                spriteBatch.End();
                ShieldEffect.Parameters["sinMult"].SetValue(10f);
                ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(shieldFront.Value.Width / 2f / HexagonTexture.Width, shieldFront.Value.Height / 2f / HexagonTexture.Height));
                ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (shieldFront.Value.Width / 2), 1f / (shieldFront.Value.Height / 2)));
                ShieldEffect.Parameters["frameAmount"].SetValue(1f);
                spriteBatch.BeginDefault(true);
                ShieldEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(shieldFront.Value, pos - screenPos, null, NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? 30 : -24, 10), NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Stomp && NPC.velocity.Length() > 0;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonMyofibre>(), 1, 8, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plating>(), 1, 4, 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Capacitor>(), 2, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AIChip>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnergyCell>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<P0T4T0>(), 150));
        }
        public override void HitEffect(NPC.HitInfo hit)
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
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 5);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SpacePaladin"))
            });
        }
    }
}

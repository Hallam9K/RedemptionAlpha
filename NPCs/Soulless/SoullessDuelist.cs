using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Redemption.BaseExtension;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.NPCs.PostML;
using Redemption.Items.Materials.PostML;
using Redemption.Biomes;
using Redemption.Projectiles.Hostile;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.DataStructures;
using System;

namespace Redemption.NPCs.Soulless
{
    public class SoullessDuelist : SoullessBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Slash
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public static void UnloadChain()
        {
            Tendril1 = null;
            Tendril2 = null;
            Tendril3 = null;
        }
        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 90;
            NPC.friendly = false;
            NPC.defense = 12;
            NPC.lifeMax = 2950;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            NPC.value = 5400;
            NPC.knockBackResist = 0.4f;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
            // TODO: Banner for soulless wanderer
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();

            NPC.GetGlobalNPC<NPCPhysChain>().glowChain = true;
            Tendril1 = new LightTendrilScarfPhys();
            Tendril2 = new LightTendrilScarfPhys();
            Tendril3 = new LightTendrilScarfPhys();
        }
        private static IPhysChain Tendril1;
        private static IPhysChain Tendril2;
        private static IPhysChain Tendril3;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                    ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(4), new SoulParticle(), Color.White, 1);

                for (int i = 0; i < 40; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dustIndex2].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>());

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        private int dodgeCooldown;
        private bool powerUp;
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.alpha = 0;
        }
        public override void AI()
        {
            if (HasEyes)
            {
                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChain[0] = Tendril1;
                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChain[1] = Tendril2;
                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChain[2] = Tendril3;

                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChainDir[0] = -NPC.spriteDirection;
                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChainDir[1] = -NPC.spriteDirection;
                NPC.GetGlobalNPC<NPCPhysChain>().npcPhysChainDir[2] = -NPC.spriteDirection;

                NPCPhysChain chains = NPC.GetGlobalNPC<NPCPhysChain>();
                NPCPhysChain.ModifyChainPhysics(NPC, Tendril1, ref chains.bodyPhysChainPositions[0], NPCChainHelper.GetNPCDrawAnchor(chains.npcPhysChainOffset[0], NPC), new Vector2(-5, -16f));
                NPCPhysChain.ModifyChainPhysics(NPC, Tendril1, ref chains.bodyPhysChainPositions[1], NPCChainHelper.GetNPCDrawAnchor(chains.npcPhysChainOffset[1], NPC), new Vector2(0, 0));
                NPCPhysChain.ModifyChainPhysics(NPC, Tendril1, ref chains.bodyPhysChainPositions[2], NPCChainHelper.GetNPCDrawAnchor(chains.npcPhysChainOffset[2], NPC), new Vector2(-9, 16f));
            }

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Slash)
                NPC.LookByVelocity();

            Rectangle SlashHitbox1 = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 52 : NPC.Center.X), (int)(NPC.Center.Y - 7), 52, 22);
            Rectangle SlashHitbox2 = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 66 : NPC.Center.X), (int)(NPC.Center.Y - 27), 66, 42);
            dodgeCooldown--;
            dodgeCooldown = (int)MathHelper.Max(0, dodgeCooldown);

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
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, interactDoorStyle: 2);

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 12, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Alert:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, interactDoorStyle: 2);

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 90, false, true))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Slash;
                        if (HasEyes && Main.rand.NextBool(2))
                            powerUp = true;
                    }

                    if (dodgeCooldown <= 0 && NPC.velocity.Y == 0)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (!proj.active || !proj.friendly || proj.damage <= 0 || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 90 + (proj.velocity.Length() * 4), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<VoidFlame>());
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            NPC.Dodge(proj, 6, 2, 12);
                            dodgeCooldown = 60;
                        }
                    }

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 3f * SpeedMultiplier, 12, 12, NPC.Center.Y > globalNPC.attacker.Center.Y);
                    break;

                case ActionState.Slash:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AniFrameY == 4 && AITimer++ < (powerUp ? 30 : 10))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.frameCounter = 0;
                    }
                    if ((AniFrameY == 6 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox1)) || (AniFrameY == 10 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox2)))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * 0.2f) : NPC.damage;
                        if (globalNPC.attacker is NPC && (globalNPC.attacker as NPC).immune[NPC.whoAmI] <= 0)
                        {
                            (globalNPC.attacker as NPC).immune[NPC.whoAmI] = 10;
                            int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                            BaseAI.DamageNPC(globalNPC.attacker as NPC, damage, 6, hitDirection, NPC);
                        }
                        else if (globalNPC.attacker is Player)
                        {
                            int hitDirection = NPC.Center.X > globalNPC.attacker.Center.X ? -1 : 1;
                            BaseAI.DamagePlayer(globalNPC.attacker as Player, (int)(damage * 1.2f), 6, hitDirection, NPC);
                        }
                    }
                    break;
            }
            if (HasEyes && Main.rand.NextBool(18))
            {
                int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, Scale: 2);
                Main.dust[dust].velocity.Y = -2;
                Main.dust[dust].velocity.X = 0;
                Main.dust[dust].noGravity = true;

            }
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                HeadY = MaskType switch
                {
                    MaskState.Angry => 1,
                    MaskState.Happy => 2,
                    _ => 0,
                };

                if (AIState is ActionState.Slash)
                {
                    if (++NPC.frameCounter >= 3)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY is 6 or 10)
                        {
                            SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                            NPC.velocity.X = 4 * NPC.spriteDirection;
                            if (powerUp)
                            {
                                int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * 0.2f) : NPC.damage;
                                if (AniFrameY is 6)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoullessDuelist_Slash_Proj>(), damage, new Vector2(20 * NPC.spriteDirection, 0), false, SoundID.DD2_PhantomPhoenixShot, "", NPC.whoAmI);
                                if (AniFrameY is 10)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SoullessDuelist_Slash_Proj2>(), (int)(damage * 1.2f), new Vector2(30 * NPC.spriteDirection, 0), false, SoundID.DD2_PhantomPhoenixShot, "", NPC.whoAmI);
                            }
                        }
                        if (AniFrameY > 14)
                        {
                            AniFrameY = 0;
                            NPC.frame.Y = 0;
                            AITimer = 0;

                            RedeNPC globalNPC = NPC.Redemption();
                            if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 90, false, true))
                            {
                                NPC.LookAtEntity(globalNPC.attacker);
                                if (HasEyes && Main.rand.NextBool(2))
                                    powerUp = true;
                                else
                                    powerUp = false;
                            }
                            else
                                AIState = ActionState.Alert;
                        }
                    }
                    if (AniFrameY is 6 or 7)
                        HeadX = 2;
                    else if (AniFrameY is 5 or 14)
                        HeadX = 0;
                    else
                        HeadX = 1;

                    HeadOffsetY = SetHeadOffsetY(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;
                }
                AniFrameY = 0;
                powerUp = false;

                if (NPC.collideY || NPC.velocity.Y == 0)
                {
                    NPC.rotation = 0;
                    if (NPC.velocity.X == 0)
                        NPC.frame.Y = 0;
                    else
                    {
                        NPC.frameCounter += NPC.velocity.X * 0.5f;
                        if (NPC.frameCounter is >= 5 or <= -5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 3 * frameHeight)
                                NPC.frame.Y = 0 * frameHeight;
                        }
                    }
                    HeadX = 0;
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.frame.Y = 4 * frameHeight;
                    HeadX = 1;
                }
            }
            HeadOffsetY = SetHeadOffsetY(ref frameHeight);
            HeadOffsetX = SetHeadOffsetX(ref frameHeight);
        }
        public override int SetHeadOffsetY(ref int frameHeight)
        {
            if (AIState is ActionState.Slash)
            {
                return AniFrameY switch
                {
                    1 => 2,
                    2 => 4,
                    3 => 6,
                    4 => 6,
                    5 => 6,
                    6 => 10,
                    7 => 6,
                    8 => 2,
                    10 => 10,
                    11 => 2,
                    12 => 2,
                    _ => 0,
                };
            }
            else
            {
                return (NPC.frame.Y / frameHeight) switch
                {
                    1 => -2,
                    2 => -2,
                    4 => 8,
                    _ => 0,
                };
            }
        }
        public override int SetHeadOffsetX(ref int frameHeight)
        {
            if (AIState is ActionState.Slash)
            {
                return AniFrameY switch
                {
                    1 => -2,
                    2 => -2,
                    3 => -2,
                    4 => -2,
                    5 => -2,
                    6 => -10,
                    7 => -12,
                    8 => -18,
                    9 => -18,
                    10 => -22,
                    11 => -26,
                    12 => -26,
                    13 => -16,
                    14 => -8,
                    _ => 0,
                };
            }
            else
            {
                return (NPC.frame.Y / frameHeight) switch
                {
                    4 => -2,
                    _ => 0,
                };
            }
        }

        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (!NPCLists.Soulless.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC();
            if (NPC.Sight(player, VisionRange, false, true))
            {
                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
            if (Main.rand.NextBool(MaskType == MaskState.Angry ? 600 : 1800))
            {
                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, false, true))
                {
                    SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
                return;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<MaskState> choice = new(Main.rand);
            choice.Add(MaskState.Normal, 1);
            choice.Add(MaskState.Happy, 1);
            choice.Add(MaskState.Angry, 1);

            MaskType = choice;
            if (Main.rand.NextBool(10))
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D head = ModContent.Request<Texture2D>("Redemption/NPCs/Soulless/Soulless_Masks").Value;
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D throwAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Slash").Value;
            Texture2D throwGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Slash_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int HeightH = head.Height / 3;
            int WidthH = head.Width / 3;
            int yH = HeightH * (int)MaskType;
            int xH = WidthH * HeadX;
            Rectangle rectH = new(xH, yH, WidthH, HeightH);
            if (AIState is ActionState.Slash)
            {
                int Height = throwAni.Height / 15;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, throwAni.Width, Height);
                Vector2 origin = new(throwAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(throwAni, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

                if (HasEyes)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(throwGlow, oldPos + NPC.Size / 2f - screenPos - new Vector2(0, 5), new Rectangle?(rect), Color.White * 0.5f, NPC.rotation, origin, NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(throwGlow, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
                }

                spriteBatch.Draw(head, NPC.Center - screenPos, new Rectangle?(rectH), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -37 : -23) + (HeadOffsetX * NPC.spriteDirection), -11 - HeadOffsetY), NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 5) - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                if (HasEyes)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(glow, oldPos - new Vector2(0, 5) + NPC.Size / 2f - screenPos, NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(glow, NPC.Center - new Vector2(0, 5) - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }

                spriteBatch.Draw(head, NPC.Center - screenPos, new Rectangle?(rectH), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -34 : -26) + (HeadOffsetX * NPC.spriteDirection), -9 - HeadOffsetY), NPC.scale, effects, 0);
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadesoulNPC>(), Main.rand.NextFloat(0, 0.6f));
            }
            else if (Main.rand.NextBool(3))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadesoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<Shadesoul>(), 3));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    ".")
            });
        }
    }
    internal class LightTendrilScarfPhys : IPhysChain
    {
        public Texture2D GetTexture(Mod mod)
        {
            return ModContent.Request<Texture2D>("Redemption/NPCs/Soulless/Soulless_LightTendril").Value;
        }

        public int NumberOfSegments => 11;

        public Color GetColor(PlayerDrawSet drawInfo, Color baseColour)
        {
            return baseColour;
        }

        public Vector2 AnchorOffset => new(-8, -6);

        public Vector2 OriginOffset(int index) //padding
        {
            switch (index)
            {
                case 0:
                    return new Vector2(0, 0);

                case 1:
                    return new Vector2(0, 0);

                case 2:
                    return new Vector2(0, 0);

                default:
                    return new Vector2(0, 0);
            }
        }

        public int Length(int index)
        {
            switch (index)
            {
                default:
                    return 5;
            }
        }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(NumberOfSegments, 1, NumberOfSegments - 1 - index, 0);
        }
        public Vector2 Force(Player player, int index, int dir, float gravDir, float time, NPC npc = null)
        {
            Vector2 force = new(
                -dir * 0.5f,
                Player.defaultGravity * (0.5f + NumberOfSegments * NumberOfSegments * 0.5f / (1 + index)) // Apply scaling gravity that weakens at the tip of the chain
                );

            if (!Main.gameMenu)
            {
                float windPower = 0.7f * dir * -10;

                // Wave in the wind
                force.X += 16f * -npc.spriteDirection;
                force.Y -= 10;
                force -= npc.velocity * 2;
                force.Y += (float)(Math.Sin(time * 1f * windPower - index * Math.Sign(force.X)) * 0.25f * windPower) * 6f * dir;
            }
            return force;
        }
    }
}
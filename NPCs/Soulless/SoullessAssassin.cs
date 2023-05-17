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
using ParticleLibrary;
using Redemption.Particles;
using Terraria.DataStructures;
using Redemption.Items.Usable;
using SubworldLibrary;
using Redemption.WorldGeneration.Soulless;

namespace Redemption.NPCs.Soulless
{
    public class SoullessAssassin : SoullessBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Hiding,
            Stalk,
            Alert,
            Stab
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 42;
            NPC.damage = 92;
            NPC.friendly = false;
            NPC.defense = 12;
            NPC.lifeMax = 2500;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            NPC.value = 5000;
            NPC.knockBackResist = 0.4f;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
            // TODO: Banner for soulless assassin
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();

            NPC.GetGlobalNPC<NPCPhysChain>().glowTrail = true;
            Tendril1 = new LightTendrilScarfPhys();
            Tendril2 = new LightTendrilScarfPhys();
            Tendril3 = new LightTendrilScarfPhys();
        }
        private static IPhysChain Tendril1;
        private static IPhysChain Tendril2;
        private static IPhysChain Tendril3;

        public override void HitEffect(NPC.HitInfo hit)
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

            if (AIState <= ActionState.Stalk)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            AIState = Main.rand.NextBool(2) ? ActionState.Hiding : ActionState.Idle;
            NPC.alpha = 0;
            NPC.netUpdate = true;
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
            if (AIState != ActionState.Stab)
                NPC.LookByVelocity();
            if (SoullessArea.soullessInts[1] is 6 && player.Hitbox.Intersects(SoullessArea.stalkerZone2))
                NPC.ai[0] = 10;
            if (NPC.ai[0] is 10)
            {
                NPC.alpha += 5;
                if (NPC.alpha >= 255)
                    NPC.active = false;
                return;
            }
            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 30 : NPC.Center.X), (int)(NPC.Center.Y - 8), 30, 18);
            if (AIState is ActionState.Hiding or ActionState.Stalk)
            {
                if (NPC.alpha < 240)
                    NPC.alpha += 4;
            }
            else
            {
                if (NPC.alpha > 0)
                    NPC.alpha -= 8;
            }
            if (NPC.alpha > 200)
                NPC.Redemption().invisible = true;

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
                        if (Main.rand.NextBool(3))
                        {
                            TimerRand = Main.rand.Next(120, 260);
                            AIState = ActionState.Wander;
                        }
                        else
                        {
                            TimerRand = Main.rand.Next(80, 280);
                            AIState = ActionState.Hiding;
                        }
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Hiding:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    AITimer++;
                    if (AITimer >= TimerRand * 6)
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
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, interactDoorStyle: 2);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, moveTo.Y * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 12, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Stalk:
                    if (NPC.ThreatenedCheck(ref runCooldown, 600))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }
                    if (NPC.PlayerDead())
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }

                    if (globalNPC.attacker.direction == NPC.direction * -1 && NPC.Sight(globalNPC.attacker, 100, true, true))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }
                    if (HasEyes && Main.rand.NextBool(200) && NPC.DistanceSQ(globalNPC.attacker.Center) >= 150 * 150)
                    {
                        Vector2 pos = new(globalNPC.attacker.Center.X, BaseWorldGen.GetFirstTileFloor((int)globalNPC.attacker.Center.X / 16, (int)globalNPC.attacker.Center.Y / 16) * 16);
                        bool landed = false;
                        int attempts = 0;
                        Vector2 telePos = Vector2.Zero;
                        while (!landed && attempts < 1000)
                        {
                            attempts++;
                            telePos = NPCHelper.FindGroundVector(pos, 6);
                            if (telePos.X > pos.X && globalNPC.attacker.direction == 1)
                                continue;
                            if (telePos.X < pos.X && globalNPC.attacker.direction == -1)
                                continue;
                            if (telePos.DistanceSQ(pos) < 40 * 40)
                                continue;

                            landed = true;
                        }
                        if (landed)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                                Main.dust[dust].velocity *= 0;
                                Main.dust[dust].noGravity = true;
                            }
                            SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse with { Pitch = .2f }, NPC.position);
                            NPC.Center = telePos - new Vector2(0, 26);
                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                                Main.dust[dust].velocity *= 0;
                                Main.dust[dust].noGravity = true;
                            }
                        }
                    }
                    if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 50, false, true))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Stab;
                    }

                    if (!NPC.Sight(globalNPC.attacker, VisionRange + 200, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 0.8f * SpeedMultiplier, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    if (NPC.PlayerDead())
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, interactDoorStyle: 2);

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 50, false, true))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Stab;
                    }
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 5f * SpeedMultiplier, 12, 12, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Stab:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = NPC.alpha < 10 ? ActionState.Wander : ActionState.Hiding;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AniFrameY == 2 && AITimer++ < 5)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.frameCounter = 0;
                    }
                    if (AniFrameY == 3 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC.immune[NPC.whoAmI] = 10;
                            int hitDirection = attackerNPC.RightOfDir(NPC);
                            BaseAI.DamageNPC(attackerNPC, damage * (NPC.alpha > 50 ? 2 : 1), 4, hitDirection, NPC);
                        }
                        else if (globalNPC.attacker is Player attackerPlayer)
                        {
                            int hitDirection = attackerPlayer.RightOfDir(NPC);
                            BaseAI.DamagePlayer(attackerPlayer, damage * (NPC.alpha > 50 ? 2 : 1), 4, hitDirection, NPC);
                        }
                    }
                    break;
            }
            if (HasEyes && Main.rand.NextBool(18) && NPC.alpha <= 50)
            {
                int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, Scale: 2);
                Main.dust[dust].velocity.Y = -2;
                Main.dust[dust].velocity.X = 0;
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
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

                if (AIState is ActionState.Stab)
                {
                    if (++NPC.frameCounter >= 4)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY is 3)
                        {
                            SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                            NPC.velocity.X = 6 * NPC.spriteDirection;
                        }
                        if (AniFrameY > 5)
                        {
                            AniFrameY = 0;
                            NPC.frame.Y = 0;
                            AITimer = 0;

                            RedeNPC globalNPC = NPC.Redemption();
                            if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 50, false, true))
                                NPC.LookAtEntity(globalNPC.attacker);
                            else
                                AIState = ActionState.Alert;
                        }
                    }
                    HeadOffsetY = SetHeadOffsetY(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;
                }
                AniFrameY = 0;

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
                            if (NPC.frame.Y > 2 * frameHeight)
                                NPC.frame.Y = 0 * frameHeight;
                        }
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.frame.Y = 0;
                }
            }
            HeadOffsetY = SetHeadOffsetY(ref frameHeight);
            HeadOffsetX = SetHeadOffsetX(ref frameHeight);
        }
        public override int SetHeadOffsetY(ref int frameHeight)
        {
            if (AIState is ActionState.Stab)
            {
                return AniFrameY switch
                {
                    4 => 2,
                    5 => 2,
                    _ => 0,
                };
            }
            else
            {
                return (NPC.frame.Y / frameHeight) switch
                {
                    0 => 0,
                    _ => -2,
                };
            }
        }
        public override int SetHeadOffsetX(ref int frameHeight)
        {
            if (AIState is ActionState.Stab)
            {
                return AniFrameY switch
                {
                    1 => 2,
                    2 => 2,
                    4 => -2,
                    5 => -2,
                    _ => 0,
                };
            }
            return 0;
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
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Stalk;
            }
            if (Main.rand.NextBool(MaskType == MaskState.Angry ? 600 : 1800))
            {
                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, false, true))
                {
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Stalk;
                }
                return;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<MaskState> choice = new(Main.rand);
            choice.Add(MaskState.Normal, 1);
            choice.Add(MaskState.Happy, .75);
            choice.Add(MaskState.Angry, .75);

            MaskType = choice;
            if (Main.rand.NextBool(10) && SubworldSystem.IsActive<SoullessSub>())
                HasEyes = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D head = ModContent.Request<Texture2D>("Redemption/NPCs/Soulless/Soulless_Masks").Value;
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D stabAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Stab").Value;
            Texture2D stabGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Stab_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int HeightH = head.Height / 3;
            int WidthH = head.Width / 3;
            int yH = HeightH * (int)MaskType;
            int xH = WidthH * HeadX;
            Rectangle rectH = new(xH, yH, WidthH, HeightH);
            if (AIState is ActionState.Stab)
            {
                int Height = stabAni.Height / 6;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, stabAni.Width, Height);
                Vector2 origin = new(stabAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(stabAni, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);

                if (HasEyes)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(stabGlow, oldPos + NPC.Size / 2f - screenPos - new Vector2(0, 5), new Rectangle?(rect), NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, origin, NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(stabGlow, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, effects, 0);
                }

                spriteBatch.Draw(head, NPC.Center - new Vector2(0, 5) - screenPos, new Rectangle?(rectH), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -14 : -6) + (HeadOffsetX * NPC.spriteDirection), -3 - HeadOffsetY), NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 1) - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                if (HasEyes)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(glow, oldPos - new Vector2(0, 1) + NPC.Size / 2f - screenPos, NPC.frame, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    }
                    spriteBatch.Draw(glow, NPC.Center - new Vector2(0, 1) - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }

                spriteBatch.Draw(head, NPC.Center - screenPos, new Rectangle?(rectH), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -18 : -2) + (HeadOffsetX * NPC.spriteDirection), -7 - HeadOffsetY), NPC.scale, effects, 0);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
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
            npcLoot.Add(ItemDropRule.ByCondition(new SoullessCondition(), ModContent.ItemType<ShadesoulGateway>(), 10));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "Depression in a human form. These half-spirits are formed by Willpower so meagre it caused the soul to invert itself, creating a shadesoul. It is said the masks they wear contain their potent sorrow."),
                new SoullessBestiaryInfoElement(
                    "Once slain in Epidotra, they are mysteriously sent to the Soulless Caverns, where they accumulate and try to survive in a competition of growth. Hopefully, someone is keeping them in check.")
            });
        }
    }
}
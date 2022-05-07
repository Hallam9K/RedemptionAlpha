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
    public class TwinfaceSoulless : SoullessBase
    {
        public enum ActionState
        {
            Start,
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
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 100;
            NPC.friendly = false;
            NPC.defense = 22;
            NPC.lifeMax = 4950;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            NPC.value = 9400;
            NPC.knockBackResist = 0.2f;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
            // TODO: Banner for soulless wanderer
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();

            NPC.GetGlobalNPC<NPCPhysChain>().glowChain = true;
            Tendril1 = new RedTendrilScarfPhys();
            Tendril2 = new RedTendrilScarfPhys();
            Tendril3 = new RedTendrilScarfPhys();
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
        public override bool CheckActive() => false;

        private Vector2 moveTo;
        private int runCooldown;
        private int dodgeCooldown;
        private bool powerUp;
        public override void OnSpawn(IEntitySource source)
        {
            NPC.alpha = 0;
            NPC.spriteDirection = -1;
            if (SoullessArea.soullessInts[0] > 0)
            {
                AIState = ActionState.Idle;
                TimerRand = Main.rand.Next(80, 280);

                NPC.dontTakeDamage = false;
                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            }
        }
        public override void AI()
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
                case ActionState.Start:
                    switch (TimerRand)
                    {
                        case 0:
                            if (NPC.DistanceSQ(new Vector2(447, 799) * 16) < 20 * 20)
                            {
                                NPC.velocity.X = 0;
                                TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            else
                                RedeHelper.HorizontallyMove(NPC, new Vector2(447, 799) * 16, 0.4f, 1, 12, 12, NPC.Center.Y > player.Center.Y);
                            break;
                        case 1:
                            if (AITimer++ >= 30 && AITimer % 10 == 0 && AITimer < 90)
                            {
                                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                            }
                            if (AITimer >= 120)
                            {
                                RedeHelper.SpawnNPC(new EntitySource_WorldGen(), 474 * 16, 759 * 16, ModContent.NPCType<LostLight>());

                                if (SoullessArea.soullessInts[0] < 1)
                                    SoullessArea.soullessInts[0] = 1;
                                TimerRand = 2;
                                NPC.netUpdate = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            break;
                        case 2:
                            if (NPC.DistanceSQ(new Vector2(534, 802) * 16) < 20 * 20)
                            {
                                NPC.dontTakeDamage = false;
                                NPC.velocity.X = 0;
                                NPC.spriteDirection = -1;
                                AIState = ActionState.Idle;
                                TimerRand = Main.rand.Next(80, 280);
                                NPC.netUpdate = true;
                                if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                            }
                            else
                                RedeHelper.HorizontallyMove(NPC, new Vector2(534, 802) * 16, 0.4f, 1, 12, 12, NPC.Center.Y > player.Center.Y);
                            break;
                    }
                    break;

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
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1, 12, 12, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Alert:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, interactDoorStyle: 2);

                    if (!NPC.Sight(globalNPC.attacker, 800, false, true))
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
                        if (Main.rand.NextBool(2))
                            powerUp = true;
                    }
                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(60) && NPC.Sight(globalNPC.attacker, 200, false, true))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Slash;
                        powerUp = true;
                    }

                    if (dodgeCooldown <= 0 && NPC.velocity.Y == 0)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (!proj.active || !proj.friendly || proj.damage <= 0 || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 90 + (proj.velocity.Length() * 4), false, true))
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
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 3.5f, 12, 12, NPC.Center.Y > globalNPC.attacker.Center.Y);
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

                    if (AniFrameY == 4 && AITimer++ < (powerUp ? 20 : 10))
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
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
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
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TwinfaceSoulless_Slash_Proj>(), damage, new Vector2(26 * NPC.spriteDirection, 0), false, SoundID.DD2_PhantomPhoenixShot, "", NPC.whoAmI);
                                if (AniFrameY is 10)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TwinfaceSoulless_Slash_Proj2>(), (int)(damage * 1.2f), new Vector2(36 * NPC.spriteDirection, 0), false, SoundID.DD2_PhantomPhoenixShot, "", NPC.whoAmI);
                            }
                        }
                        if (AniFrameY > 14)
                        {
                            AniFrameY = 0;
                            NPC.frame.Y = 0;
                            AITimer = 0;
                            if (powerUp)
                            {
                                NPC.velocity.X = -5 * NPC.spriteDirection;
                                NPC.velocity.Y -= 2;
                            }

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
                    return;
                }
                AniFrameY = 0;
                powerUp = false;

                if (NPC.collideY || NPC.velocity.Y == 0)
                {
                    NPC.rotation = 0;
                    if (NPC.velocity.X == 0)
                    {
                        if (AIState is ActionState.Start && AITimer >= 30 && AITimer < 90)
                        {
                            if (NPC.frameCounter++ >= 5)
                            {
                                NPC.frameCounter = 0;
                                if (NPC.frame.Y == 0)
                                    NPC.frame.Y = 5 * frameHeight;
                                else
                                    NPC.frame.Y = 0;
                            }
                        }
                        else
                            NPC.frame.Y = 0;
                    }
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
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            if (NPC.Sight(player, 600, false, true))
            {
                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        private float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D heart = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Heart").Value;
            Texture2D slashAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Slash").Value;
            Texture2D slashGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Slash_Glow").Value;
            Texture2D slashHeart = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Slash_Heart").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (AIState is ActionState.Slash)
            {
                int Height = slashAni.Height / 15;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, slashAni.Width, Height);
                Vector2 origin = new(slashAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(slashAni, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(slashGlow, oldPos + NPC.Size / 2f - screenPos - new Vector2(0, 5), new Rectangle?(rect), Color.White * 0.5f, NPC.rotation, origin, NPC.scale, effects, 0);
                }
                spriteBatch.Draw(slashGlow, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
                RedeDraw.DrawTreasureBagEffect(spriteBatch, slashHeart, ref drawTimer, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), Color.White, NPC.rotation, origin, NPC.scale, effects);
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 5) - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(glow, oldPos - new Vector2(0, 5) + NPC.Size / 2f - screenPos, NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
                spriteBatch.Draw(glow, NPC.Center - new Vector2(0, 5) - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                RedeDraw.DrawTreasureBagEffect(spriteBatch, heart, ref drawTimer, NPC.Center - screenPos - new Vector2(0, 5), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects);
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadesoulNPC>(), Main.rand.NextFloat(0, 0.8f));

            if (SoullessArea.soullessInts[0] < 2)
                SoullessArea.soullessInts[0] = 2;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<Shadesoul>(), 1, 5, 5));
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
    internal class RedTendrilScarfPhys : IPhysChain
    {
        public Texture2D GetTexture(Mod mod)
        {
            return ModContent.Request<Texture2D>("Redemption/NPCs/Soulless/Soulless_RedTendril").Value;
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
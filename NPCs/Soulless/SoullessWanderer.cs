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

namespace Redemption.NPCs.Soulless
{
    public class SoullessWanderer : SoullessBase
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert,
            Throw
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 80;
            NPC.friendly = false;
            NPC.defense = 12;
            NPC.lifeMax = 2750;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            NPC.value = 5000;
            NPC.knockBackResist = 0.4f;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
            // TODO: Banner for soulless wanderer
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<EpidotrianSkeletonBanner>();
        }
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
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Throw)
                NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Begin:
                    ChoosePersonality();
                    SetStats();

                    TimerRand = Main.rand.Next(80, 280);
                    AIState = ActionState.Idle;
                    NPC.alpha = 0;
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

                    if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, VisionRange, false, true))
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Throw;
                    }

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier, 12, 12, NPC.Center.Y > globalNPC.attacker.Center.Y);
                    break;

                case ActionState.Throw:
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

                    if (AniFrameY == 2 && AITimer++ < 30)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.frameCounter = 0;
                    }
                    break;
            }
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                /*HeadY = MaskType switch
                {
                    MaskState.Angry => 1,
                    MaskState.Happy => 2,
                    _ => 0,
                };*/

                if (AIState is ActionState.Throw)
                {
                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY is 3)
                        {
                            RedeNPC globalNPC = NPC.Redemption();
                            SoundEngine.PlaySound(SoundID.Item1, NPC.position);
                            NPC.velocity.X = 2 * NPC.spriteDirection;
                            int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * 0.2f) : NPC.damage;
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShadeJavelin_Proj>(), damage,
                                RedeHelper.PolarVector(14, (globalNPC.attacker.Center - NPC.Center).ToRotation() - (0.16f * NPC.spriteDirection)), false, SoundID.Item1, "", NPC.whoAmI);
                        }
                        if (AniFrameY > 5)
                        {
                            AniFrameY = 0;
                            NPC.frame.Y = 0;
                            AITimer = 0;

                            RedeNPC globalNPC = NPC.Redemption();
                            if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, VisionRange, false, true))
                                NPC.LookAtEntity(globalNPC.attacker);
                            else
                                AIState = ActionState.Alert;
                        }
                    }
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
                    NPC.frame.Y = 2 * frameHeight;
                }
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
            //Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D throwAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Throw").Value;
            //Texture2D throwGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Throw_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (AIState is ActionState.Throw)
            {
                int Height = throwAni.Height / 6;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, throwAni.Width, Height);
                Vector2 origin = new(throwAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(throwAni, NPC.Center - screenPos - new Vector2(0, 5), new Rectangle?(rect), drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

                //spriteBatch.Draw(head, NPC.Center - screenPos, new Rectangle?(rectH), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -48 : -38) + (HeadOffsetX * NPC.spriteDirection), -1 + HeadOffset), NPC.scale, effects, 0);

                //if (HasEyes)
                //   spriteBatch.Draw(SlashGlow, NPC.Center - screenPos - new Vector2(0, 11), new Rectangle?(rect), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 2) - screenPos, NPC.frame, NPC.IsABestiaryIconDummy ? drawColor : NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                //if (HasEyes)
                //    spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            /*int Height = head.Height / 3;
            int Width = head.Width / 3;
            int y = Height * MaskType;
            int x = Width * MaskX;
            Rectangle rect = new(x, y, Width, Height);
            spriteBatch.Draw(head, NPC.Center - screenPos, new Rectangle?(rect), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -4 : 2, 2 + HeadOffset), NPC.scale, effects, 0);

            */
            return false;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
            {
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadesoulNPC>(), Main.rand.NextFloat(0, 0.4f));
            }
            else if (Main.rand.NextBool(3))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadesoulNPC>(), Main.rand.NextFloat(0, 0.2f));
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
}
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.NPCs.PreHM;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class SkeletonAssassin_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonAssassin";
        public enum ActionState
        {
            Idle,
            Wander,
            Hiding,
            Stalk,
            Alert,
            Stab,
            SoulMove = 10
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Skeleton Assassin");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 50;
            NPC.damage = 23;
            NPC.friendly = true;
            NPC.defense = 8;
            NPC.lifeMax = 116;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.Redemption().spiritSummon = true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 2);

            if (AIState is ActionState.Idle or ActionState.Wander or ActionState.Hiding or ActionState.Stalk)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(NoticeSound, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override bool CheckActive() => false;
        private int runCooldown;
        public override void ModifyTypeName(ref string typeName)
        {
            if (NPC.ai[3] != -1)
                typeName = Main.player[(int)NPC.ai[3]].name + "'s " + Lang.GetNPCNameValue(Type);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool(3))
                HasEyes = true;
            SetStats();

            moveTo = NPC.FindGround(20);
            TimerRand = Main.rand.Next(80, 280);
            AIState = Main.rand.NextBool(2) ? ActionState.Hiding : ActionState.Wander;
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            SSBase.SpiritBasicAI(NPC, player);

            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;
            NPC.TargetClosest();
            if (AIState != ActionState.Stab)
                NPC.LookByVelocity();
            Rectangle KnifeHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 46 : NPC.Center.X + 10), (int)(NPC.Center.Y - 12), 36, 18);

            if (Main.rand.NextBool(6500) && !Main.dedServ)
                SoundEngine.PlaySound(AmbientSound, NPC.position);

            if (AIState is not ActionState.SoulMove)
            {
                if (AIState is ActionState.Hiding or ActionState.Stalk)
                {
                    if (NPC.alpha < 240)
                        NPC.alpha += 4;
                }
                else
                {
                    if (NPC.alpha <= 30)
                    {
                        if (SSBase.NoSpiritEffect(NPC))
                            NPC.alpha = 0;
                        else
                        {
                            NPC.alpha += Main.rand.Next(-10, 11);
                            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
                        }
                    }
                    else
                        NPC.alpha -= 8;
                }
            }
            if (NPC.alpha > 200)
                NPC.Redemption().invisible = true;

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.DistanceSQ(player.Center) > 200 * 200)
                    {
                        moveTo = NPC.FindGround(20);
                        if (NPC.DistanceSQ(player.Center) > 200 * 200)
                            moveTo = (player.Center + new Vector2(20 * NPC.RightOfDir(player), 0)) / 16;
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
                    if (AITimer >= TimerRand * 6 || NPC.DistanceSQ(player.Center) > 200 * 200)
                    {
                        moveTo = NPC.FindGround(20);
                        if (NPC.DistanceSQ(player.Center) > 200 * 200)
                            moveTo = (player.Center + new Vector2(20 * NPC.RightOfDir(player), 0)) / 16;
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
                        NPC.netUpdate = true;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16, player);
                    break;

                case ActionState.Stalk:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Hiding;
                        break;
                    }

                    if (HasEyes && attacker.direction == -NPC.direction && NPC.Sight(attacker, 200, true, true))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(NoticeSound, NPC.position);
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 50 * 50)
                    {
                        NPC.LookAtEntity(attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 3 * NPC.spriteDirection;
                        AIState = ActionState.Stab;
                    }

                    if (!NPC.Sight(attacker, VisionRange + 200, HasEyes, HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 0.8f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > attacker.Center.Y, attacker);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 50 * 50)
                    {
                        NPC.LookAtEntity(attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 3 * NPC.spriteDirection;
                        AIState = ActionState.Stab;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > attacker.Center.Y, attacker);
                    break;

                case ActionState.Stab:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = NPC.alpha < 10 ? ActionState.Wander : ActionState.Hiding;
                        break;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    AITimer++;

                    if (AITimer == 10)
                        SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                    if (AITimer >= 10)
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 2, KnifeHitbox, NPC.damage * (NPC.alpha > 50 ? 2 : 1), 4f, false, 20);
                    break;
                case ActionState.SoulMove:
                    SSBase.SoulMoveState(NPC, ref AITimer, player, ref TimerRand, ref runCooldown, ref moveTo, yOffset: 4);
                    break;
            }
            CustomFrames(56);
        }
        private void CustomFrames(int frameHeight)
        {
            if (AIState is ActionState.Stab)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                    NPC.frame.Y = 13 * frameHeight;
                else if (NPC.frameCounter < 20)
                    NPC.frame.Y = 14 * frameHeight;
                else if (NPC.frameCounter < 30)
                    NPC.frame.Y = 15 * frameHeight;
                else
                {
                    AIState = ActionState.Alert;
                    NPC.frameCounter = 0;
                }
                return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            if (AIState is ActionState.Stab)
                return;
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 5 * frameHeight;
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
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();

            int gotNPC = SSBase.GetNearestNPC(NPC);
            if (player.MinionAttackTargetNPC != -1)
                gotNPC = player.MinionAttackTargetNPC;
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false) || gotNPC == player.MinionAttackTargetNPC))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Stalk;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            bool noSpiritEffect = SSBase.NoSpiritEffect(NPC);
            Color color = noSpiritEffect ? drawColor : Color.White;
            if (!noSpiritEffect)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!noSpiritEffect)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffType<DirtyWoundDebuff>(), Main.rand.Next(800, 2400));
        }
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
    }
}
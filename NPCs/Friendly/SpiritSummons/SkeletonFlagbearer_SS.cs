using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPCs;
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
    public class SkeletonFlagbearer_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonFlagbearer";
        public enum ActionState
        {
            Idle,
            Wander,
            Retreat,
            Follow,
            Dance,
            SoulMove = 10
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Skeleton Flagbearer");
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 46;
            NPC.damage = 10;
            NPC.friendly = true;
            NPC.defense = 9;
            NPC.lifeMax = 92;
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

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                AIState = ActionState.Retreat;
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

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            SSBase.SpiritBasicAI(NPC, player);
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (Main.rand.NextBool(4000) && !Main.dedServ)
                SoundEngine.PlaySound(AmbientSound, NPC.position);

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

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16, player);
                    break;

                case ActionState.Retreat:
                    SightCheck();
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, NPCHelper.RunAwayVector(NPC, attacker), 0.4f, 2.4f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.1f : 1), 12, 8, NPC.Center.Y > player.Center.Y, attacker);
                    break;

                case ActionState.Follow:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }
                    if (globalNPC.attacker is NPC nonSpirit && !nonSpirit.Redemption().spiritSummon)
                    {
                        AIState = ActionState.Retreat;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 140 * 140)
                    {
                        NPC.LookAtEntity(attacker);
                        NPC.frameCounter = 0;
                        NPC.frame.Y = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Dance;
                    }
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 2.4f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > attacker.Center.Y, attacker);
                    break;

                case ActionState.Dance:
                    if (NPC.ThreatenedCheck(ref runCooldown, 360, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AITimer = 0;
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }
                    if (globalNPC.attacker is NPC nonSpirit2 && !nonSpirit2.Redemption().spiritSummon)
                    {
                        NPC.frameCounter = 0;
                        AIState = ActionState.Retreat;
                        break;
                    }

                    NPC.LookAtEntity(attacker);

                    NPC.velocity *= 0;

                    AITimer++;
                    if (AITimer % 60 == 0)
                    {
                        RedeDraw.SpawnCirclePulse(NPC.Center, Color.DarkRed, .75f, NPC);
                        foreach (NPC target in Main.ActiveNPCs)
                        {
                            if (target.whoAmI == NPC.whoAmI)
                                continue;

                            if (!target.Redemption().spiritSummon)
                                continue;

                            if (NPC.DistanceSQ(target.Center) > 300 * 300)
                                continue;

                            target.AddBuff(BuffType<FlagbearerBuff>(), 600);
                        }
                    }

                    if (NPC.DistanceSQ(attacker.Center) >= 300 * 300)
                    {
                        NPC.frameCounter = 0;
                        AIState = ActionState.Follow;
                    }
                    break;
                case ActionState.SoulMove:
                    SSBase.SoulMoveState(NPC, ref AITimer, player, ref TimerRand, ref runCooldown, ref moveTo, yOffset: 4);
                    break;
            }
            if (AIState is not ActionState.SoulMove)
            {
                if (SSBase.NoSpiritEffect(NPC))
                    NPC.alpha = 0;
                else
                {
                    NPC.alpha += Main.rand.Next(-10, 11);
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 6;
            NPC.frame.X = AIState is ActionState.Dance ? NPC.frame.Width : 0;
            if (AIState is ActionState.Dance)
            {
                if (++NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 7 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
                return;
            }

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
        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.type == NPCType<SkeletonFlagbearer>() || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (!target.Redemption().spiritSummon)
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
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();

            int gotNPC = GetNearestNPC();
            if (player.MinionAttackTargetNPC != -1)
                gotNPC = player.MinionAttackTargetNPC;
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false) || gotNPC == player.MinionAttackTargetNPC))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Follow;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = Request<Texture2D>(Texture + "_Glow").Value;
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

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 33), NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(Glow, NPC.Center - screenPos - new Vector2(0, 33), NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!noSpiritEffect)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
    }
}
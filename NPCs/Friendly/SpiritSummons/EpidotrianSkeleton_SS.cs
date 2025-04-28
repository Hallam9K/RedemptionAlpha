using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
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
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class EpidotrianSkeleton_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/EpidotrianSkeleton";
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            SoulMove = 10
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 18;
            NPC.friendly = true;
            NPC.defense = 7;
            NPC.lifeMax = 108;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.knockBackResist = 0.5f;
            NPC.alpha = 255;
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
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.SkeletonNotice with { Volume = .5f }, NPC.position);
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
            ChoosePersonality();
            SetStats();

            NPC.alpha = 0;
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;
            NPC.TargetClosest();
            SSBase.SpiritBasicAI(NPC, player);

            NPC.LookByVelocity();

            if (Main.rand.NextBool(3500) && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.SkeletonAmbient with { Volume = .5f }, NPC.position);
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

                    SightCheck();
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.DamageAnyAttackers(0, 4);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > attacker.Center.Y, attacker);

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
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
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
            HeadOffset = SetHeadOffset(ref frameHeight);
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
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.SkeletonNotice with { Volume = .5f }, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<int> head = new(Main.rand);
            head.Add(0);
            head.Add(1, 0.6);
            head.Add(2, 0.6);
            head.Add(3, 0.4);
            head.Add(4, 0.4);
            head.Add(5, 0.1);
            head.Add(6, 0.1);
            head.Add(7, 0.1);
            head.Add(8, 0.06);
            head.Add(9, 0.06);
            head.Add(10, 0.06);
            head.Add(11, 0.06);
            HeadType = head;

            if (Main.rand.NextBool(3))
                HasEyes = true;
            NPC.netUpdate = true;
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

            int Height = head.Value.Height / 14;
            int Width = head.Value.Width / 2;
            int y = Height * HeadType;
            int x = Width * HeadX;
            Rectangle rect = new(x, y, Width, Height);
            spriteBatch.Draw(head.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -4 : 2, 2 + HeadOffset), NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!noSpiritEffect)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => AIState == ActionState.Alert;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
    }
}
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
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
    public class Asher_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonDuelist";
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            IdleAlert,
            Attack,
            SoulMove = 10
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Asher");
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 48;
            NPC.damage = 22;
            NPC.friendly = true;
            NPC.defense = 10;
            NPC.lifeMax = 380;
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

            if (AIState is ActionState.Idle or ActionState.Wander or ActionState.IdleAlert)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(NoticeSound, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override bool CheckActive() => false;
        private int runCooldown;
        private int dodgeCooldown;

        private int AniFrameY;
        private int AniFrameX;
        public override void OnSpawn(IEntitySource source)
        {
            HeadType = 8;
            HasEyes = true;
            VisionIncrease = 300;
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
            Rectangle SlashHitbox1 = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 66 : NPC.Center.X + 4), (int)(NPC.Center.Y - 60), 62, 86);
            Rectangle SlashHitbox2 = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 94 : NPC.Center.X), (int)(NPC.Center.Y - 40), 94, 84);
            dodgeCooldown--;
            dodgeCooldown = (int)MathHelper.Max(0, dodgeCooldown);

            if (Main.rand.NextBool(4500) && !Main.dedServ)
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

                case ActionState.IdleAlert:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
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
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 2 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 240);
                        AIState = ActionState.IdleAlert;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (dodgeCooldown <= 0 && NPC.velocity.Y == 0)
                    {
                        foreach (Projectile proj in Main.ActiveProjectiles)
                        {
                            if (!proj.hostile || proj.damage <= 0 || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 90 + (proj.velocity.Length() * 4), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke);
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            NPC.Dodge(proj, 6, 2, 12);
                            dodgeCooldown = 60;
                        }
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 80 * 80)
                    {
                        NPC.LookAtEntity(attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Attack;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 4.2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1),
                        12, 8, NPC.Center.Y > attacker.Center.Y, attacker);

                    break;

                case ActionState.Attack:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }

                    NPC.LookAtEntity(attacker);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AniFrameY is 3 or 6)
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 2, AniFrameY == 3 ? SlashHitbox1 : SlashHitbox2, NPC.damage, 6f, false, 6);
                    break;
                case ActionState.SoulMove:
                    SSBase.SoulMoveState(NPC, ref AITimer, player, ref TimerRand, ref runCooldown, ref moveTo);
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
            CustomFrames(58);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            int damageDone = hit.Damage;
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffType<DirtyWoundDebuff>(), Main.rand.Next(800, 2400));
        }
        private void CustomFrames(int frameHeight)
        {
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;

            if (AIState is ActionState.Attack)
            {
                if (++NPC.frameCounter >= 3)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AniFrameY is 3 or 6)
                    {
                        SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                        NPC.velocity.X = 2 * NPC.spriteDirection;
                    }
                    if (AniFrameY > 10)
                    {
                        AniFrameY = 0;
                        NPC.frame.Y = 0;
                        HeadOffset = 0;

                        if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 100 * 100)
                            NPC.LookAtEntity(attacker);
                        else
                            AIState = ActionState.Alert;
                    }
                }
                if (AIState is ActionState.Attack)
                    HeadOffset = SetHeadOffsetY();
                else
                    HeadOffset = SetHeadOffset(ref frameHeight);
                HeadOffsetX = SetHeadOffsetX();
                return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private int HeadOffsetX;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 6;
            NPC.frame.X = NPC.frame.Width * (AIState is ActionState.Alert or ActionState.IdleAlert ? 2 : 3);
            AniFrameX = 1;
            if (AIState is ActionState.Attack)
                return;
            AniFrameY = 0;

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
            if (AIState is ActionState.Attack)
                HeadOffset = SetHeadOffsetY();
            else
                HeadOffset = SetHeadOffset(ref frameHeight);
            HeadOffsetX = SetHeadOffsetX();
        }
        public int SetHeadOffsetY()
        {
            return AniFrameY switch
            {
                1 => 2,
                3 => -2,
                4 => -2,
                10 => 2,
                _ => 0,
            };
        }
        public int SetHeadOffsetX()
        {
            return AniFrameY switch
            {
                1 => 2,
                6 => -6,
                7 => -14,
                8 => -14,
                9 => -6,
                _ => 0,
            };
        }
        public void SightCheck()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;

            int gotNPC = SSBase.GetNearestNPC(NPC);
            if (player.MinionAttackTargetNPC != -1)
                gotNPC = player.MinionAttackTargetNPC;
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false) || gotNPC == player.MinionAttackTargetNPC))
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(NoticeSound, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center - new Vector2(0, 1);

            bool noSpiritEffect = SSBase.NoSpiritEffect(NPC);
            Color color = noSpiritEffect ? drawColor : Color.White;
            if (!noSpiritEffect)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            int HeightH = head2.Value.Height / 12;
            int WidthH = head2.Value.Width / 2;
            int yH = HeightH * HeadType;
            int xH = WidthH * HeadX;
            Rectangle rectH = new(xH, yH, WidthH, HeightH);

            if (AIState is ActionState.Attack)
            {
                int Height = SlashAni.Value.Height / 11;
                int Width = SlashAni.Value.Width / 3;
                int y = Height * AniFrameY;
                int x = Width * AniFrameX;
                Rectangle rect = new(x, y, Width, Height);
                Vector2 origin = new(Width / 2f, Height / 2f);
                spriteBatch.Draw(SlashAni.Value, pos - screenPos - new Vector2(0, 11), new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, origin, NPC.scale, effects, 0);

                spriteBatch.Draw(head2.Value, pos - screenPos, new Rectangle?(rectH), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -48 : -38) + (HeadOffsetX * NPC.spriteDirection), -1 + HeadOffset), NPC.scale, effects, 0);

                if (HasEyes)
                    spriteBatch.Draw(SlashGlow.Value, pos - screenPos - new Vector2(0, 11), new Rectangle?(rect), NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    float fade = dodgeCooldown / 120f;
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY - 1), NPC.frame, NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    }
                }
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                spriteBatch.Draw(head2.Value, pos - screenPos, new Rectangle?(rectH), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -48 : -38) + (HeadOffsetX * NPC.spriteDirection), -2 + HeadOffset), NPC.scale, effects, 0);

                if (HasEyes)
                    spriteBatch.Draw(Glow, pos - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
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
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(.8f, 1.6f));
        }
    }
}
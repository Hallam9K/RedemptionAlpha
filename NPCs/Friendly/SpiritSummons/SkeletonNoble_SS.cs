using Microsoft.Xna.Framework.Graphics;
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
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class SkeletonNoble_SS : SkeletonBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonNoble";
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Stab,
            Slash,
            SoulMove = 10
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Skeleton Noble");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCSets.UsesGuardPoints[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.damage = 28;
            NPC.friendly = true;
            NPC.defense = 15;
            NPC.lifeMax = 144;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.knockBackResist = 0.4f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.RedemptionGuard().GuardPoints = 20;
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
                    SoundEngine.PlaySound(NoticeSound, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (!NPC.RedemptionGuard().GuardBroken)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.NPCHit4, .25f, false, DustID.DungeonSpirit, damage: NPC.lifeMax / 4);
            }
        }

        public override bool CheckActive() => false;
        private int runCooldown;
        public override void ModifyTypeName(ref string typeName)
        {
            if (NPC.ai[3] != -1)
                typeName = Main.player[(int)NPC.ai[3]].name + "'s " + Lang.GetNPCNameValue(Type);
        }

        private int AniFrameY;
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
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
            if (AIState != ActionState.Stab)
                NPC.LookByVelocity();

            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 78 : NPC.Center.X), (int)(NPC.Center.Y - 66), 78, 94);

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
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.9f * SpeedMultiplier, 6, 6, NPC.Center.Y > moveTo.Y * 16, player);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (!NPC.Sight(attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(attacker.Center) < 80 * 80)
                    {
                        NPC.LookAtEntity(attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = Main.rand.NextBool(2) ? ActionState.Stab : ActionState.Slash;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.2f, 1.7f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1),
                        6, 6, NPC.Center.Y > attacker.Center.Y, attacker);
                    break;

                case ActionState.Slash:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    NPC.LookAtEntity(attacker);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (AniFrameY is 4)
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 2, SlashHitbox, NPC.damage, 9f, false, 10);
                    break;

                case ActionState.Stab:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2) && AITimer == 0)
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    Vector2 attackerPos = Main.LocalPlayer.Center;
                    if (attacker != null)
                        attackerPos = attacker.Center;

                    if (AITimer == 0)
                    {
                        NPC.Shoot(NPC.Center, ProjectileType<SkeletonNoble_SS_HalberdProj>(), NPC.damage,
                            RedeHelper.PolarVector(8, (attackerPos - NPC.Center).ToRotation()), SoundID.Item1, NPC.whoAmI);
                        AITimer = 1;
                    }
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
            CustomFrames(70);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 1f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            int damageDone = hit.Damage;
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit, 80);

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffType<DirtyWoundDebuff>(), Main.rand.Next(800, 2400));
        }
        private void CustomFrames(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.Stab:
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 10)
                        NPC.frame.Y = 13 * frameHeight;
                    else if (NPC.frameCounter < 20)
                        NPC.frame.Y = 14 * frameHeight;
                    else if (NPC.frameCounter < 40)
                        NPC.frame.Y = 15 * frameHeight;
                    else
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Alert;
                    }
                    if (AIState is ActionState.Slash)
                        HeadOffset = SetHeadOffsetY();
                    else
                        HeadOffset = SetHeadOffset(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;

                case ActionState.Slash:
                    if (++NPC.frameCounter >= 7)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY is 1)
                            SoundEngine.PlaySound(SoundID.Item19, NPC.position);
                        if (AniFrameY is 4)
                        {
                            SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;
                            NPC.velocity.X = 2 * NPC.spriteDirection;
                        }
                        if (AniFrameY > 5)
                        {
                            AniFrameY = 0;
                            AIState = ActionState.Alert;
                        }
                    }
                    if (AIState is ActionState.Slash)
                        HeadOffset = SetHeadOffsetY();
                    else
                        HeadOffset = SetHeadOffset(ref frameHeight);
                    HeadOffsetX = SetHeadOffsetX(ref frameHeight);
                    return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private int HeadOffsetX;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            if (AIState is ActionState.Slash or ActionState.Stab)
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
            if (AIState is ActionState.Slash)
                HeadOffset = SetHeadOffsetY();
            else
                HeadOffset = SetHeadOffset(ref frameHeight);
            HeadOffsetX = SetHeadOffsetX(ref frameHeight);
        }
        public override int SetHeadOffset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                1 => -2,
                2 => -2,
                3 => -2,
                5 => -2,
                8 => -2,
                9 => -2,
                12 => -2,
                14 => -2,
                _ => 0,
            };
        }
        public int SetHeadOffsetY()
        {
            return AniFrameY switch
            {
                1 => -2,
                2 => -2,
                5 => 4,
                _ => 0,
            };
        }
        public int SetHeadOffsetX(ref int frameHeight)
        {
            if (AIState is ActionState.Slash)
            {
                return AniFrameY switch
                {
                    1 => 2,
                    4 => -8,
                    5 => -2,
                    _ => 0,
                };
            }
            else
            {
                return (NPC.frame.Y / frameHeight) switch
                {
                    4 => -2,
                    8 => -2,
                    12 => -2,
                    14 => -10,
                    15 => -2,
                    _ => 0,
                };
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
                if (!Main.dedServ)
                    SoundEngine.PlaySound(NoticeSound, NPC.position);
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
            head.Add(12, 0.3);
            head.Add(13, 0.3);
            HeadType = head;

            if (Main.rand.NextBool(2))
                HasEyes = true;
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

            int HeightH = head.Value.Height / 14;
            int WidthH = head.Value.Width / 2;
            int yH = HeightH * HeadType;
            int xH = WidthH * HeadX;
            Rectangle rectH = new(xH, yH, WidthH, HeightH);

            if (AIState is ActionState.Slash)
            {
                spriteBatch.Draw(head.Value, NPC.Center - screenPos, new Rectangle?(rectH), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -35 : -35) + (HeadOffsetX * NPC.spriteDirection), -6 + -HeadOffset), NPC.scale, effects, 0);

                int Height = NobleSlashAni.Value.Height / 6;
                int Width = NobleSlashAni.Value.Width / 3;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, Width, Height);
                Vector2 origin = new(Width / 2f, Height / 2f);
                spriteBatch.Draw(NobleSlashAni.Value, NPC.Center - screenPos - new Vector2(0, 17), new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, origin, NPC.scale, effects, 0);

                if (HasEyes && HeadType < 12)
                    spriteBatch.Draw(NobleSlashGlow.Value, NPC.Center - screenPos - new Vector2(0, 17), new Rectangle?(rect), NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(head.Value, NPC.Center - screenPos, new Rectangle?(rectH), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2 + new Vector2((NPC.spriteDirection == 1 ? -36 : -34) + (HeadOffsetX * NPC.spriteDirection), -4 + HeadOffset), NPC.scale, effects, 0);

                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 4), NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                if (HasEyes && HeadType < 12)
                    spriteBatch.Draw(Glow, NPC.Center - screenPos - new Vector2(0, 4), NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
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
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.2f, 0.8f));
        }
    }
    public class SkeletonNoble_SS_HalberdProj : SkeletonNoble_HalberdProj
    {
        public override string Texture => "Redemption/NPCs/PreHM/SkeletonNoble_HalberdProj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Noble's Halberd");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.Redemption().friendlyHostile = false;
            Projectile.Redemption().IsSpear = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool noSpiritEffect = SSBase.NoSpiritEffect(Main.npc[(int)Projectile.ai[0]]);
            Color color = noSpiritEffect ? lightColor : Color.White;
            if (!noSpiritEffect)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0, 8), new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            if (!noSpiritEffect)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            return false;
        }
    }
}
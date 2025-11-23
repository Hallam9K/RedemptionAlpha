using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.NPCs.Minibosses.Calavia;
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
    public class Calavia_SS : SSBase
    {
        public override string Texture => "Redemption/NPCs/Friendly/SpiritSummons/Kyretha";
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Slash,
            Stab,
            SpinSlash,
            Icefall,
            SoulMove = 10
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.localAI[0];
        public float[] oldrot = new float[6];
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Kyretha");
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Hot);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Cold);
        }
        public override void SetSafeDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 78;
            NPC.defense = 17;
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0.2f;
            NPC.HitSound = SoundID.FemaleHit with { Pitch = .3f, Volume = .5f };
            NPC.DeathSound = SoundID.PlayerKilled with { Pitch = .1f };
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.RedemptionGuard().GuardPoints = NPC.lifeMax / 10;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= .8f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (hit.Damage > 1)
                SoundEngine.PlaySound(SoundID.NPCHit4, NPC.position);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit);
                    Main.dust[dustIndex].velocity *= 2;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(2.6f, 3.4f));
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.localAI[0] = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
        }
        private int runCooldown;
        private int dodgeCooldown;
        public float[] doorVars = new float[3];
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;
            NPC.TargetClosest();

            dodgeCooldown--;
            dodgeCooldown = (int)MathHelper.Max(0, dodgeCooldown);

            if (AIState < ActionState.Slash)
                NPC.LookByVelocity();

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
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 3, 18, 18, NPC.Center.Y > moveTo.Y * 16, player);
                    break;
                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 380))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }

                    if (!NPC.Sight(attacker, 1400, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 12, attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, attacker.Center, 0.3f, 4, 18, 18, NPC.Center.Y > attacker.Center.Y, attacker);

                    if (NPC.life < NPC.lifeMax / 10 && dodgeCooldown <= 0 && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        foreach (Projectile proj in Main.ActiveProjectiles)
                        {
                            if (!proj.hostile || proj.damage <= 0 || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 100 + (proj.velocity.Length() * 5), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke);
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            if (Main.rand.NextBool())
                                NPC.velocity.X *= -1;
                            NPC.velocity.X *= 2f;
                            NPC.velocity.Y -= Main.rand.NextFloat(1, 3);
                            dodgeCooldown = 30;
                            break;
                        }
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);
                    Rectangle tooHighCheck = new((int)NPC.Center.X - 60, (int)NPC.Center.Y - 240, 120, 240);
                    if (NPC.DistanceSQ(attacker.Center) < 180 * 180 || attacker.Hitbox.Intersects(tooHighCheck) || (AITimer++ >= 60 && !NPC.Sight(attacker, -1, false, true)))
                    {
                        WeightedRandom<ActionState> attacks = new(Main.rand);
                        attacks.Add(ActionState.Slash);
                        attacks.Add(ActionState.Stab, NPC.DistanceSQ(attacker.Center) < 80 * 80 ? 0 : 0.5);
                        attacks.Add(ActionState.SpinSlash, 0.2);
                        attacks.Add(ActionState.Icefall, 0.3);
                        ActionState choice = attacks;
                        if (!NPC.Sight(attacker, -1, false, true, true))
                            choice = ActionState.Stab;
                        if (BaseAI.HitTileOnSide(NPC, 3))
                        {
                            AIState = choice;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = 0;
                            if (choice == ActionState.Slash && attacker.Center.Y + 60 < NPC.Center.Y)
                            {
                                AITimer = -60;
                                TimerRand = 1;
                            }
                        }
                        break;
                    }
                    break;
                case ActionState.Slash:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    switch (TimerRand)
                    {
                        default:
                            if (AITimer++ == 0)
                            {
                                CustomBodyAni = 2;
                                TimerRand2 = Main.rand.Next(10, 21);
                                int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                                NPC.Shoot(NPC.Center, ProjectileType<Calavia_SS_BladeOfTheMountain>(), damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, NPC.whoAmI, TimerRand2, knockback: 7);
                            }
                            if (AITimer < TimerRand2 + 10)
                            {
                                NPC.LookAtEntity(attacker);
                                NPC.velocity.X *= 0.5f;
                            }
                            else
                                NPC.velocity *= 0.94f;
                            if (AITimer == TimerRand2 + 10)
                            {
                                NPC.LookByVelocity();
                                if (NPC.DistanceSQ(attacker.Center) > 70 * 70)
                                    NPC.velocity.X += 12 * NPC.spriteDirection;
                            }
                            if (AITimer >= TimerRand2 + 40)
                            {
                                NPC.knockBackResist = 0.2f;
                                CustomBodyAni = 0;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.Alert;
                            }
                            break;
                        case 1:
                            if (AITimer < -20 && BaseAI.HitTileOnSide(NPC, 3))
                            {
                                NPC.velocity.Y -= MathHelper.Distance(NPC.Center.Y, attacker.Center.Y) / 8;
                                AITimer = -20;
                            }

                            if (AITimer++ == 0)
                            {
                                CustomBodyAni = 2;
                                TimerRand2 = Main.rand.Next(0, 11);
                                int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                                NPC.Shoot(NPC.Center, ProjectileType<Calavia_SS_BladeOfTheMountain>(), damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, NPC.whoAmI, TimerRand2, knockback: 7);
                            }
                            if (AITimer >= 0 && AITimer < TimerRand2 + 10)
                            {
                                NPC.LookAtEntity(attacker);
                                NPC.velocity.Y = -.51f;
                                NPC.velocity *= 0.6f;
                            }
                            else
                                NPC.velocity *= 0.94f;
                            if (AITimer == TimerRand2 + 10)
                            {
                                NPC.LookByVelocity();
                                if (NPC.DistanceSQ(attacker.Center) > 70 * 70)
                                    NPC.velocity.X += 18 * NPC.spriteDirection;
                            }
                            if (AITimer >= TimerRand2 + 40)
                            {
                                NPC.knockBackResist = 0.2f;
                                CustomBodyAni = 0;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.Alert;
                            }
                            break;
                    }
                    break;
                case ActionState.Stab:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    switch (TimerRand)
                    {
                        default:
                            NPC.LookAtEntity(attacker);
                            if (AITimer++ == 0)
                            {
                                NPC.velocity *= 0;
                                customArm = true;
                                int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                                NPC.Shoot(NPC.Center, ProjectileType<Calavia_SS_BladeOfTheMountain2>(), damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, NPC.whoAmI, TimerRand2, knockback: 7);
                            }
                            break;
                        case 1:
                            NPC.LookByVelocity();
                            NPC.rotation = NPC.velocity.X * 0.07f;
                            dodgeCooldown = 20;
                            NPC.noGravity = true;
                            if (AITimer++ == 0 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swoosh1, NPC.position);
                            if (AITimer >= 20)
                                TimerRand = 2;
                            break;
                        case 2:
                            NPC.noGravity = false;
                            customArm = false;
                            customArmRot = 0;
                            dodgeCooldown = 30;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = 0;
                            AIState = ActionState.Alert;
                            break;
                    }
                    break;
                case ActionState.SpinSlash:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    if (AITimer++ == 0)
                    {
                        NPC.velocity *= 0;
                        customArm = true;
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        NPC.Shoot(NPC.Center, ProjectileType<Calavia_SS_BladeOfTheMountain2>(), damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, NPC.whoAmI, 3, knockback: 7);
                    }
                    if (AITimer == 60)
                        NPC.velocity.Y = -12;
                    if (AITimer >= 80)
                    {
                        dodgeCooldown = 20;
                        NPC.rotation += 0.5f * NPC.spriteDirection;
                        NPC.velocity.Y = -.49f;
                        NPC.velocity.X += 0.04f * NPC.spriteDirection;
                    }
                    else
                    {
                        NPC.LookAtEntity(attacker);
                        NPC.velocity *= 0.94f;
                    }
                    if (AITimer >= 200)
                    {
                        NPC.noGravity = false;
                        customArm = false;
                        customArmRot = 0;
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                        AIState = ActionState.Alert;
                    }
                    break;
                case ActionState.Icefall:
                    BaseAI.WalkupHalfBricks(NPC);
                    NPC.velocity.X *= .96f;
                    if (AITimer++ == 0)
                    {
                        NPC.velocity *= 0;
                        HoldIcefall = true;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.IceMist, NPC.position);
                    }
                    if (AITimer == 30)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            NPC.Shoot(NPC.Center + new Vector2(100, -10), ProjectileType<Calavia_IcefallMist>(), NPC.damage * NPCHelper.HostileProjDamageMultiplier(), new Vector2(Main.rand.NextFloat(-1, 1), 0), 1, knockback: 2, ai2: NPC.whoAmI);
                            NPC.Shoot(NPC.Center + new Vector2(-100, -10), ProjectileType<Calavia_IcefallMist>(), NPC.damage * NPCHelper.HostileProjDamageMultiplier(), new Vector2(Main.rand.NextFloat(-1, 1), 0), 1, knockback: 2, ai2: NPC.whoAmI);
                        }
                    }
                    if (AITimer >= 80)
                    {
                        HoldIcefall = false;
                        AIState = ActionState.Slash;
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                    }
                    break;
                case ActionState.SoulMove:
                    SoulMoveState(NPC, ref AITimer, player, ref TimerRand, ref runCooldown, ref moveTo);
                    break;

            }
            if (AIState is not ActionState.SoulMove)
            {
                if (NoSpiritEffect(NPC))
                    NPC.alpha = 0;
                else
                {
                    NPC.alpha += Main.rand.Next(-10, 11);
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public int BodyFrame;
        public int CustomBodyAni;
        private int CustomBodyCounter;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (AIState is not ActionState.SpinSlash || AITimer < 120)
                    NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
            {
                if (AIState is not ActionState.SpinSlash || AITimer < 80)
                    NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 5 * frameHeight;
            }

            switch (CustomBodyAni)
            {
                default:
                    BodyFrame = NPC.frame.Y;
                    break;
                case 1:
                    if (CustomBodyCounter++ == 0)
                        BodyFrame = 4 * frameHeight;
                    if (CustomBodyCounter != 0 && CustomBodyCounter % 8 == 0)
                    {
                        BodyFrame -= frameHeight;
                        if (BodyFrame < 3 * frameHeight)
                        {
                            BodyFrame = 3 * frameHeight;
                            CustomBodyAni = 0;
                            CustomBodyCounter = 0;
                        }
                    }
                    break;
            }
            if (HoldIcefall)
                BodyFrame = 2 * frameHeight;
        }
        public void SightCheck()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            var attacker = globalNPC.attacker;

            int gotNPC = GetNearestNPC(NPC);
            if (player.MinionAttackTargetNPC != -1)
                gotNPC = player.MinionAttackTargetNPC;
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], 1400, false, true) || gotNPC == player.MinionAttackTargetNPC))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item) => dodgeCooldown <= 20 ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => dodgeCooldown <= 20 ? null : false;
        public override bool CanBeHitByNPC(NPC attacker) => dodgeCooldown <= 20;
        private bool customArm;
        public float customArmRot;
        private bool HoldIcefall;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool noSpiritEffect = NoSpiritEffect(NPC);
            Color color = noSpiritEffect ? drawColor : Color.White;
            if (!noSpiritEffect)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, BodyFrame, NPC.frame.Width, NPC.frame.Height);

            spriteBatch.Draw(Calavia.CloakTex.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(color) * (noSpiritEffect ? 1f : .5f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.LegsTex.Value, NPC.Center + new Vector2(2 * NPC.spriteDirection, 0) - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HoldIcefall)
                spriteBatch.Draw(TextureAssets.Item[ItemType<Icefall>()].Value, NPC.Center + new Vector2(14 * NPC.spriteDirection, 0) - screenPos, null, NPC.GetAlpha(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (customArm)
                spriteBatch.Draw(Calavia.ArmTex2.Value, NPC.Center + new Vector2(-8 * NPC.spriteDirection, 0) - screenPos, null, NPC.ColorTintedAndOpacity(color), NPC.rotation + customArmRot, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            else
                spriteBatch.Draw(Calavia.ArmTex.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.ShoulderTex.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.ColorTintedAndOpacity(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            float fade = dodgeCooldown / 40f;
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(Calavia.CloakTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                Main.spriteBatch.Draw(Calavia.LegsTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(2 * NPC.spriteDirection, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                if (customArm)
                    spriteBatch.Draw(Calavia.ArmTex2.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY) + new Vector2(-8 * NPC.spriteDirection, 0), null, NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i] + customArmRot, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                else
                    spriteBatch.Draw(Calavia.ArmTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(Calavia.ShoulderTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), NPC.GetAlpha(color) * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            if (!noSpiritEffect)
            {
                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using System.IO;
using ParticleLibrary;
using Redemption.Dusts;
using Redemption.Particles;
using Terraria.Graphics.Shaders;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class AncientGladestoneGolem_SS : SSBase
    {
        public override string Texture => "Redemption/NPCs/PreHM/AncientGladestoneGolem";
        public enum ActionState
        {
            Idle,
            Wander,
            Threatened,
            PillarAttack,
            PillarJump,
            SoulMove
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Ancient Gladestone Golem");
            Main.npcFrameCount[NPC.type] = 12;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
        }
        public override void SetSafeDefaults()
        {
            NPC.width = 54;
            NPC.height = 80;
            NPC.damage = 35;
            NPC.defense = 20;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.RedemptionGuard().GuardPoints = NPC.lifeMax / 4;
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
                SoundEngine.PlaySound(SoundID.Zombie63, NPC.position);
                AITimer = 0;
                AIState = ActionState.Threatened;
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.DD2_WitherBeastCrystalImpact, .1f, false, DustID.DungeonSpirit, default, 20, 2, 10);
            }
            else
                modifiers.FinalDamage *= 2;

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
                writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                moveTo = reader.ReadVector2();
        }
        public override void ModifyTypeName(ref string typeName)
        {
            if (NPC.ai[3] != -1)
                typeName = Main.player[(int)NPC.ai[3]].name + "'s Ancient Gladestone Golem";
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public NPC npcTarget;
        public Vector2 moveTo;
        public int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[(int)NPC.ai[3]];
            NPC.damage = (int)(NPC.damage * player.GetTotalDamage(DamageClass.Summon).Additive);
            TimerRand = Main.rand.Next(120, 280);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand || NPC.DistanceSQ(player.Center) > 200 * 200)
                    {
                        moveTo = NPC.FindGround(15);
                        if (NPC.DistanceSQ(player.Center) > 200 * 200)
                            moveTo = (player.Center + new Vector2(20 * NPC.RightOfDir(player), 0)) / 16;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    if (NPC.lavaWet && Main.rand.NextBool(250) && NPC.velocity.Y == 0)
                    {
                        AITimer = 0;
                        NPC.frame.Y = 0;
                        AIState = ActionState.PillarJump;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Wander:
                    SightCheck();
                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 280);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 30, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.1f, 1, 10, 2, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Threatened:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 800, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 30, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.1f, 3, 10, 1, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    NPC.DamageHostileAttackers(0, 7);

                    if (Main.rand.NextBool(100) && NPC.velocity.Y == 0)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)globalNPC.attacker.Center.X / 16, (int)globalNPC.attacker.Center.Y / 16);
                        int dist = (tilePosY * 16) - (int)globalNPC.attacker.Center.Y;

                        NPC.frame.Y = 0;

                        if (NPC.DistanceSQ(globalNPC.attacker.Center) < 300 * 300 && dist < 140 && globalNPC.attacker.active)
                            AIState = ActionState.PillarAttack;
                        else
                            AIState = ActionState.PillarJump;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.PillarAttack:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                        AIState = ActionState.Wander;

                    NPC.velocity.X = 0;
                    break;

                case ActionState.PillarJump:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                        AIState = ActionState.Wander;
                    break;
                case ActionState.SoulMove:
                    NPC.alpha = 255;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    AITimer = 0;

                    ParticleManager.NewParticle(NPC.Center + RedeHelper.Spread(10) + NPC.velocity, Vector2.Zero, new SpiritParticle(), Color.White, 1f, 0, 1);
                    for (int i = 0; i < 2; i++)
                    {
                        int dust = Dust.NewDust(NPC.Center + NPC.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= .1f;
                        Color dustColor = new(188, 244, 227) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }

                    if (NPC.Hitbox.Intersects(player.Hitbox) && Collision.CanHit(NPC.Center, 0, 0, player.Center, 0, 0) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 2);
                            Main.dust[dust].velocity *= 2f;
                            Main.dust[dust].noGravity = true;
                        }

                        NPC.alpha = 0;
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;
                        NPC.velocity *= 0f;

                        moveTo = NPC.FindGround(20);
                        runCooldown = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.Move(player.Center - new Vector2(0, 20), 20, 20);
                    break;
            }
            if (AIState is not ActionState.SoulMove)
            {
                NPC.alpha += Main.rand.Next(-10, 11);
                NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                RedeNPC globalNPC = NPC.Redemption();
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
                switch (AIState)
                {
                    case ActionState.PillarAttack:
                        NPC.rotation = NPC.velocity.X * 0.05f;
                        NPC.frame.X = NPC.frame.Width;
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 5)
                        {
                            NPC.frame.Y += frameHeight;
                            NPC.frameCounter = 0;
                            if (NPC.frame.Y == frameHeight)
                            {
                                NPC.LookAtEntity(globalNPC.attacker);
                                SoundEngine.PlaySound(SoundID.Zombie64, NPC.position);
                                int tilePosY = BaseWorldGen.GetFirstTileFloor((int)globalNPC.attacker.Center.X / 16, (int)globalNPC.attacker.Center.Y / 16);
                                NPC.Shoot(new Vector2(globalNPC.attacker.Center.X, (tilePosY * 16) + 55), ModContent.ProjectileType<AncientGladestonePillar_SS>(), NPC.damage, Vector2.Zero);
                            }
                            if (NPC.frame.Y == 7 * frameHeight)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 6;
                            }
                            if (NPC.frame.Y > 9 * frameHeight)
                                AIState = ActionState.Threatened;
                        }
                        return;
                    case ActionState.PillarJump:
                        NPC.rotation = NPC.velocity.X * 0.05f;
                        NPC.frame.X = NPC.frame.Width;
                        if (NPC.frame.Y < 6 * frameHeight) { NPC.velocity.X = 0; }
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 5)
                        {
                            NPC.frame.Y += frameHeight;
                            NPC.frameCounter = 0;
                            if (NPC.frame.Y == frameHeight)
                            {
                                SoundEngine.PlaySound(SoundID.Zombie64, NPC.position);
                                int tilePosY = BaseWorldGen.GetFirstTileFloor((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16);
                                NPC.Shoot(new Vector2(NPC.Center.X, (tilePosY * 16) + 55), ModContent.ProjectileType<AncientGladestonePillar_SS>(), NPC.damage, Vector2.Zero);
                            }
                            if (NPC.frame.Y == 6 * frameHeight)
                                NPC.velocity.X += NPC.spriteDirection == 1 ? Main.rand.Next(2, 7) : Main.rand.Next(-7, -2);
                            if (NPC.frame.Y == 7 * frameHeight)
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 6;
                            }
                            if (NPC.frame.Y > 9 * frameHeight)
                                AIState = ActionState.Threatened;
                        }
                        return;
                }
                NPC.frame.X = 0;
                if (NPC.collideY || NPC.velocity.Y == 0)
                {
                    NPC.rotation = 0;
                    if (NPC.velocity.X == 0)
                        NPC.frame.Y = 0;
                    else
                    {
                        NPC.frameCounter += NPC.velocity.X * 0.5f;
                        if (NPC.frameCounter is >= 3 or <= -3)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 11 * frameHeight)
                                NPC.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    if (NPC.velocity.Y < 0)
                        NPC.frame.Y = 3 * frameHeight;
                    else
                        NPC.frame.Y = 10 * frameHeight;
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
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (target.friendly || target.lifeMax <= 5 || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])
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
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], 800, true, true) || gotNPC == player.MinionAttackTargetNPC))
            {
                SoundEngine.PlaySound(SoundID.Zombie63, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(15);
                AITimer = 0;
                AIState = ActionState.Threatened;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override bool CanHitNPC(NPC target) => AIState == ActionState.Threatened;
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.6f, 0.8f));
        }
    }
    public class AncientGladestonePillar_SS : AncientGladestonePillar
    {
        public override string Texture => "Redemption/Projectiles/Hostile/AncientGladestonePillar";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ancient Gladestone Pillar");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Summon;
            Projectile.hostile = false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && Projectile.velocity.Length() != 0 ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            Vector2 drawOrigin = new(Projectile.width / 2, Projectile.height / 2);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
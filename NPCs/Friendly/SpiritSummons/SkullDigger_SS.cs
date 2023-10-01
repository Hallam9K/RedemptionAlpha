using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using System.IO;
using Redemption.NPCs.Bosses.Keeper;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Globals.NPC;
using ParticleLibrary;
using Redemption.Dusts;
using Redemption.Particles;
using Redemption.NPCs.Minibosses.SkullDigger;
using System;

namespace Redemption.NPCs.Friendly.SpiritSummons
{
    public class SkullDigger_SS : SSBase
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger";
        public enum ActionState
        {
            Begin,
            Idle,
            Alert,
            Attacks,
            SoulMove
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public float[] oldrot = new float[5];
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            NPC.width = 60;
            NPC.height = 92;
            NPC.damage = 28;
            NPC.defense = 0;
            NPC.lifeMax = 2400;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.lavaImmune = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 4);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 2);

            if (AIState is ActionState.Idle)
                AIState = ActionState.Alert;
        }
        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(3f, 4f));
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
        }

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        private Vector2 origin;
        private int runCooldown;

        public List<int> AttackList = new() { 0, 1, 2 };
        public List<int> CopyList = null;

        public int ID;
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[(int)NPC.ai[3]];
            NPC.damage = (int)(NPC.damage * player.GetTotalDamage(DamageClass.Summon).Additive);
            NPC.localAI[0] = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<SkullDigger_SS_FlailBlade>()))
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkullDigger_SS_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI);

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;
            bool fading = false;
            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);
                        NPC.velocity.Y = -6;
                    }
                    if (AITimer > 2)
                        NPC.alpha -= 2;

                    int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                    Main.dust[dustIndex].velocity.Y -= 10f;
                    Main.dust[dustIndex].velocity.X *= 0f;
                    Main.dust[dustIndex].noGravity = true;

                    NPC.velocity *= 0.96f;
                    if (NPC.alpha <= 30)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(player.Center) >= 200 * 200)
                        NPC.Move(player.Center, 2, 20);
                    else
                        NPC.velocity *= .96f;

                    AITimer++;
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer >= NPC.localAI[0])
                            {
                                fading = true;
                                NPC.alpha += 5;
                            }
                            if (NPC.alpha >= 255)
                            {
                                NPC.velocity *= 0f;
                                NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 30);
                                TimerRand = 1;
                            }
                            break;
                        case 1:
                            NPC.alpha -= 5;
                            if (NPC.alpha <= 30)
                            {
                                NPC.localAI[0] = Main.rand.Next(80, 120);
                                AITimer = 0;
                                TimerRand = 0;
                            }
                            break;
                    }
                    SightCheck();
                    break;
                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 380))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Idle;
                        AITimer = 0;
                        TimerRand = 0;
                        NPC.localAI[0] = 5;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);

                    if (!NPC.Sight(globalNPC.attacker, 800, false, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.Move(globalNPC.attacker.Center, 2, 20);
                    AITimer++;
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer >= 5)
                            {
                                fading = true;
                                NPC.alpha += 5;
                            }
                            if (NPC.alpha >= 255)
                            {
                                NPC.velocity *= 0f;
                                NPC.position = new Vector2(Main.rand.NextBool(2) ? globalNPC.attacker.Center.X - 180 : globalNPC.attacker.Center.X + 180, globalNPC.attacker.Center.Y - 30);
                                TimerRand = 1;
                            }
                            break;
                        case 1:
                            NPC.alpha -= 5;
                            if (NPC.alpha <= 30)
                            {
                                AttackChoice();
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = ActionState.Attacks;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
                case ActionState.Attacks:
                    if (NPC.ThreatenedCheck(ref runCooldown, 380))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Idle;
                        AITimer = 0;
                        TimerRand = 0;
                        NPC.localAI[0] = 5;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);
                    switch (ID)
                    {
                        #region Flail Throw
                        case 0:
                            NPC.Move(globalNPC.attacker.Center, 2, 20);
                            if (AITimer >= 1)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Soul Charge
                        case 1:
                            AITimer++;
                            if (AITimer < 100)
                            {
                                NPC.MoveToVector2(new Vector2(globalNPC.attacker.Center.X - 160 * NPC.spriteDirection, globalNPC.attacker.Center.Y - 70), 3);
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 1);
                                    dust2.velocity = -NPC.DirectionTo(dust2.position);
                                    dust2.noGravity = true;
                                }
                                origin = globalNPC.attacker.Center;
                            }
                            if (AITimer >= 100 && AITimer < 120)
                            {
                                NPC.velocity.Y = 0;
                                NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 3);

                                if (AITimer % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = .5f }, NPC.position);
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(Main.rand.NextFloat(10, 12), (origin - NPC.Center).ToRotation()), ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), 3, player.whoAmI, 1);
                                    Main.projectile[p].hostile = false;
                                    Main.projectile[p].friendly = true;
                                    Main.projectile[p].DamageType = DamageClass.Summon;
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
                            if (AITimer >= 120)
                                NPC.velocity *= 0.98f;
                            if (AITimer >= 160)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Alert;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Flail Speen
                        case 2:
                            NPC.Move(globalNPC.attacker.Center, 2, 20);
                            if (AITimer >= 1)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.SoulMove:
                    NPC.alpha = 255;
                    AITimer = 0;

                    ParticleManager.NewParticle(NPC.Center + RedeHelper.Spread(10) + NPC.velocity, Vector2.Zero, new SpiritParticle(), Color.White, 4, 0, 1);
                    for (int i = 0; i < 2; i++)
                    {
                        int dust = Dust.NewDust(NPC.Center + NPC.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 4f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= .1f;
                        Color dustColor = new(188, 244, 227) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }

                    if (NPC.Hitbox.Intersects(player.Hitbox))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, 0, 0, Scale: 4);
                            Main.dust[dust].velocity *= 2f;
                            Main.dust[dust].noGravity = true;
                        }

                        NPC.alpha = 0;
                        NPC.velocity *= 0f;

                        runCooldown = 0;
                        NPC.localAI[0] = Main.rand.Next(80, 120);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.Move(player.Center, 20, 20);
                    break;
            }
            if (!fading && AIState is not ActionState.Begin && AIState is not ActionState.SoulMove)
            {
                if (NPC.alpha <= 30)
                {
                    NPC.alpha += Main.rand.Next(-10, 11);
                    NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
                }
            }
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }
        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == ModContent.NPCType<KeeperSpirit>() || target.type == ModContent.NPCType<Keeper>() || target.type == NPCID.TargetDummy)
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
            if (gotNPC != -1 && (NPC.Sight(Main.npc[gotNPC], 800, false, false) || gotNPC == player.MinionAttackTargetNPC))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AIState = ActionState.Alert;
                AITimer = 0;
                TimerRand = 0;
                NPC.localAI[0] = 5;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
    }
    public class SkullDigger_SS_FlailBlade : SkullDigger_FlailBlade
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_FlailBlade";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        private float rot;
        private float length;
        private float speed;
        public override bool PreAI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<SkullDigger_SS>())
                Projectile.Kill();

            Vector2 originPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
            Vector2 defaultPosition = new(host.Center.X + (host.spriteDirection == 1 ? 35 : -35), host.Center.Y + 60);
            if (host.alpha >= 255)
                Projectile.Center = defaultPosition;

            Player player = Main.player[(int)host.ai[3]];
            RedeNPC globalNPC = host.Redemption();
            Projectile.timeLeft = 10;

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = originPos;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) + 1.57f;
            Projectile.alpha = host.alpha;

            if (host.ai[0] == 3)
            {
                switch ((host.ModNPC as SkullDigger_SS).ID)
                {
                    case 0:
                        switch (Projectile.localAI[1])
                        {
                            case 0:
                                rot = originPos.ToRotation();
                                length = Projectile.Distance(originPos);
                                speed = MathHelper.ToRadians(2);
                                Projectile.localAI[1] = 1;
                                Projectile.netUpdate = true;
                                break;
                            case 1:
                                Projectile.localAI[0]++;
                                if (Projectile.localAI[0] >= 40 && Projectile.localAI[0] % 20 == 0 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                rot += speed * host.spriteDirection;
                                speed *= 1.04f;
                                speed = MathHelper.Clamp(speed, MathHelper.ToRadians(2), MathHelper.ToRadians(25));
                                Projectile.Center = originPos + new Vector2(0, 1).RotatedBy(rot) * length;
                                if (Projectile.localAI[0] >= 120)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                    Projectile.velocity = RedeHelper.PolarVector(14 + (Projectile.Distance(globalNPC.attacker.Center) / 30), (globalNPC.attacker.Center - Projectile.Center).ToRotation());
                                    host.velocity = RedeHelper.PolarVector(14, (globalNPC.attacker.Center - host.Center).ToRotation());
                                    Projectile.localAI[0] = 0;
                                    Projectile.localAI[1] = 2;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 2:
                                Projectile.localAI[0]++;
                                Projectile.velocity *= 0.97f;
                                if (Projectile.velocity.Length() < 5)
                                {
                                    Projectile.localAI[1] = 3;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 3:
                                Projectile.Move(defaultPosition, 20, 20);
                                if (Projectile.DistanceSQ(defaultPosition) < 50 * 50)
                                {
                                    host.ai[1] = 1;
                                    Projectile.netUpdate = true;
                                }
                                break;
                        }
                        break;
                    case 1:
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 0;
                        Projectile.Move(defaultPosition, 9, 20);
                        break;
                    case 2:
                        switch (Projectile.localAI[1])
                        {
                            case 0:
                                rot = originPos.ToRotation();
                                length = Projectile.Distance(originPos);
                                speed = MathHelper.ToRadians(1);
                                Projectile.localAI[1] = 1;
                                Projectile.netUpdate = true;
                                break;
                            case 1:
                                Projectile.localAI[0]++;
                                if (Projectile.localAI[0] % 50 == 0 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                length++;
                                length = MathHelper.Clamp(length, 10, 100);
                                rot += speed;
                                speed *= 1.02f;
                                speed = MathHelper.Clamp(speed, MathHelper.ToRadians(2), MathHelper.ToRadians(8));
                                Projectile.Center = originPos + new Vector2(0, 1).RotatedBy(rot) * length;
                                if (Projectile.localAI[0] >= 60 && Projectile.localAI[0] % 15 == 0 && Main.myPlayer == player.whoAmI)
                                {
                                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(0.08f, (globalNPC.attacker.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<SkullDigger_FlailBlade_ProjF>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, host.whoAmI);
                                    Main.projectile[p].localAI[1] = 1;
                                    Main.projectile[p].DamageType = DamageClass.Summon;
                                    Main.projectile[p].netUpdate = true;
                                }
                                if (Projectile.localAI[0] >= 260)
                                {
                                    Projectile.localAI[0] = 0;
                                    Projectile.localAI[1] = 2;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 2:
                                Projectile.Move(defaultPosition, 20, 20);
                                if (Projectile.DistanceSQ(defaultPosition) < 50 * 50)
                                {
                                    host.ai[1] = 1;
                                    Projectile.netUpdate = true;
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                Projectile.localAI[0] = 0;
                Projectile.localAI[1] = 0;
                Projectile.Move(defaultPosition, 9, 20);
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override bool? CanHitNPC(NPC target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return !target.friendly && host.ai[0] == 3 && (host.ModNPC as SkullDigger_SS).ID != 1 ? null : false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            NPC host = Main.npc[(int)Projectile.ai[0]];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Chain").Value;
            Vector2 HeadPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
            Rectangle sourceRectangle = new(0, 0, chainTexture.Width, chainTexture.Height);
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            float num1 = chainTexture.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            var effects = host.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Main.EntitySpriteDraw(chainTexture, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), Projectile.GetAlpha(lightColor), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Projectile.GetAlpha(Color.LightCyan) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ballTexture, drawPos, new Rectangle?(rect), color, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}

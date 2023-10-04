using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.NPCs.Friendly
{
    public class WraithSlayer_Samurai : ModNPC
    {
        public float[] oldrot = new float[4];
        public enum ActionState
        {
            Begin,
            Idle,
            Alert,
            Slash
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cursed Samurai");
            Main.npcFrameCount[NPC.type] = 15;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
            NPCID.Sets.DontDoHardmodeScaling[NPC.type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 70;
            NPC.damage = 300;
            NPC.friendly = true;
            NPC.defense = 24;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.RedemptionGuard().GuardPoints = 300;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Wraith, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.PurpleTorch, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/WraithSamuraiGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Wraith, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle)
            {
                SoundEngine.PlaySound(SoundID.Zombie81, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.NPCHit4, .25f, false, DustID.Wraith, default, 10, 1, 150);
            }
        }
        private Vector2 moveTo;
        private int runCooldown;
        public override void AI()
        {
            CustomFrames(86);

            Player player = Main.player[(int)NPC.ai[3]];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Slash)
                NPC.LookByVelocity();

            if (NPC.life <= 1)
            {
                NPC.alpha += 5;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
            else if (TimerRand++ % 10 == 0)
                NPC.life--;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie82, NPC.position);
                        NPC.velocity.Y = -4;
                        Flare = true;
                    }
                    if (NPC.alpha > 0)
                        NPC.alpha -= 10;
                    NPC.velocity.Y *= 0.98f;
                    if (AITimer >= 30)
                    {
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Idle:
                    if (AITimer++ % 120 == 0)
                    {
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        moveTo.X = (float)(Math.Sin(angle) * 200);
                        moveTo.Y = (float)(Math.Cos(angle) * 100);
                    }
                    NPC.Move(player.Center + new Vector2(moveTo.X, moveTo.Y - 100), 9, 50);

                    SightCheck();
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 4))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Idle;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 600, false, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;
                    moveTo = globalNPC.attacker.Center + new Vector2(NPC.RightOfDir(globalNPC.attacker) * 80, -20);
                    NPC.Move(moveTo, 16, 10);
                    if (NPC.DistanceSQ(moveTo) < 20 * 20)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity *= 0;
                        AIState = ActionState.Slash;
                    }
                    break;

                case ActionState.Slash:
                    NPC.velocity.X *= 0.5f;
                    AITimer = 0;
                    break;
            }
        }
        private void CustomFrames(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (Flare)
            {
                NPC.immortal = true;
                FlareTimer++;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }
            else
                NPC.immortal = false;

            if (AIState is ActionState.Slash)
            {
                NPC.rotation = 0;
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 5 * frameHeight)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Slice1, NPC.position);
                    }
                    if (NPC.frame.Y == 8 * frameHeight)
                    {
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;

                        NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y + 15), ModContent.ProjectileType<WraithSlayer_Slash>(), 0, new Vector2(20 * NPC.spriteDirection, 0), SoundID.Item71);
                        Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 280 : NPC.Center.X - 18), (int)NPC.Center.Y, 280, 60);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || target.friendly || target.lifeMax <= 5 || target.type == Type || target.whoAmI == NPC.whoAmI)
                                continue;

                            if (!target.Hitbox.Intersects(SlashHitbox))
                                continue;

                            BaseAI.DamageNPC(target, NPC.damage, 8, NPC.spriteDirection, NPC);
                        }
                        Flare = true;
                        FlareTimer = 0;
                        NPC.velocity.X += 280 * NPC.spriteDirection;
                    }
                    if (NPC.frame.Y > 14 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Alert;
                    }
                }
                return;
            }
            if (floatOffset.Y < 10)
                floatOffset.Y += 0.05f;
            else
                floatOffset.Y -= 0.05f;
        }
        private bool Flare;
        private float FlareTimer;
        private Vector2 floatOffset;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Slash)
                return;

            NPC.rotation = NPC.velocity.X * 0.05f;
            if (NPC.frameCounter++ >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }
        public void SightCheck()
        {
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = RedeHelper.GetNearestNPC(NPC.Center, canBeChasedBy: true);
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 600, false, false))
            {
                SoundEngine.PlaySound(SoundID.Zombie81, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.End();
            spriteBatch.BeginAdditive();

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f + new Vector2(21 * NPC.spriteDirection, 0) + floatOffset - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.MediumPurple), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(21 * NPC.spriteDirection, 0) + floatOffset - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(-2 * NPC.spriteDirection, -12 + NPC.gfxOffY);
                Vector2 position2 = NPC.Center - screenPos + new Vector2(4 * NPC.spriteDirection, -12 + NPC.gfxOffY);
                if (AIState is ActionState.Slash)
                {
                    position = NPC.Center - screenPos + new Vector2(6 * NPC.spriteDirection, -4 + NPC.gfxOffY);
                    position2 = NPC.Center - screenPos + new Vector2(12 * NPC.spriteDirection, -4 + NPC.gfxOffY);
                }
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.DarkRed, NPC.rotation, .8f);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position2, Color.DarkRed, NPC.rotation, .8f);

            }
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }
    public class WraithSlayer_Slash : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slash");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.hide = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.scale = 0.75f;
            Projectile.alpha = 0;
            Projectile.extraUpdates = 3;
        }
        public void DoTrailCreation(TrailManager tM) => tM.CreateTrail(Projectile, new StandardColorTrail(new Color(159, 127, 170, 200)), new RoundCap(), new DefaultTrailPosition(), 100f, 800f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        public override void AI()
        {
            Projectile.alpha += 20;
            if (Projectile.alpha > 255)
                Projectile.Kill();

            if (Projectile.velocity.Length() < 26)
                Projectile.velocity *= 1.02f;
        }
    }
}
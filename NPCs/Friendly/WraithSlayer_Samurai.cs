using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.Minions;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.NPCs.Friendly.SpiritSummons;
using Redemption.Particles;
using Redemption.Projectiles;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly
{
    public class WraithSlayer_Samurai : ModNPC
    {
        public float[] oldrot = new float[4];
        public Vector2[] oldpos = new Vector2[4];
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
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
            NPCID.Sets.DontDoHardmodeScaling[NPC.type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 66;
            NPC.height = 74;
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
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, Find<ModGore>("Redemption/WraithSamuraiGore" + (i + 1)).Type, 1);
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
            if (!NPC.RedemptionGuard().GuardBroken)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.NPCHit4, .25f, false, DustID.Wraith, default, 10, 1, 150);
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        public override void AI()
        {
            Player player = Main.player[(int)NPC.ai[3]];
            if (!player.active || player.dead || !player.HasBuff<CursedSamuraiBuff>())
                NPC.life = 1;

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
                        moveTo.X = (float)(Math.Sin(angle) * 140);
                        moveTo.Y = (float)(Math.Cos(angle) * 80);
                    }
                    NPC.Move(player.Center + new Vector2(moveTo.X, moveTo.Y - 80), 9, 50);

                    SightCheck();
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 4) || globalNPC.attacker is Player)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Idle;
                        break;
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
            CustomFrames();
        }
        private void CustomFrames()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
                oldpos[k] = oldpos[k - 1];
            }
            oldrot[0] = NPC.rotation;
            oldpos[0] = NPC.Center;

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
                if (NPC.frame.Y < 4)
                    NPC.frame.Y = 4;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y++;
                    if (NPC.frame.Y == 5)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Slice1, NPC.position);
                    }
                    if (NPC.frame.Y == 8)
                    {
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 5;

                        Flare = true;
                        FlareTimer = 0;
                        NPC.velocity.X += 100 * NPC.spriteDirection;
                    }
                    if (NPC.frame.Y is 8 or 9)
                    {
                        int extend = 50;
                        Rectangle SlashHitbox = new((int)NPC.Center.X - 154, (int)NPC.Center.Y - 106, 154 + extend, 154);
                        if (NPC.spriteDirection == 1)
                            SlashHitbox.X += SlashHitbox.Width - (extend * 2);
                        NPC.RedemptionHitbox().DamageInHitbox(NPC, 0, SlashHitbox, NPC.damage, 8);
                    }
                    if (NPC.frame.Y > 15)
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
                NPC.frame.Y++;
                if (NPC.frame.Y > 3)
                    NPC.frame.Y = 0;
            }
        }
        public void SightCheck()
        {
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = SSBase.GetNearestNPC(NPC);
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 600, false, false))
            {
                SoundEngine.PlaySound(SoundID.Zombie81, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Spirit.Contains(target.type))
                modifiers.FinalDamage *= 2;
        }
        public bool strike;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if (!strike)
            {
                SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = -.2f }, target.position);
                strike = true;
            }

            Vector2 dir = NPC.Center.DirectionFrom(target.Center);
            Vector2 drawPos = Vector2.Lerp(NPC.Center, target.Center, 0.9f);
            RedeParticleManager.CreateSlashParticle(drawPos, dir.RotatedBy(0.4f * -1 * NPC.spriteDirection) * 80, 1, Color.MediumPurple, 8);
            for (int i = 0; i < 5; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.4f, 0.4f);
                float randomVel = Main.rand.NextFloat(2f, 3f);
                Vector2 direction = target.Center.DirectionFrom(NPC.Center);
                Vector2 position = target.Center - direction * 30;
                RedeParticleManager.CreateSpeedParticle(position, direction.RotatedBy(randomRotation) * randomVel * 12, .8f, Color.MediumPurple.WithAlpha(0));
            }

            int damageDone = hit.Damage;
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
        }

        Asset<Texture2D> slashTex;
        Asset<Texture2D> slashEffectTex;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            slashTex ??= Request<Texture2D>(Texture + "_Slash");
            slashEffectTex ??= Request<Texture2D>(Texture + "_SlashEffect");

            Rectangle rect = texture.Frame(1, 4, 0, NPC.frame.Y);
            Vector2 origin = rect.Size() / 2 + new Vector2(21 * NPC.spriteDirection, -6);

            if (NPC.frame.Y >= 4)
            {
                texture = slashTex;
                rect = texture.Frame(1, 12, 0, NPC.frame.Y - 4);
                origin = rect.Size() / 2 + new Vector2(21 * NPC.spriteDirection, 16);
            }

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.End();
            spriteBatch.BeginAdditive();

            Vector2 slashEffectOrigin = slashEffectTex.Size() / 2 + new Vector2(-51 * NPC.spriteDirection, 29);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                spriteBatch.Draw(texture.Value, oldpos[i] + floatOffset - screenPos, rect, NPC.GetAlpha(Color.MediumPurple), oldrot[i], origin, NPC.scale, effects, 0);
            }

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(texture.Value, NPC.Center + floatOffset - screenPos, rect, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);

            if (NPC.frame.Y == 8)
            {
                spriteBatch.Draw(slashEffectTex.Value, NPC.Center + floatOffset - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, slashEffectOrigin, NPC.scale, effects, 0);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(10 * NPC.spriteDirection, -10);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.DarkRed, NPC.rotation, .8f);
            }
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }
}
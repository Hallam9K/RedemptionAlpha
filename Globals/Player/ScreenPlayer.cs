using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.WorldGeneration.Misc;
using ReLogic.Content;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption
{
    public class ScreenPlayer : ModPlayer
    {
        public int rumbleDuration;
        public int rumbleStrength;
        public int interpolantTimer;
        public bool lockScreen = false;
        public bool cutscene;
        public bool cutsceneEnd;
        public override void Initialize()
        {
            NebCutsceneflag = false;
            NebCutscene = false;
        }
        public static bool NebCutscene;
        public static bool NebCutsceneflag = false;
        public Vector2 setCurrentVecScreenPos;
        float up;
        public float yeet;
        public float yeet2;
        public override void PostUpdate()
        {
            if (RedeConfigClient.Instance.CameraLockDisable)
                cutscene = false;
            if (cutscene)
            {
                cutsceneEnd = true;
                WorldGen.spawnEye = false;
                WorldGen.spawnHardBoss = 0;
                RedeWorld.spawnKeeper = false;
            }
            if (rumbleDuration > 0)
            {
                rumbleDuration--;
            }
            if (lockScreen)
            {
                if (interpolantTimer < 100) interpolantTimer += 1;
            }
            else
            {
                if (interpolantTimer > 0) interpolantTimer -= 1;
            }
            ScreenFocusInterpolant = Utils.GetLerpValue(15f, 80f, interpolantTimer, true);
            lockScreen = false;
            if (cutsceneEnd && !cutscene)
            {
                Player.immune = true;
                Player.immuneTime = 120;
                cutsceneEnd = false;
            }
            cutscene = false;
            customZoom = 0;
            if (SubworldSystem.IsActive<CSub>())
                customZoom = 2f;
        }
        public override void UpdateEquips()
        {
            if (cutscene && !RedeConfigClient.Instance.CameraLockDisable)
            {
                for (int p = 0; p < Main.maxNPCs; p++)
                {
                    NPC target = Main.npc[p];
                    if (!target.active || target.boss || target.friendly || target.knockBackResist <= 0)
                        continue;

                    if (Player.DistanceSQ(target.Center) >= 500 * 500)
                        continue;

                    target.velocity -= RedeHelper.PolarVector(0.3f, (Player.Center - target.Center).ToRotation());
                }
                Player.wingTime = Player.wingTimeMax;
            }
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            return cutsceneEnd;
        }
        public override bool CanUseItem(Item item)
        {
            if (cutscene && item.damage > 0 && !RedeConfigClient.Instance.CameraLockDisable)
                return false;
            return base.CanUseItem(item);
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (cutscene && !RedeConfigClient.Instance.CameraLockDisable)
            {
                Player.statLife = 1;
                return false;
            }
            return true;
        }
        public override void UpdateDead()
        {
            lockScreen = false;
            rumbleDuration = 0;
            NebCutsceneflag = false;
            NebCutscene = false;
        }
        /// <summary>
        /// Shakes the screen.
        /// </summary>
        /// <param name="duration">How long the screen should shake</param>
        /// <param name="intensity">How strong the screen should shake, default is 10.</param>
        public void Rumble(int duration, int intensity = 10)
        {
            rumbleDuration = duration;
            rumbleStrength = intensity;
        }

        public float ScreenShakeIntensity;
        public Vector2 ScreenShakeOrigin = Vector2.Zero;
        public Vector2 ScreenFocusPosition;
        public float ScreenFocusInterpolant;

        public Vector2 timedZoom = Vector2.Zero;
        public float timedZoomTime = 0;
        public float timedZoomTimeMax = 0;
        public float timedZoomDuration = 0;
        public float timedZoomDurationMax = 0;
        public float customZoom;
        public void TimedZoom(Vector2 zoom, int zoomTime, int zoomDuration)
        {
            timedZoom = zoom;
            timedZoomTimeMax = zoomTime;
            timedZoomDurationMax = zoomDuration;
        }
        public override void PostUpdateMiscEffects()
        {
            if (timedZoomDurationMax > 0 && timedZoom != Vector2.Zero)
            {
                if (timedZoomDuration == 0)
                {
                    if (timedZoomTime < timedZoomTimeMax)
                        timedZoomTime++;
                    else if (timedZoomTime == timedZoomTimeMax)
                        timedZoomDuration = 1;
                }
                else if (timedZoomDuration >= 1 && timedZoomDuration < timedZoomDurationMax)
                    timedZoomDuration++;
                else if (timedZoomDuration == timedZoomDurationMax)
                {
                    if (timedZoomTime > 0)
                        timedZoomTime--;
                    else if (timedZoomTime == 0)
                    {
                        timedZoomTime = 0;
                        timedZoomTimeMax = 0;
                        timedZoomDuration = 0;
                        timedZoomDurationMax = 0;

                        timedZoom = Vector2.Zero;
                    }
                }
            }
        }
        public enum CutscenePriority : byte
        {
            None,
            Low,
            Medium,
            High,
            Max
        }
        public static CutscenePriority CurrentCutscenePriority;
        public static void CutsceneLock(Player player, Entity focus, CutscenePriority priority, int cutsceneRange = 600, int focusRange = 1200, int vignetteRange = 600)
        {
            CutsceneLock(player, focus.Center, priority, cutsceneRange, focusRange, vignetteRange);
        }
        public static void CutsceneLock(Player player, Vector2 focus, CutscenePriority priority, int cutsceneRange = 600, int focusRange = 1200, int vignetteRange = 600, bool reverse = false, bool noLockMoveRadius = false)
        {
            if (RedeConfigClient.Instance.CameraLockDisable)
                return;
            CurrentCutscenePriority = priority;
            if (cutsceneRange == 0 || focus.DistanceSQ(player.Center) <= cutsceneRange * cutsceneRange)
            {
                if (priority >= CutscenePriority.High)
                {
                    player.RedemptionScreen().ScreenFocusPosition = focus;
                    if (!noLockMoveRadius)
                        NPCHelper.LockMoveRadius(focus, player);
                }
                else
                {
                    if (reverse)
                        player.RedemptionScreen().ScreenFocusPosition = Vector2.Lerp(player.Center, focus, player.DistanceSQ(focus) / (focusRange * focusRange));
                    else
                        player.RedemptionScreen().ScreenFocusPosition = Vector2.Lerp(focus, player.Center, player.DistanceSQ(focus) / (focusRange * focusRange));
                }
                player.RedemptionScreen().lockScreen = true;
                if (priority >= CutscenePriority.Low)
                    player.RedemptionScreen().cutscene = true;
                if (vignetteRange is 0 || player.RedemptionAbility().SpiritwalkerActive)
                    return;
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(MathHelper.Lerp(1, 0, player.DistanceSQ(focus) / (vignetteRange * vignetteRange))).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                player.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
            }
        }
        public override void ModifyScreenPosition()
        {
            ADDScreenLock();
            if (ScreenFocusInterpolant > 0f && !RedeConfigClient.Instance.CameraLockDisable)
            {
                Vector2 idealScreenPosition = ScreenFocusPosition - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                Main.screenPosition = Vector2.Lerp(Main.screenPosition, idealScreenPosition, lockScreen ? EaseFunction.EaseCubicOut.Ease(ScreenFocusInterpolant) : EaseFunction.EaseCubicIn.Ease(ScreenFocusInterpolant));
            }
            Redemption.Instance.cameraOffset *= 0.9f;
            Main.screenPosition += Redemption.Instance.cameraOffset;
            if (rumbleDuration > 0)
            {
                int r = rumbleStrength;
                Main.screenPosition.X += Main.rand.Next(-r, r + 1);
                Main.screenPosition.Y += Main.rand.Next(-r, r + 1);
            }
            if (ScreenShakeIntensity > 0.1f)
            {
                if (ScreenShakeOrigin != Vector2.Zero)
                {
                    float dist = Player.Distance(ScreenShakeOrigin) / 400;
                    dist = MathHelper.Max(dist, 1);
                    ScreenShakeIntensity /= dist;
                }
                Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity),
                    Main.rand.NextFloat(ScreenShakeIntensity));

                ScreenShakeIntensity *= 0.9f;
                ScreenShakeOrigin = Vector2.Zero;
            }
            ScreenShakeIntensity = MathHelper.Clamp(ScreenShakeIntensity, 0, 200);

            if (Player.dead || !Player.active || (!NPC.AnyNPCs(ModContent.NPCType<Nebuleus>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus2>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus_Clone>()) && !NPC.AnyNPCs(ModContent.NPCType<Nebuleus2_Clone>())))
            {
                NebCutscene = false;
                NebCutsceneflag = false;
                yeet2 = 0;
                yeet = 0;
                return;
            }
            if (NebCutscene)
            {
                if (!NebCutsceneflag)
                {
                    up = 0;
                    NebCutsceneflag = true;
                    setCurrentVecScreenPos = Player.Center;
                }
                yeet = Player.Center.Y - setCurrentVecScreenPos.Y;
                up += (400 - up) / 32f;
                yeet2 = up + yeet;
                Main.screenPosition.Y -= yeet2;
            }
            else
            {
                if (Math.Abs(yeet2) > 2)
                {
                    yeet2 -= yeet2 / 20f;
                    Main.screenPosition.Y -= yeet2;
                    if (Math.Abs(yeet2) > 20)
                    {
                        Main.screenPosition.X += Main.rand.Next(-10, 11);
                        Main.screenPosition.Y += Main.rand.Next(-10, 11);
                    }
                }
                NebCutsceneflag = false;
            }
        }
        public void ADDScreenLock()
        {
            Rectangle ADDscreen = NPC.AnyNPCs(ModContent.NPCType<Ukko>()) || NPC.AnyNPCs(ModContent.NPCType<Akka>()) ? new Rectangle((int)ArenaWorld.arenaTopLeft.X - 1000, (int)ArenaWorld.arenaTopLeft.Y - 1000, (int)ArenaWorld.arenaSize.X + 2000, (int)ArenaWorld.arenaSize.Y + 2000) : Rectangle.Empty;
            if (!ADDscreen.IsEmpty)
            {
                Vector2 pos = new(ADDscreen.Center.X, ADDscreen.Center.Y);
                float x = Player.Center.X;
                float y = Player.Center.Y;
                if (ADDscreen.Width > Main.screenWidth)
                    pos.X = MathHelper.Clamp(x, ADDscreen.X + Main.screenWidth / 2, ADDscreen.Right - Main.screenWidth / 2);
                if (ADDscreen.Height > Main.screenHeight)
                    pos.Y = MathHelper.Clamp(y, ADDscreen.Y + Main.screenHeight / 2, ADDscreen.Bottom - Main.screenHeight / 2);
                pos -= new Vector2(Main.screenWidth, Main.screenHeight) / 2;

                if (Redemption.Instance.currentScreen != ADDscreen)
                {
                    Redemption.Instance.cameraOffset = Main.screenPosition - pos;
                    Redemption.Instance.currentScreen = ADDscreen;
                }
                Main.screenPosition = pos;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Bosses.ADD;
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
            if (rumbleDuration > 0)
            {
                rumbleDuration--;
            }
            if (lockScreen)
            {
                if (interpolantTimer < 100) interpolantTimer += 2;
            }
            else
            {
                if (interpolantTimer > 0) interpolantTimer -= 2;
            }
            ScreenFocusInterpolant = Utils.GetLerpValue(15f, 80f, interpolantTimer, true);
            lockScreen = false;
            cutscene = false;
            customZoom = 0;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            return !cutscene;
        }
        public override bool CanUseItem(Item item)
        {
            return !cutscene;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (cutscene)
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
        public override void ModifyScreenPosition()
        {
            ADDScreenLock();
            if (ScreenFocusInterpolant > 0f && !RedeConfigClient.Instance.CameraLockDisable)
            {
                Vector2 idealScreenPosition = ScreenFocusPosition - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                Main.screenPosition = Vector2.Lerp(Main.screenPosition, idealScreenPosition, ScreenFocusInterpolant);
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
            Rectangle ADDscreen = NPC.AnyNPCs(ModContent.NPCType<Ukko>()) || NPC.AnyNPCs(ModContent.NPCType<Akka>()) ? new Rectangle((int)ArenaWorld.arenaTopLeft.X, (int)ArenaWorld.arenaTopLeft.Y, (int)ArenaWorld.arenaSize.X, (int)ArenaWorld.arenaSize.Y) : Rectangle.Empty;
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

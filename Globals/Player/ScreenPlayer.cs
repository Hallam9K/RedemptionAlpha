using Microsoft.Xna.Framework;
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
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
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
        public Vector2 ScreenFocusPosition;
        public float ScreenFocusInterpolant;

        public override void ModifyScreenPosition()
        {
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
                Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity),
                    Main.rand.NextFloat(ScreenShakeIntensity));

                ScreenShakeIntensity *= 0.9f;
            }
        }
    }
}

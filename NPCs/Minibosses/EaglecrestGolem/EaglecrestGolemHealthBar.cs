using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    public class EaglecrestGolemHealthBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            iconFrame = Rectangle.Empty;
            return null;
        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != NPCType<EaglecrestGolem>())
                return false;

            info.showText = false;
            lifeMax = npc.lifeMax;
            life = Utils.Clamp(npc.life, 0, lifeMax);
            if (npc.ModNPC is EaglecrestGolem golem && npc.frame.Y == 12 * 84 && npc.ai[0] != 2)
            {
                shieldMax = golem.GuardPointMax;
                shield = Utils.Clamp(npc.RedemptionGuard().GuardPoints, 0, shieldMax) * MathHelper.Lerp(0, 1, life / lifeMax);
            }
            else
            {
                shieldMax = 0;
                shield = 0;
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
        {
            Point p = new(456, 22);
            Rectangle rectangle = Utils.CenteredRectangle(Main.ScreenSize.ToVector2() * new Vector2(0.5f, 1f) + new Vector2(0f, -50f), p.ToVector2());
            if (BigProgressBarSystem.ShowText)
            {
                BigProgressBarHelper.DrawHealthText(spriteBatch, rectangle, Vector2.Zero, drawParams.Life, drawParams.LifeMax);

                if (drawParams.Shield <= 0)
                    return;
                DynamicSpriteFont value = FontAssets.ItemStack.Value;
                Vector2 vector = rectangle.Center.ToVector2() + new Vector2(0, 15);
                vector.Y += 1f;
                string text = "/";
                Color color = new(255, 228, 131);
                Vector2 vector2 = value.MeasureString(text);
                Utils.DrawBorderStringFourWay(spriteBatch, value, text, vector.X, vector.Y, color, Color.Black, vector2 * 0.5f, .9f);
                text = ((int)drawParams.Shield).ToString();
                vector2 = value.MeasureString(text);
                Utils.DrawBorderStringFourWay(spriteBatch, value, text, vector.X - 5f, vector.Y, color, Color.Black, vector2 * new Vector2(1f, 0.5f), .9f);
                text = ((int)drawParams.ShieldMax).ToString();
                vector2 = value.MeasureString(text);
                Utils.DrawBorderStringFourWay(spriteBatch, value, text, vector.X + 5f, vector.Y, color, Color.Black, vector2 * new Vector2(0f, 0.5f), .9f);
            }
        }
    }
}
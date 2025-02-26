using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using ReLogic.Content;
using Terraria;
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

            lifeMax = npc.lifeMax;
            life = Utils.Clamp(npc.life, 0, lifeMax);
            if (npc.ModNPC is EaglecrestGolem golem && npc.frame.Y == 12 * 84 && npc.ai[0] != 2)
            {
                shieldMax = golem.GuardPointMax;
                shield = Utils.Clamp(npc.RedemptionGuard().GuardPoints, 0, shieldMax);
            }
            else
            {
                shieldMax = 0;
                shield = 0;
            }
            return true;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACEHealthBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            iconFrame = Rectangle.Empty;
            return null;
        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != ModContent.NPCType<MACEProject>())
                return false;

            lifeMax = npc.lifeMax;
            life = Utils.Clamp(npc.life, 0, lifeMax);
            if (npc.ModNPC is MACEProject mace)
            {
                shieldMax = mace.GuardPointMax;
                shield = Utils.Clamp(npc.RedemptionGuard().GuardPoints, 0, shieldMax);
            }
            return true;
        }
    }
}
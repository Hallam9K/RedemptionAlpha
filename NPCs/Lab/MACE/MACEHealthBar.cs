using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACEHealthBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;
        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != ModContent.NPCType<MACEProject>())
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            life = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);
            if (npc.ModNPC is MACEProject mace)
            {
                shield = Utils.Clamp((float)npc.RedemptionGuard().GuardPoints / mace.GuardPointMax, 0f, 1f);
            }
            return true;
        }
    }
}
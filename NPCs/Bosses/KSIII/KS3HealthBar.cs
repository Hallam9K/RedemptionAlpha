using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3HealthBar : ModBossBar
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
            if (!npc.active || (npc.ModNPC is not KS3 && npc.ModNPC is not KS3_Clone))
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();
            lifeMax = npc.lifeMax;
            life = Utils.Clamp(npc.life, 0, lifeMax);
            if (npc.ModNPC is KS3 ks3 && ks3.phase >= 5)
            {
                shieldMax = lifeMax;
                shield = life;
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
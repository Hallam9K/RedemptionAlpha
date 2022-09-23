using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZHealthBar : ModBossBar
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
        public override string Texture => "Redemption/Textures/BossBars/InfectionBossBar";
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != ModContent.NPCType<PZ>())
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            if (npc.ModNPC is PZ)
            {
                int k = NPC.FindFirstNPC(ModContent.NPCType<PZ_Kari>());
                if (k > -1)
                {
                    NPC kari = Main.npc[k];
                    if (k > -1 && kari.active && kari.type == ModContent.NPCType<PZ_Kari>())
                        lifePercent = Utils.Clamp(kari.life / (float)kari.lifeMax, 0f, 1f);
                }
                else
                    lifePercent = 1f;
            }
            shieldPercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);
            if (npc.life <= npc.lifeMax * 0.1f)
                shieldPercent = 0;
            return true;
        }
    }
}
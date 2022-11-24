using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaHealthBar : ModBossBar
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
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != ModContent.NPCType<Akka>())
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            lifePercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);
            if (npc.ModNPC is Akka akka)
            {
                shieldPercent = Utils.Clamp((float)npc.RedemptionGuard().GuardPoints / akka.GuardPointMax, 0f, 1f);
            }
            return true;
        }
    }
    public class UkkoHealthBar : ModBossBar
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
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active || npc.type != ModContent.NPCType<Ukko>())
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            lifePercent = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);
            if (npc.ModNPC is Ukko ukko)
            {
                shieldPercent = Utils.Clamp((float)npc.RedemptionGuard().GuardPoints / ukko.GuardPointMax, 0f, 1f);
            }
            return true;
        }
    }
}
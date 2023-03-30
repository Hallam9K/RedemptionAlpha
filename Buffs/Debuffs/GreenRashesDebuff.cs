using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class GreenRashesDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Green Rashes");
            // Description.SetDefault("... Really itchy");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().greenRashes = true;
            player.statDefense -= 7;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen = -10;
        }
    }
}
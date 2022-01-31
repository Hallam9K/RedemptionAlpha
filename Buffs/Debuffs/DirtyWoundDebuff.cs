using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class DirtyWoundDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dirty Wound");
            Description.SetDefault("Better wash it");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().dirtyWound = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().dirtyWound = true;
        }
    }
}
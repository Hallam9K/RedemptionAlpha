using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class DevilScentedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil Scented");
            Description.SetDefault("You smell delicious");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.stinky = true;
            player.RedemptionPlayerBuff().devilScented = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.stinky = true;
            npc.RedemptionNPCBuff().devilScented = true;
        }
    }
}
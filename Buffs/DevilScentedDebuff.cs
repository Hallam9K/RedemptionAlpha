using Redemption.Globals.NPC;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
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
            LongerExpertDebuff = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.stinky = true;
            player.GetModPlayer<BuffPlayer>().devilScented = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.stinky = true;
            npc.GetGlobalNPC<BuffNPC>().devilScented = true;
        }
    }
}
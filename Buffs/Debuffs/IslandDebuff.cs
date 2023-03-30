using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class IslandDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            // Description.SetDefault("");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().island = true;
        }
    }
}
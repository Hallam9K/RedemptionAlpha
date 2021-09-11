using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class WellFed4 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Delightfully Indulged");
            Description.SetDefault("\"Massive improvements to all stats\"");
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {         
            player.GetModPlayer<BuffPlayer>().wellFed4 = true;
            player.moveSpeed *= 1.5f;
            player.statDefense += 6;
            player.meleeSpeed *= 1.2f;
            player.minionKB += 1.5f;
            player.pickSpeed *= 0.7f;    
        }
    }
}
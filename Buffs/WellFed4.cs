using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs
{
    public class WellFed4 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Delightfully Indulged");
            Description.SetDefault("Massive improvements to all stats");
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.IsWellFed[Type] = true;
            BuffID.Sets.IsFedState[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {         
            player.RedemptionPlayerBuff().wellFed4 = true;
            player.wellFed = true;
            player.moveSpeed *= 1.5f;
            player.statDefense += 6;
            player.meleeSpeed *= 1.2f;
            player.minionKB += 1.5f;
            player.pickSpeed *= 0.7f;    
        }
    }
}
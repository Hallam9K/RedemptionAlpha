using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
	public class RedeItem : GlobalItem
	{
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (item.axe > 0 && crit)
            {
                damage += damage / 2;
            }
        }
    }
}
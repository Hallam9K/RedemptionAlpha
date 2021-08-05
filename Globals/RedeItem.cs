using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption
{
	public class RedeItem : GlobalItem
	{
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (item.axe > 0 && crit)
            {
                damage += damage / 2;
            }
        }
    }
}
using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class BleachedSolution : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Used by the Clentaminator"
				+ "\nSpreads the Wasteland"); */
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.DefaultToSolution(ModContent.ProjectileType<BleachedSolution_Proj>());
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.Orange;
		}
	}
}
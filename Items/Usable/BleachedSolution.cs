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
			Tooltip.SetDefault("Used by the Clentaminator"
				+ "\nSpreads the Wasteland");
		}

		public override void SetDefaults()
		{
			Item.shoot = ModContent.ProjectileType<BleachedSolution_Proj>() - ProjectileID.PureSpray;
			Item.ammo = AmmoID.Solution;
			Item.width = 10;
			Item.height = 12;
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
			Item.consumable = true;
		}
	}
}
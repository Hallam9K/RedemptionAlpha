using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ammo
{
    public class XenomiteBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Xenomite Bullet");
			// Tooltip.SetDefault("Infects hit enemies");
		}
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 2f;
			Item.value = 1;
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<XenomiteBulletProj>();
			Item.shootSpeed = 4f;
			Item.ammo = AmmoID.Bullet;
		}
		public override void AddRecipes()
		{
			CreateRecipe(33)
			.AddIngredient(ItemID.MusketBall, 33)
			.AddIngredient(ModContent.ItemType<XenomiteShard>(), 1)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}

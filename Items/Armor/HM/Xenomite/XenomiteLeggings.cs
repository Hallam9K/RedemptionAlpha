using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Armor.HM.Xenomite
{
    [AutoloadEquip(EquipType.Legs)]
    public class XenomiteLeggings : ModItem
    {
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("4% increased damage and critical strike chance\n" +
				"8% increased movement speed"); */

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 12;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Generic) += .04f;
			player.GetCritChance(DamageClass.Generic) += 4;
			player.moveSpeed += 0.08f;
		}

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient(ItemID.TitaniumBar, 5)
				.AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 15)
				.AddTile(TileID.MythrilAnvil)
				.Register();
            CreateRecipe()
				.AddIngredient(ItemID.AdamantiteBar, 5)
				.AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 15)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
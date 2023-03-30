using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Legs)]
    public class HardlightBoots : ModItem
    {
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("5% increased damage\n" +
				"10% increased movement speed"); */

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.LightPurple;
			Item.defense = 13;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Generic) += .05f;
			player.moveSpeed += 0.1f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CyberPlating>(), 9)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
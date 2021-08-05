using Redemption.Items.Critters;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
	public class RoastLarva : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Medium improvements to all stats" +
				"\n'The forbidden croissant'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 18;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item2;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 5);
			Item.buffType = BuffID.WellFed2;
			Item.buffTime = 21600;
		}
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<GrandLarvaBait>())
				.AddTile(TileID.CookingPots)
				.Register();
		}
    }
}
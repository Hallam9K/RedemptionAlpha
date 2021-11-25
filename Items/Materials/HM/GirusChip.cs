using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameConent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
	public class GirusChip : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Girus Chip");
			Tooltip.SetDefault("'What is this?'");
			Main.RegisterItemAnimation(Item.type, (DrawAnimation)new DrawAnimationVertical(10, 10));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Red;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<AIChip>(), 1)
			.AddIngredient(ItemID.Ectoplasm, 5)
			.AddTile(ModContent.TileType<CorruptorTile>())
			.Register();
		}
	}
}

using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
	public class CorruptedXenomiteItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupted Xenomite");
			Tooltip.SetDefault("'Infects mechanical things...'");
			Main.RegisterItemAnimation(Item.type, (DrawAnimation)new DrawAnimationVertical(4, 4));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 24;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Red;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<XenomiteItem>(), 5)
			.AddIngredient(ModContent.ItemType<VlitchBattery>(), 1)
			.AddTile(ModContent.TileType<GirusCorruptorTile>())
			.Register();
		}
	}
}

using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class SkullSign : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SkullSignTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.value = 100;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<LabPlating>(), 6)
                .AddIngredient(ItemID.YellowPaint, 4)
                .AddIngredient(ItemID.BlackPaint, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
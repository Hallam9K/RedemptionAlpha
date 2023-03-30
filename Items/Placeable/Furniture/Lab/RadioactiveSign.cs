using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class RadioactiveSign : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadioactiveSignTile>(), 0);
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
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
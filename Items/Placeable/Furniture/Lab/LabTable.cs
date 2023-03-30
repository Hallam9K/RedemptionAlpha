using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabTable : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Table");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTableTile>(), 0);
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LabPlating>(), 12)
               .AddIngredient(ModContent.ItemType<Plating>(), 2)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}
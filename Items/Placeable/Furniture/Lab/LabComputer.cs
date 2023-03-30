using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabComputer : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Computer");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<LabComputerTile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 6)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
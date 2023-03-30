using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCeilingMonitor : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Ceiling Monitor");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCeilingMonitorTile>(), 0);
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 6)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
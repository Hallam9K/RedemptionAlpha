using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabReceptionMonitors : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Reception Monitors");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabReceptionMonitorsTile>(), 0);
            Item.width = 48;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 18)
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddIngredient(ModContent.ItemType<LabCeilingMonitor>(), 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
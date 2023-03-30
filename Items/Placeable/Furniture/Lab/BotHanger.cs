using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class BotHanger : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Occupied Bot Hanger");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<BotHangerTile>(), 0);
            Item.width = 30;
            Item.height = 42;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 12)
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
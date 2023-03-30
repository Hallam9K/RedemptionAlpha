using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class SlayerShipPanel : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Ship Panel");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

		public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SlayerShipPanelTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = Item.CommonMaxStack;
		}
    }
    public class SlayerShipPanel2 : ModItem
	{
        public override string Texture => "Redemption/Items/Placeable/Tiles/SlayerShipPanel";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Ship Panel");
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SlayerShipPanelTile2>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient(ModContent.ItemType<Cyberscrap>())
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SlayerShipPanelWall>(), 4)
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
        }
    }
}

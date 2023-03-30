using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Walls;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class SlayerShipPanelWall : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Ship Panel Wall");
        }

		public override void SetDefaults()
		{
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SlayerShipPanelWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<SlayerShipPanel2>())
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
        }
    }
}

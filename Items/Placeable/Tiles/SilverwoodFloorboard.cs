using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class SilverwoodFloorboard : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Silverwood Floorboard");
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodFloorboardTile>(), 0);
            Item.width = 24;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Silverwood>()
                .AddIngredient<AncientSlate>()
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}

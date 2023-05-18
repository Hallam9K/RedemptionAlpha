using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Items.Materials.PostML;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientAlloyBrick : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAlloyBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(15)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddIngredient<AncientAlloy>()
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}

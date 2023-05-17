using Redemption.Items.Placeable.Tiles;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodChandelier : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodChandelierTile>());
            Item.width = 34;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 600;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient<AncientChain>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
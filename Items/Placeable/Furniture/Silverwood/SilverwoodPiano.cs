using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodPiano : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodPianoTile>());
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient<Tiles.Silverwood>(15)
                .AddIngredient<AncientAlloy>(2)
                .AddIngredient<EvergoldBar>()
                .AddIngredient(ItemID.Bone)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
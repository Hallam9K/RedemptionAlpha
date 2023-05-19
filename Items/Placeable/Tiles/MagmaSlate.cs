using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class MagmaSlate : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ancient Magma Slate");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagmaSlateTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient<AncientSlate>(50)
                .AddCondition(Condition.NearLava)
                .Register();
        }
    }
}

using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class OvergrownLabPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Overgrown Laboratory Panel (Unsafe)");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OvergrownLabPlatingTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class OvergrownLabPlating2 : OvergrownLabPlating
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/OvergrownLabPlating";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Overgrown Laboratory Panel");
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<OvergrownLabPlatingTile2>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<LabPlatingUnsafe2>(10)
                .AddIngredient(ItemID.GrassSeeds)
                .Register();
        }
    }
}

using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabPlatingUnsafe : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Panel (Unsafe)");
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabPlatingTileUnsafe>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 0, 2, 0);
        }
    }
    public class LabPlatingUnsafe2 : LabPlatingUnsafe
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/LabPlatingUnsafe";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Unsafe Laboratory Panel");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<LabPlating>();
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<LabPlatingTileUnsafe2>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LabPlatingWallUnsafe2>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
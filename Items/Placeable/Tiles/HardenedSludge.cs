using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class HardenedSludge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HardenedSludgeSafe>();
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            Item.ResearchUnlockCount = 50;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardenedSludgeTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 75);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class HardenedSludgeSafe : HardenedSludge
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/HardenedSludge";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HardenedSludge>();
            Item.ResearchUnlockCount = 50;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<HardenedSludgeTileSafe>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ModContent.ItemType<ToxicBile>())
                .AddTile<XeniumRefineryTile>()
                .AddCondition(Condition.InGraveyard)
                .Register();
            CreateRecipe()
                .AddIngredient<HardenedSludgeWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

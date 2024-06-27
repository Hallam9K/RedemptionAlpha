using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class BlackHardenedSludge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BlackHardenedSludgeSafe>();
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            Item.ResearchUnlockCount = 50;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BlackHardenedSludgeTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 75);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class BlackHardenedSludgeSafe : BlackHardenedSludge
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/BlackHardenedSludge";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BlackHardenedSludge>();
            Item.ResearchUnlockCount = 50;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<BlackHardenedSludgeTileSafe>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient(ModContent.ItemType<ToxicBile>())
                .AddIngredient(ModContent.ItemType<RawXenium>())
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
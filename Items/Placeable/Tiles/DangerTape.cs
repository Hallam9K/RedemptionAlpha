using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class DangerTape : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Danger Tape");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<DangerTapeTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 25);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class DangerTape2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/DangerTape";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Danger Tape");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<DangerTape2Tile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 25);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(8)
                .AddIngredient(ModContent.ItemType<LabPlating>())
                .AddIngredient(ItemID.YellowPaint)
                .AddIngredient(ItemID.BlackPaint)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DangerTapeWall2>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

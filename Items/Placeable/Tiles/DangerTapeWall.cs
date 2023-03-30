using Redemption.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class DangerTapeWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Danger Tape");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<DangerTapeWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 25);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class DangerTapeWall2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/DangerTapeWall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Danger Tape");
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<DangerTapeWall2Tile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 25);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<DangerTape2>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

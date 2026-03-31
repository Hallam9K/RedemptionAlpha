using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientDirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<AncientDirtTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (Main.rand.NextBool(5))
            {
                resultType = ModContent.ItemType<ElderWood>();
                resultStack = 14;
            }
            else
            {
                resultType = ItemID.AshBlock;
                resultStack = 1;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock)
                .AddIngredient(ItemID.SiltBlock, 2)
                .AddTile(TileID.Solidifier)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemType<AncientDirtWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientDirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Can grow Ancient Trees");
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientDirtTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }

        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
        {
            if (Main.rand.Next(5) == 0)
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
                .AddIngredient(ItemID.AshBlock, 5)
                .AddTile(TileID.Solidifier)
                .Register(); 
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AncientDirtWall>(), 4)
                .Register();
        }
    }
}

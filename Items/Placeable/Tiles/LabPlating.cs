using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Panel");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabPlatingTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(66)
                .AddIngredient(ItemID.MartianConduitPlating, 66)
                .AddIngredient(ItemID.LunarOre)
                .AddTile(TileID.WorkBenches)
                .Register(); 
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlatingWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}

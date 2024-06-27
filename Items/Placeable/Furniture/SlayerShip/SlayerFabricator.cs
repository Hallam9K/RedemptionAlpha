using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class SlayerFabricator : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlayerFabricatorTile>(), 0);
            Item.width = 30;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Cyan;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 50)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
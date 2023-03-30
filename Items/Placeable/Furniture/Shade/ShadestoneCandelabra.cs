using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneCandelabra : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneCandelabraTile>(), 0);
            Item.width = 22;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 300;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 5)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
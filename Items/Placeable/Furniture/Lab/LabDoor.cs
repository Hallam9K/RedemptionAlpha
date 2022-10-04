using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabDoor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Laboratory Door");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabDoorClosed>(), 0);
            Item.width = 18;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 1500;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LabPlating>(), 7)
               .AddIngredient(ItemID.Glass, 2)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}
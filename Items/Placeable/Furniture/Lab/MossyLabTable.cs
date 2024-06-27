using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class MossyLabTable : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MossyLabTableTile>(), 0);
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<LabPlating>(12)
               .AddIngredient<Plating>(2)
               .AddIngredient(ItemID.GrassSeeds)
               .AddTile(TileID.WorkBenches)
               .Register();
            CreateRecipe()
               .AddIngredient<LabTable>()
               .AddIngredient(ItemID.GrassSeeds)
               .Register();
        }
    }
}
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class ServerCabinet : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ServerCabinetTile>(), 0);
            Item.width = 24;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 7000;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 12)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<AIChip>(), 1)
                .AddIngredient(ModContent.ItemType<Capacitator>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
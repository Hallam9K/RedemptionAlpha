using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCeilingLamp : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Ceiling Lamp");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCeilingLampTile>(), 0);
            Item.width = 24;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 9000;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 6)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
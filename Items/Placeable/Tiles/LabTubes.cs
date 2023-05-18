using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabTubes : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Tubes");
            /* Tooltip.SetDefault("'Filled with green sludge'" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTubeTile>(), 0);
            Item.width = 36;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class LabTubes2 : LabTubes
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/LabTubes";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Tubes");
            /* Tooltip.SetDefault("'Filled with green sludge'"); */
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTubeTile2>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LabPlatingUnsafe2>()
                .AddIngredient<XenomiteShard>(2)
                .Register();
        }
    }
}

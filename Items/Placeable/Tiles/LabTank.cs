using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabTank : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Tank");
            /* Tooltip.SetDefault("'Filled with green sludge'" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTankTile>(), 0);
            Item.width = 16;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class LabTank2 : LabTank
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/LabTank";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Tank");
            /* Tooltip.SetDefault("'Filled with green sludge'"); */
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<LabTankTile2>(), 0);
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

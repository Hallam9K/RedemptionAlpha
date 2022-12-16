using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria.GameContent.Creative;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class CyberTable : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<CyberTableTile>(), 0);
            Item.width = 32;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
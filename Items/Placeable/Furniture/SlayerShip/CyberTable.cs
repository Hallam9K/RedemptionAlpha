using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Items.Materials.HM;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class CyberTable : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<CyberTableTile>(), 0);
            Item.width = 32;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
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
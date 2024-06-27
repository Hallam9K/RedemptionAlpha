using Redemption.Items.Materials.HM;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class RedLaser : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RedLaserTile>(), 0);
            Item.width = 12;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class RedLaserSafe : GreenLaser
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/RedLaser";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<RedLaserTileSafe>(), 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient<HalogenLamp>(20)
                .AddIngredient<CorruptedXenomite>()
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}

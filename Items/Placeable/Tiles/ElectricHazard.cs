using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ElectricHazard : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electric Hazard");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 3));

            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ElectricHazardTile>(), 0);
            Item.width = 32;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 0, 15, 0);
        }
    }
}

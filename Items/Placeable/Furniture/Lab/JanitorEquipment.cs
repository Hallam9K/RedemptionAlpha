using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class JanitorEquipment : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JanitorEquipmentTile>(), 0);
            Item.width = 36;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}